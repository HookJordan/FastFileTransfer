using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FFT.Core.Networking
{
    class Updater
    {
        private static readonly string UPDATE_CHECK_URL = "https://www.dropbox.com/s/6fg3mbt0zpm1usb/Version.txt?dl=1";
        public static UpdateInfo CheckForUpdates(string version)
        {
            using (WebClient wc = new WebClient())
            {
                // Download the updater information
                string data = wc.DownloadString(UPDATE_CHECK_URL);
                var parsed = FromString(data);

                // If an update is found
                return parsed;
            }

        }

        private static UpdateInfo FromString(string data)
        {
            string[] lines = data.Replace("\r", "").Split('\n');

            // TODO: Additional update instructions could be provided here
            // For example, we could specify the update to download a file or launch a page instead
            return new UpdateInfo()
            {
                Version = lines[0],
                Link = lines[1]
            };
        }

        internal struct UpdateInfo
        {
            public string Version;
            public string Link;
        }

    }
}
