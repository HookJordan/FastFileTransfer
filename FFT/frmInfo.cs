using FFT.Core.IO;
using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmInfo : Form
    {
        private Client client;
        public frmInfo(Client client)
        {
            InitializeComponent();

            this.client = client;
            this.Text = $"Connected - {client.IP}";
        }

        private void frmInfo_Load(object sender, EventArgs e)
        {
            this.FormClosing += FrmInfo_FormClosing;
        }

        private void FrmInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!txtLog.Text.Contains("End of connection"))
            {
                if (MessageBox.Show("Closing this window will close the remote connection. Are you sure you wish to cancel this session?", "Cancel Session", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    client.Close();
                    FileExplorer.Log?.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public delegate void WriteTextDelegate(string text);
        public void WriteText(string text)
        {
            // Invoke is required as we will be updating from a different thread
            if (InvokeRequired)
            {
                var d = new WriteTextDelegate(WriteText);
                txtLog?.Invoke(d, new object[] { text });
            }
            else
            {
                txtLog.AppendText(text + Environment.NewLine);
            }
        }
    }
}
