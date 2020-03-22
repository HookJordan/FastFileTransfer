using FFT.Core.Compression;
using FFT.Core.Encryption;
using FFT.Core.IO;
using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmMain : Form
    {
        private Server server;
        private Client client;
        private Client incomingClient;
        private CompressionProvider compressionProvider;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("FFT - FastFileTransfer {0:0}", Program.BUILD_VERSION);

            // Remove ugly buttons
            this.numPort.Controls[0].Visible = false;

            // Initial information load
            this.refreshIncomePass();
            this.refreshExternalIp();

            // Load user config
            this.loadPreferences();
            this.compressionProvider = new CompressionProvider(CompressionAlgorithm.GZIP);

            // Start waiting for incoming connections
            this.startServer();
        }

        private void startServer()
        {
            this.server = new Server(Convert.ToInt32(this.txtIncomePort.Text), this.txtIncomePass.Text);

            try
            {
                this.server.ConnectionRequest += Server_ConnectionRequest;
                this.server.Start();
                this.stsStatus.Text = "Awaiting Connection";
                this.stsStatus.ForeColor = Color.Green;
            }
            catch (Exception e)
            {
                this.stsStatus.Text = "Error starting server";
                this.stsStatus.ForeColor = Color.Red;
                this.btnRefresh.Enabled = false;

                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Server_ConnectionRequest(Server server, System.Net.Sockets.Socket socket)
        {
            if (this.incomingClient == null || !this.incomingClient.Connected)
            {
                this.server.AcceptingConnections = false;
                incomingClient = new Client(socket, this.txtIncomePass.Text, compressionProvider, new CryptoProvider(CryptoAlgorithm.AES, server.Password));
                incomingClient.PacketReceived += Client_PacketReceived;
                incomingClient.Disconnected += IncomingClient_Disconnected;

                Console.WriteLine($"NEW CONNECTION {incomingClient.IP}:{incomingClient.Port}");

                // Send a quick packet to ensure the connection is successful
                incomingClient.Send(Packet.Create(PacketHeader.PingPong, "TEST"));

                Invoke((MethodInvoker)delegate
                {
                    this.stsStatus.Text = $"Connected - {incomingClient.IP}:{incomingClient.Port}";
                    this.stsStatus.ForeColor = Color.Blue;
                });
            }
            else
            {
                socket.Close();
            }
        }

        private void IncomingClient_Disconnected(Client client)
        {
            this.server.AcceptingConnections = true;
            Invoke((MethodInvoker)delegate
            {
                this.stsStatus.Text = "Awaiting Connection";
                this.stsStatus.ForeColor = Color.Green;
            });
        }

        private void loadPreferences()
        {
            // todo: Load user configuration (load user defined port and defined password is exists)
        }

        private void refreshIncomePass()
        {
            this.txtIncomePass.Text = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            if (this.server != null)
            {
                this.server.SetPassword(this.txtIncomePass.Text);
            }
        }

        private void refreshExternalIp()
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    txtIncomeIp.Text = wc.DownloadString("https://api.ipify.org");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                txtIncomeIp.Text = "Failed to load...";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.refreshIncomePass();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.server.Stop();
            Application.Exit(new CancelEventArgs(true));
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnConnect.Enabled = false;
                this.client = new Client(this.txtIp.Text, (int)this.numPort.Value, this.txtPassword.Text);

                if (this.client.Connected)
                {
                    var fb = new frmFileBrowser(this.client);
                    fb.FormClosing += Fb_FormClosing;
                    fb.Show();
                }
            }
            catch (Exception err)
            {
                this.btnConnect.Enabled = true;
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Fb_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnConnect.Enabled = true;
        }

        private void Client_PacketReceived(Client client, byte[] payload)
        {
            Packet p = Packet.FromPayload(payload);

            if (p.PacketHeader != PacketHeader.FileChunk)
            {
                Console.WriteLine($"Received packet {p.PacketHeader} from {client.IP}:{client.Port}");
            }

            FileExplorer.HandlePacket(client, p);
        }

        private void txtIncomePass_TextChanged(object sender, EventArgs e)
        {
            if (this.server != null)
            {
                this.server.SetPassword(this.txtIncomePass.Text);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This action will launch a new window you web browser. Are you sure you wish to continue?", "About", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://jordanhook.com/index.php?&controller=home&view=projectsItem&projectId=8");
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmPreferences preferences = new frmPreferences())
            {
                preferences.ShowDialog();

                // Save Configuration
            }
        }
    }
}
