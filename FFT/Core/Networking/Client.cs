using FFT.Core.Compression;
using FFT.Core.Encryption;
using FFT.Core.IO;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FFT.Core.Networking
{
    public class Client
    {
        public string IP { get; private set; }
        public int Port { get; private set; }
        public string Password { get; private set; }
        public bool Connected => socket.Connected;

        private Socket socket;
        private string encodedPassword;

        public CompressionProvider compressionProvider { get; private set; }
        public CryptoProvider cryptoProvider { get; private set; }
        public int BufferSize { get; private set; }

        public bool InboundConnection { get; private set; } = false;

        public Client(Socket socket, string password, CompressionProvider compressionProvider, CryptoProvider cryptoProvider, int bufferSize)
        {
            this.InboundConnection = true;
            this.compressionProvider = compressionProvider;
            this.cryptoProvider = cryptoProvider;
            this.Password = password;
            this.encodedPassword = Encryption.SHA.Encode(password);
            this.socket = socket;
            this.BufferSize = bufferSize;

            try
            {
                this.IP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
                this.Port = ((IPEndPoint)socket.LocalEndPoint).Port;

                // Send compression information
                byte[] compressionMode = BitConverter.GetBytes((int)compressionProvider.algorithm);
                socket.Send(compressionMode);

                // Send crypto information
                byte[] encryptionMode = BitConverter.GetBytes((int)cryptoProvider.algorithm);
                socket.Send(encryptionMode);

                // Begin receiving real data now
                this.socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Receive, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception($"Connection with {IP} was lost.");
            }

        }

        public Client(string ip, int port, string password, int bufferSize)
        {
            this.Password = password;
            this.encodedPassword = Encryption.SHA.Encode(password);
            this.IP = ip;
            this.Port = port;
            this.BufferSize = bufferSize;
        }

        public void Connect()
        {
            try
            {
                // Create new socket
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.socket.Connect(new IPEndPoint(ResolveDns(this.IP), this.Port));

                if (this.socket.Connected)
                {
                    // Password is sent as as sha hash for verification
                    // Hash is encrypted with RC4 and the set password
                    // The raw password that is actually used to encrypt the data
                    // is never sent over the network
                    byte[] payload = Encoding.ASCII.GetBytes(this.encodedPassword);
                    RC4.Perform(ref payload, this.Password);

                    this.socket.Send(BitConverter.GetBytes(payload.Length));
                    this.socket.Send(payload);

                    // Wait for server to confirm that the settings are match
                    byte[] confirm = new byte[1];
                    int rec = this.socket.Receive(confirm);
                    if (rec == confirm.Length && confirm[0] == 1)
                    {
                        // Password is correct, start receiving data
                        this.socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, BeginReceiveConfigurations, null);
                    }
                    else
                    {
                        // Failed to sync with server
                        this.ClientReady?.Invoke(this, false);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception($"Unable to connect to {IP} on port {Port}");
            }
        }

        private IPAddress ResolveDns(string dns)
        {
            try
            {
                return IPAddress.Parse(dns);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Dns.GetHostAddresses(dns)[0];
            }
        }

        private void BeginReceiveConfigurations(IAsyncResult ir)
        {
            try
            {
                // Receive compression preferences 
                byte[] compressionType = new byte[4];
                this.socket.Receive(compressionType);
                this.compressionProvider = new CompressionProvider((CompressionAlgorithm)BitConverter.ToInt32(compressionType, 0));

                // Receive crypto preferences
                byte[] encryptionMode = new byte[4];
                this.socket.Receive(encryptionMode);
                this.cryptoProvider = new CryptoProvider((CryptoAlgorithm)BitConverter.ToInt32(encryptionMode, 0), this.Password);

                // Notify Ready
                this.ClientReady?.Invoke(this, true);

                // Begin receiving real data now
                this.socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Receive, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.Disconnected?.Invoke(this);
            }
        }

        private void Receive(IAsyncResult ir)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    int read = 0;
                    int size = 0;

                    byte[] buffer = new byte[4];

                    read = this.socket.Receive(buffer);

                    if (read == 0)
                    {
                        throw new Exception("Bad data received.");
                    } 
                    else
                    {
                        // Determine size of incoming payload
                        size = BitConverter.ToInt32(buffer, 0);
                        buffer = new byte[1024 * BufferSize];

                        // Well there is still data to receive
                        while (size > 0)
                        {
                            // Receive the data and store it in the memory stream
                            read = this.socket.Receive(buffer, 0, size > buffer.Length ? buffer.Length : size, SocketFlags.None);
                            ms.Write(buffer, 0, read);
                            size -= read;
                        }

                        // Copy ms to buffer
                        buffer = ms.ToArray();
                    }

                    // Decrypt the packet
                    buffer = cryptoProvider.Decrypt(buffer);

                    // PASS THE PACKET TO A HANDLER?
                    this.PacketReceived?.Invoke(this, buffer);

                    buffer = null;
                }

                // Next packet
                this.socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Receive, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.Disconnected?.Invoke(this);
            }
        }

        public void Send(byte[] payload)
        {
            try
            {
                payload = cryptoProvider.Encrypt(payload);

                this.socket.Send(BitConverter.GetBytes(payload.Length));
                this.socket.Send(payload);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Send(Packet p)
        {
            if (InboundConnection)
                FileExplorer.Debug($"Packet Sent: {p.PacketHeader}, Length {p.Payload.Length}");

            this.Send(p.ToRawBytes());
        }

        public void Close()
        {
            if (this.socket.Connected)
            {
                this.socket.Close();
            }

            this.socket.Dispose();
        }

        public void TriggerDisconnect()
        {
            this.Disconnected?.Invoke(this);
        }

        // Events
        public delegate void PacketReceivedHandler(Client client, byte[] payload);
        public event PacketReceivedHandler PacketReceived;
        public delegate void DisconnectedHandler(Client client);
        public event DisconnectedHandler Disconnected;
        public delegate void ClientReadyHandler(Client client, bool success);
        public event ClientReadyHandler ClientReady;
    }
}
