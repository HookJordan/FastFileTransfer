using FFT.Core.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFT.Core.IO
{
    public class Logger : IDisposable
    {
        public string FileName { get; private set; }
        public bool isDisposed { get; private set; } = false;

        private StreamWriter streamWriter;
        private frmInfo frmInfo;

        public Logger(Client client)
        {
            this.FileName = $"logs\\{client.IP}\\{DateTime.Now.ToString("yyyy-MM-dd")}.txt";

            if (!Directory.Exists($"logs\\{client.IP}"))
            {
                Directory.CreateDirectory($"logs\\{client.IP}");
            }

            this.streamWriter = new StreamWriter(this.FileName, true);
            this.streamWriter.AutoFlush = true; // Auto flush to file in case of any crashes / errors

            // Create new thread for the visua logging UI
            // This could be changed to use the normal UI thread as well in the future.
            var uiThread = new Thread(() => 
            {
                frmInfo = new frmInfo(client, FileName);
                frmInfo.ShowDialog();
            });
            uiThread.IsBackground = false;
            uiThread.Start();
 
        }

        public void Info(string msg)
        {
            WriteLine($"[INFO] {msg}");
        }

        public void Error(string msg)
        {
            WriteLine($"[ERROR] {msg}");
        }

        public void Debug(string msg)
        {
            WriteLine($"[DEBUG] {msg}");
        }

        private void WriteLine(string msg)
        {
            var log = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {msg}";
            streamWriter?.WriteLine(log);
            frmInfo?.WriteText(log);
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Info("End of connection");
                streamWriter.Dispose();
            }
        }
    }
}
