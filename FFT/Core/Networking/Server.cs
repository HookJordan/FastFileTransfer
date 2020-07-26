using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FFT.Core.Networking
{
    class Server
    {
        public bool Running { get; private set; }
        public int Port { get; private set; }
        public string Password { get; private set; }
        private Socket listener;

        public bool AcceptingConnections { get; set; }

        public Server(int port, String password)
        {
            this.Port = port;
            this.Password = password;
        }

        public void Start()
        {
            try
            {
                this.Running = true;
                this.AcceptingConnections = true;
                this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.listener.Bind(new IPEndPoint(IPAddress.Any, this.Port));

                if (this.listener.IsBound)
                {
                    this.listener.Listen(1);

                    this.listener.BeginAccept(this.AcceptSocket, null);
                }

            } 
            catch (Exception e)
            {
                this.Running = false;
                Console.WriteLine(e.Message);
                throw new Exception($"Unable to start server socket on port {Port}");
            }
        }

        public void ChangePort(int port)
        {
            if (port != this.Port)
            {
                this.Stop();
                this.Port = port;
                this.Start();
            }
        }

        public void Stop()
        {
            this.Running = false;
            this.AcceptingConnections = false;
            if (this.listener != null)
            {
                this.listener.Close();
            }
        }

        private void AcceptSocket(IAsyncResult ir)
        {
            try
            {
                Socket incoming = this.listener.EndAccept(ir);

                if (Running && AcceptingConnections)
                {
                    this.Handshake(incoming);

                    // Await next connection
                    this.listener.BeginAccept(this.AcceptSocket, null);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Handshake(Socket sock)
        {
            try
            {
                // First packet received on connection will be amount of data incoming
                byte[] sizeAsByte = new byte[4];
                int rec = sock.Receive(sizeAsByte);

                if (rec == 0)
                {
                    sock.Close();
                } 
                else
                {
                    int size = BitConverter.ToInt32(sizeAsByte, 0);

                    // Password validation should not be bigger then 4096 bytes
                    if (size > 1024)
                    {
                        sock.Close();
                    }

                    byte[] data = new byte[size];
                    rec = sock.Receive(data);

                    // Password is received in RC4(SHA(password), password) format
                    // We need to decrypt the password and verify the sha hash matches 
                    // the hash of the password the server is using
                    Encryption.RC4.Perform(ref data, this.Password);
                    
                    if (Encoding.ASCII.GetString(data) == Encryption.SHA.Encode(this.Password))
                    {
                        sock.Send(new byte[] { 1 });
                        this.ConnectionRequest(this, sock);
                    } 
                    else
                    {
                        sock.Send(new byte[] { 0 });
                        sock.Close();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ShutDown()
        {
            this.Running = false;
        }


        public void SetPassword(string password)
        {
            lock (this)
            {
                this.Password = password;
            }
        }

        public void SetPort(int port)
        {
            lock(this)
            {
                this.Port = port;

                this.Start();
            }
        }

        public delegate void ConnectionRequestHandler(Server server, Socket socket);
        public event ConnectionRequestHandler ConnectionRequest;
    }
}
