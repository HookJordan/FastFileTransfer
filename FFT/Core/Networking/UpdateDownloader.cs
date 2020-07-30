using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.Networking
{
    class UpdateDownloader : IDisposable
    {
        public dlgLoad dlgLoad { get; private set; }
        public string CurrentVersion { get; private set; }
        public string TargetVersion { get; private set; }
        public string Endpoint { get; private set; }
        public string Destination { get; private set; }

        private WebClient downloader;

        public UpdateDownloader(string targetV, string currentV, string appId, string destFile)
        {
            CurrentVersion = currentV;
            TargetVersion = targetV;
            Endpoint = $"https://jordanhook.com/index.php?&controller=api&view=download&appId={appId}";
            Destination = destFile;

            // Init objects
            dlgLoad = new dlgLoad("Downloading Update", $"Current Version: {CurrentVersion}\nTarget Version: {TargetVersion}\n\nDestination:\n{destFile}");

            downloader = new WebClient();
            downloader.DownloadFileCompleted += Downloader_DownloadFileCompleted;
        }

        private void Downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            dlgLoad.TriggerDone("Update Downloaded!", $"The updated package has been downloaded and saved to: {Destination}");
        }

        public void Download()
        {
            downloader.DownloadFileAsync(new Uri(Endpoint), Destination);
            dlgLoad.ShowDialog();
        }

        public void Dispose()
        {
            dlgLoad.Dispose();
            downloader.Dispose();
        }
    }
}
