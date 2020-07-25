using FFT.Core;
using FFT.Core.Compression;
using FFT.Core.Encryption;
using System;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmPreferences : Form
    {
        public Configuration configuration { get; private set; }
        public frmPreferences(Configuration configuration)
        {
            InitializeComponent();

            this.configuration = configuration;
        }

        private void frmPreferences_Load(object sender, EventArgs e)
        {
            this.Text = "Preferences";

            // Load Port
            this.numPort.Value = configuration.Port;
            this.numBufferSize.Value = configuration.BufferSize;

            // Load compression mode
            switch (configuration.compressionAlgorithm)
            {
                case CompressionAlgorithm.Disabled:
                    cbDisableCompression.Checked = true;
                    break;
                case CompressionAlgorithm.GZIP:
                    cbGZIP.Checked = true;
                    break;
                case CompressionAlgorithm.LZMA:
                    cbLZMA.Checked = true;
                    break;
                case CompressionAlgorithm.LZO:
                    cbLZO.Checked = true;
                    break;
            }

             // Load encryption mode
            switch (configuration.cryptoAlgorithm)
            {
                case CryptoAlgorithm.Disabled:
                    cbDisabledCrypto.Checked = true;
                    break;
                case CryptoAlgorithm.XOR:
                    cbXOR.Checked = true;
                    break;
                case CryptoAlgorithm.RC4:
                    cbRC4.Checked = true;
                    break;
                case CryptoAlgorithm.AES:
                    cbAES.Checked = true;
                    break;
                case CryptoAlgorithm.Blowfish:
                    cbBlowFish.Checked = true;
                    break;
            }

            // Load Protected folders
            this.lstProtected.Items.AddRange(configuration.ProtectedFolders.ToArray());

            cbDebug.Checked = configuration.DebugMode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            // Set port
            configuration.Port = (int)numPort.Value;
            configuration.BufferSize = (int)numBufferSize.Value;

            // Set compression mode
            if (cbDisableCompression.Checked)
            {
                configuration.compressionAlgorithm = CompressionAlgorithm.Disabled;
            }
            else if (cbGZIP.Checked)
            {
                configuration.compressionAlgorithm = CompressionAlgorithm.GZIP;
            }
            else if (cbLZMA.Checked)
            {
                configuration.compressionAlgorithm = CompressionAlgorithm.LZMA;
            }
            else if (cbLZO.Checked)
            {
                configuration.compressionAlgorithm = CompressionAlgorithm.LZO;
            }

            // Set encryption mode
            if (cbDisabledCrypto.Checked)
            {
                configuration.cryptoAlgorithm = CryptoAlgorithm.Disabled;
            }
            else if (cbXOR.Checked)
            {
                configuration.cryptoAlgorithm = CryptoAlgorithm.XOR;
            }
            else if (cbRC4.Checked)
            {
                configuration.cryptoAlgorithm = CryptoAlgorithm.RC4;
            }
            else if (cbAES.Checked)
            {
                configuration.cryptoAlgorithm = CryptoAlgorithm.AES;
            } else if (cbBlowFish.Checked)
            {
                configuration.cryptoAlgorithm = CryptoAlgorithm.Blowfish;
            }

            configuration.ProtectedFolders = new System.Collections.Generic.List<string>();
            foreach (var item in lstProtected.Items)
            {
                configuration.ProtectedFolders.Add(item.ToString());
            }

            configuration.DebugMode = cbDebug.Checked;


            this.DialogResult = DialogResult.OK;
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    lstProtected.Items.Add(fbd.SelectedPath);
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstProtected.SelectedItem != null)
            {
                var dir = (string)lstProtected.SelectedItem;

                if (MessageBox.Show($"By removing this protected folder remotely connected users will be able to access and modify it's contents. Are you sure you wish to remove:\n'{dir}'?", "Remove Forbidden Folder", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    lstProtected.Items.Remove(lstProtected.SelectedItem);
                }
            }
        }
    }
}
