using FFT.Core;
using FFT.Core.Compression;
using FFT.Core.Encryption;
using FFT.Core.IO;
using FFT.Core.Networking;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmMain : Form
    {
        private Server server;
        private Client client;
        private Client incomingClient;
        private Configuration configuration;
        private bool closeToTaskbar = true;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            this.Text = String.Format("FFT - FastFileTransfer {0:0}", Program.BUILD_VERSION);
            this.icnMain.Text = this.Text;

            // Remove ugly buttons
            this.numPort.Controls[0].Visible = false;

            // Initial information load
            this.refreshIncomePass();
            this.refreshExternalIp();

            // Load user config
            this.loadPreferences();

            // Start waiting for incoming connections
            this.startServer();

            // Check for updates on startup
            UpdateCheck(true);
        }

        private void startServer()
        {
            this.server = new Server(Convert.ToInt32(this.txtIncomePort.Text), this.txtIncomePass.Text);

            // If unattended access is configued, add password to the server
            if (configuration.UnattendedAccess)
            {
                this.server.SetPersonalPassword(configuration.PersonalPassword);
            }

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

        private void Server_ConnectionRequest(Server server, System.Net.Sockets.Socket socket, string password)
        {
            if (this.incomingClient == null || !this.incomingClient.Connected)
            {
                this.server.AcceptingConnections = false;
                incomingClient = new Client(socket, password, new CompressionProvider(configuration.compressionAlgorithm), new CryptoProvider(configuration.cryptoAlgorithm, password), configuration.BufferSize);
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
            this.incomingClient = null;
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
            this.configuration = Configuration.FromFile("config.data");
            this.txtIncomePort.Text = configuration.Port.ToString();
            FileExplorer.Config = this.configuration;
            UpdateStatusStrip();   
        }

        private void UpdateStatusStrip()
        {
            lblCompressionMode.Text = configuration.compressionAlgorithm.ToString();
            lblEncryption.Text = configuration.cryptoAlgorithm.ToString();
            lblCompressionMode.ForeColor = (configuration.compressionAlgorithm == CompressionAlgorithm.Disabled) ? Color.Red : Color.Green;
            lblEncryption.ForeColor = (configuration.cryptoAlgorithm == CryptoAlgorithm.Disabled) ? Color.Red : Color.Green;
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
            closeToTaskbar = false;
            this.server?.Stop();
            Application.Exit(new CancelEventArgs(true));
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text == string.Empty)
                    throw new Exception("Password field can not be empty. Please enter a password to continue.");

                this.btnConnect.Enabled = false;
                this.client = new Client(this.txtIp.Text, (int)this.numPort.Value, this.txtPassword.Text, configuration.BufferSize);

                // Setup callback for when connection has been established
                this.client.ClientReady += Client_ClientReady;

                // Begin connection / pairing
                this.client.Connect();
            }
            catch (Exception err)
            {
                this.btnConnect.Enabled = true;
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Client_ClientReady(Client client, bool success)
        {
            Invoke((MethodInvoker)delegate
            {
                if (success)
                {
                    var fb = new frmFileBrowser(client);
                    fb.FormClosing += Fb_FormClosing;
                    fb.Show();
                }
                else
                {
                    MessageBox.Show("Unable to connect to server. Please verify your configuration and try again.", "Unable to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnConnect.Enabled = true;
                }
            });
        }

        private void Fb_FormClosing(object sender, FormClosingEventArgs e)
        {
            btnConnect.Enabled = true;
            UpdateStatusStrip();
        }

        private void Client_PacketReceived(Client client, byte[] payload)
        {
            Packet p = Packet.FromPayload(payload);

            if (p.PacketHeader != PacketHeader.FileChunk)
            {
                Console.WriteLine($"Received packet {p.PacketHeader} from {client.IP}:{client.Port}");
                if (p.PacketHeader == PacketHeader.GoodBye)
                {
                    client.TriggerDisconnect();
                }
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
            if (MessageBox.Show("This action will launch a new window in your web browser. Are you sure you wish to continue?", "About", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("https://jordanhook.com/index.php?&controller=home&view=appDetails&appId=15");
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (frmPreferences preferences = new frmPreferences(configuration))
            {
                if (preferences.ShowDialog() == DialogResult.OK)
                {
                    // Save Configuration
                    configuration = preferences.configuration;
                    configuration.Save("config.data");

                    this.txtIncomePort.Text = configuration.Port.ToString();
                    this.server.ChangePort(configuration.Port);

                    // Update Display of other settings
                    this.UpdateStatusStrip();
                    FileExplorer.Config = configuration;

                    if (configuration.UnattendedAccess)
                    {
                        server.SetPersonalPassword(configuration.PersonalPassword);
                    }
                    else
                    {
                        server.SetPersonalPassword(null);
                    }

                    // TODO: Check add to startup
                }
            }
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateCheck();
        }

        private void UpdateCheck(bool systemCheck = false)
        {
            try
            {
                var update = Updater.CheckForUpdates(Program.BUILD_VERSION);

                int numericVersion = int.Parse(Program.BUILD_VERSION.Replace(".", ""));
                int numericUpdate = int.Parse(update.version.Replace(".", ""));

                if (numericUpdate > numericVersion)
                {
                    if (MessageBox.Show($"A newer version of FastFileTransfer has been found. Would you like to download version: {update.version}?", "Update Found", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // Create new instance of the downloader
                        using (UpdateDownloader ud = new UpdateDownloader(update.version, Program.BUILD_VERSION, "15", $"{Application.StartupPath}\\{update.package.Replace("apps/", "")}"))
                        {
                            // Downloader will handle rest of update from here
                            ud.Download();
                        }
                    }
                }
                else
                {
                    // Alert user even when no update is found
                    if (!systemCheck)
                    {
                        MessageBox.Show($"FastFileTransfer is up to date.", "Checking for updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception e)
            { 
                MessageBox.Show("Unable to fetch updates at this time. Please try again later!", "Error Checking For Updates", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(e.Message);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clsose to notification bar
            this.Hide();

            // Hide to task bar
            if (closeToTaskbar)
            {
                icnMain.ShowBalloonTip(5000, "FastFileTransfer", "FastFileTransfer is still running in the background.", ToolTipIcon.Info);
                e.Cancel = true;
            }
        }

        private void checkForUpdatesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            checkForUpdatesToolStripMenuItem.PerformClick();
        }

        private void preferencesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            preferencesToolStripMenuItem.PerformClick();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            quitToolStripMenuItem.PerformClick();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void icnMain_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
