using System;
using System.Windows.Forms;

namespace FFT
{
    static class Program
    {
        public static string BUILD_VERSION = "0.0.2";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
