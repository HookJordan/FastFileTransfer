using System;
using System.Drawing;
using System.Windows.Forms;

namespace FFT
{
    public partial class dlgLoad : Form
    {
        public dlgLoad(string title, string msg)
        {
            InitializeComponent();

            this.Text = title;

            // Set body as well here
            lblMsg.Text = msg;
        }

        private void dlgLoad_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.FormClosing += DlgLoad_FormClosing;
        }
            
        private void DlgLoad_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        public void TriggerDone(string msg, string title)
        {
            MessageBox.Show(title, msg, MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Hide();
        }
    }
}
