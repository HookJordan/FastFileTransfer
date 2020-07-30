using System.Net;
using System.Web.Script.Serialization;

namespace FFT.Core.Networking
{
    class Updater
    {
        private static readonly string UPDATE_CHECK_URL = "https://jordanhook.com/index.php?&controller=api&view=updater&appId=15";
        public static UpdateInfo CheckForUpdates(string version)
        {
            using (WebClient wc = new WebClient())
            {
                // Download the updater information
                string data = wc.DownloadString(UPDATE_CHECK_URL);

                // If an update is found
                return new JavaScriptSerializer().Deserialize<UpdateInfo>(data);
            }

        }
            
        internal struct UpdateInfo
        {
            public string name { get; set; }
            public string releaseDate { get; set; }
            public string version { get; set; }
            public string package { get; set; }
        }
    }
}
