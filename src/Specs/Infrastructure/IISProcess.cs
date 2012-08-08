using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Specs.Infrastructure
{

    public class IISProcess : IDisposable
    {

        private readonly string _websiteLocation;
        private readonly int? _portNumber;
        private readonly string _iisExpressPath;
        private Process _iisProcess;
        private Uri _url;

        public Uri Url { get { return _url; } }

        public IISProcess()
            : this(GetWebsitePath(), null, GetIISExpressPath())
        {
        }

        public IISProcess(int portNumber)
            : this(GetWebsitePath(), portNumber, GetIISExpressPath())
        {
        }

        public IISProcess(string websiteLocation)
            : this(websiteLocation, null, GetIISExpressPath())
        {
            _websiteLocation = websiteLocation;
        }

        public IISProcess(string websiteLocation, int portNumber)
            : this(websiteLocation, portNumber, GetIISExpressPath())
        {
        }

        private IISProcess(string websiteLocation, int? portNumber, string iisExpressPath)
        {
            _websiteLocation = websiteLocation;
            _portNumber = portNumber;
            _iisExpressPath = iisExpressPath;
        }

        private static string GetWebsitePath()
        {
            return Settings.WebsitePath;
        }

        private static string GetIISExpressPath()
        {
            return Settings.IISExpressPath;
        }

        public void Start()
        {
            var args = new Dictionary<string, string>()
                           {
                               {"path", string.Format("\"{0}\"", _websiteLocation)}
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

            if (!match.Success)
            {
                Console.WriteLine(output);
                throw new ApplicationException("Unable to parse site url from IIS express startup text. Something went wrong.");
            }

            _url = new Uri(match.Groups["url"].Value);
        }


        public void Dispose()
        {
            Stop();
        }

    }
}
