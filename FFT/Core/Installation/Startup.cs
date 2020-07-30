using System;
using System.Diagnostics;

namespace FFT.Core.Installation
{
    class Startup : IDisposable
    {
        public string Name { get; private set; }
        public string ExecutablePath { get; private set; }

        public Startup(string name, string path)
        {
            this.Name = name;
            this.ExecutablePath = path;
        }

        public void Install()
        {
            ExecuteCommand($"schtasks /create /sc onlogon /tn \"{Name}\" /rl highest /tr \"{ExecutablePath}\"");
        }

        public void Uninstall()
        {
            ExecuteCommand($"schtasks /Delete /Tn \"{Name}\" /f");
        }

        private void ExecuteCommand(string cmd)
        {
            // Create a hidden CMD instance with input redirects
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;

            // Start the process
            Process p = Process.Start(psi);

            // Execute the command
            p.StandardInput.WriteLine(cmd);

            // If startup already exists, we need to exit twice
            // This could be cleaned up to actually handle the errors
            // that occurs but this is a quick work around for now
            p.StandardInput.WriteLine("exit");
            p.StandardInput.WriteLine("exit");

            // Wait for commands to finish
            p.WaitForExit();
        }

        public void Dispose()
        {
            Name = null;
            ExecutablePath = null;
        }
    }
}
