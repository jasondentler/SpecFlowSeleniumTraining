using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace Specs.Infrastructure
{
    public class IISExpress : IInfrastructure
    {

        private static readonly int? DefaultPortNumber = GetRandomUnusedPort();

        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private readonly Website _website;
        private readonly int? _portNumber;
        private readonly string _iisExpressPath;
        private Process _iisProcess;
        private Uri _url;

        public Uri Url { get { return _url; } }

        public IISExpress()
            : this(new Website(), DefaultPortNumber, Settings.IISExpressPath)
        {
        }

        public IISExpress(Website website)
            : this(website, DefaultPortNumber, Settings.IISExpressPath)
        {
        }

        public IISExpress(int portNumber)
            : this(new Website(), portNumber)
        {
        }


        public IISExpress(Website website, int portNumber)
            : this(website, portNumber, Settings.IISExpressPath)
        {
        }

        private IISExpress(Website website, int? portNumber, string iisExpressPath)
        {
            _website = website;
            _portNumber = portNumber;
            _iisExpressPath = iisExpressPath;
        }
        
        public void Start()
        {
            var args = new Dictionary<string, string>()
                           {
                               {"path", string.Format("\"{0}\"", _website.Location)},
                               {"trace", "info"}
                           };

            if (_portNumber.HasValue)
                args["port"] = _portNumber.Value.ToString(CultureInfo.InvariantCulture);

            var arguments = string.Join(" ", args.Select(x => string.Format("/{0}:{1}", x.Key, x.Value)));

            Console.WriteLine("{0} {1}", _iisExpressPath, arguments);

            var si = new ProcessStartInfo(_iisExpressPath, arguments)
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _iisProcess = new Process() { StartInfo = si };
            _iisProcess.Start();
            WaitForStartup(_iisProcess);
        }

        public void Stop()
        {
            if (_iisProcess == null || _iisProcess.HasExited) return;

            _iisProcess.Kill();

            _iisProcess.WaitForExit(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds));
        }

        private void WaitForStartup(Process process)
        {
            const string waitForText = "IIS Express is running.";
            var output = new StringBuilder();
            var outputBuffer = new char[1000];
            var stdout = process.StandardOutput;
            Console.WriteLine("Waiting for IIS Express to start up.");

            var hasStarted = new Func<bool>(() => output.ToString().Contains(waitForText));

            while (!hasStarted() && !process.HasExited)
            {
                var outputLength = outputBuffer.Length;
                while (!hasStarted() && !process.HasExited && outputLength == outputBuffer.Length)
                {
                    outputLength = stdout.Read(outputBuffer, 0, outputBuffer.Length);
                    output.Append(outputBuffer, 0, outputLength);
                    Console.Write(outputBuffer, 0, outputLength);
                }
            }

            ParseUrl(output.ToString());

        }

        private void ParseUrl(string output)
        {
            const string pattern = "Successfully registered URL \"(?<url>[^\"]+)\"";
            var regex = new Regex(pattern);
            var match = regex.Match(output);

            if (!match.Success || !match.Groups["url"].Success)
                throw new ApplicationException("Unable to parse site url from IIS express startup text. Something went wrong.");

            _url = new Uri(match.Groups["url"].Value);
        }


        public void Dispose()
        {
            Stop();
        }

        public void Reset()
        {
            if (Settings.RecycleAppPoolBetweenTests)
                _website.RecycleAppPool();
        }


        
    }
}
