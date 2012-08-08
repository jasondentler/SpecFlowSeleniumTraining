using System;
using System.Diagnostics;

namespace Specs.Infrastructure
{
    public class RavenInstance : IDisposable
    {

        private readonly Process _process;

        public RavenInstance()
            : this(8081)
        {
        }

        public RavenInstance(int portNumber)
        {
            _process = Start(portNumber);
        }

        private static Process Start(int portNumber)
        {
            var path = Settings.RavenDbExecutablePath;
            var args = string.Format("/ram --set=Raven/Port=={0}", portNumber);
            var si = new ProcessStartInfo(path, args)
            {
                CreateNoWindow = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            };
            return Process.Start(si);
        }

        private static void Stop(Process process)
        {
            if (process.HasExited)
                return;

            process.StandardInput.WriteLine("q");
            process.WaitForExit((int)TimeSpan.FromSeconds(5).TotalMilliseconds);

            if (process.HasExited)
                return;

            Console.WriteLine("RavenDB process appears to be hung. Attempting to kill it.");
            process.Kill();
        }

        public void Dispose()
        {
            if (_process != null && !_process.HasExited)
                Stop(_process);
        }
    }
}