using FFT.Core.IO;
using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFT
{
    public partial class frmInfo : Form
    {
        private Client client;
        public frmInfo(Client client, string logPath)
        {
            InitializeComponent();

            this.client = client;
            this.Text = $"Connected - {client.IP}";
            WriteText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [INFO] Log File: {logPath}");
            PrettyPrint();
        }

        private void frmInfo_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.FormClosing += FrmInfo_FormClosing;
            this.rtxtLog.TextChanged += RtxtLog_TextChanged;
        }

        private void RtxtLog_TextChanged(object sender, EventArgs e)
        {
            PrettyPrint();
        }

        private void PrettyPrint()
        {
            // Log highlighting
            foreach (Match match in Regex.Matches(rtxtLog.Text, @"ERROR"))
            {
                SetColor(match, Color.Red);
            }

            foreach (Match match in Regex.Matches(rtxtLog.Text, @"INFO"))
            {
                SetColor(match, Color.Green);
            }

            foreach (Match match in Regex.Matches(rtxtLog.Text, @"DEBUG"))
            {
                SetColor(match, Color.Blue);
            }

            rtxtLog.SelectionStart = rtxtLog.Text.Length;
            rtxtLog.ScrollToCaret();
        }

        private void SetColor(Match match, Color color)
        {
            rtxtLog.Select(match.Index, match.Length);
            rtxtLog.SelectionColor = color;
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
                rtxtLog.AppendText(text + Environment.NewLine);
            }
        }
    }
}
