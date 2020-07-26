using System;
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
            this.FormClosing += DlgLoad_FormClosing;
        }

        private void DlgLoad_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
