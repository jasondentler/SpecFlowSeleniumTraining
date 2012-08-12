using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Specs.Infrastructure
{
    public class Raven : IInfrastructure
    {

        private static readonly int DefaultPort = GetRandomUnusedPort();
        private const string DefaultConnectionStringName = "RavenDB";

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
        
        private readonly Website _website;
        private readonly int _portNumber;
        private readonly string _connectionStringName;

        private Process _process;

        public Raven()
            : this(new Website(), DefaultPort, DefaultConnectionStringName)
        {
        }

        public Raven(Website website)
            : this(website, DefaultPort, DefaultConnectionStringName)
        {
        }

        public Raven(int portNumber)
            : this(new Website(), portNumber, DefaultConnectionStringName)
        {
        }

        public Raven(string connectionStringName)
            : this(new Website(), DefaultPort, connectionStringName)
        {
        }

        public Raven(Website website, int portNumber, string connectionStringName)
        {
            _website = website;
            _portNumber = portNumber;
            _connectionStringName = connectionStringName;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            _process = Start(_portNumber);

            var connectionString = string.Format("Url=http://localhost:{0}", _portNumber);
            _website.SetConnectionString(_connectionStringName, connectionString, null);
        }

        public void Stop()
        {
            if (_process != null && !_process.HasExited)
                Stop(_process);
        }

        public void Reset()
        {
            if (_process == null || _process.HasExited)
            {
                Start();
                return;
            }

            if (_process.StartInfo.RedirectStandardInput)
            {
                _process.StandardInput.WriteLine("reset");
                return;
            }

            Stop();
            Start();
        }

        private Process Start(int portNumber)
        {
            var path = Settings.RavenDbExecutablePath;

            var args = string.Format("/ram --set=Raven/Port=={0}", portNumber);

            var si = new ProcessStartInfo(path, args)
                         {
                             UseShellExecute = false,
                             CreateNoWindow = true
                         };

            Console.WriteLine("Starting RavenDB in-memory server on port {0}", portNumber);
            var process = Process.Start(si);
            return process;
        }
        
        private void Stop(Process process)
        {
            Console.WriteLine("Stopping RavenDB in-memory server");
            
            if (process.HasExited)
                return;

            if (process.StartInfo.RedirectStandardInput)
            {
                process.StandardInput.WriteLine("q");
                process.WaitForExit((int) TimeSpan.FromSeconds(5).TotalMilliseconds);

                if (process.HasExited)
                    return;

                Console.WriteLine("RavenDB process appears to be hung. Attempting to kill it.");
            }

            process.Kill();
        }

    }
}
