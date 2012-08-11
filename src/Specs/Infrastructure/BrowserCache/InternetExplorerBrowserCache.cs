using System;
using System.Diagnostics;

namespace Specs.Infrastructure.BrowserCache
{
    public class InternetExplorerBrowserCache : IBrowserCache
    {
        public void Clear()
        {
            const string exe = "RunDll32.exe";
            const string args = "InetCpl.cpl,ClearMyTracksByProcess 8";
            var si = new ProcessStartInfo(exe, args)
                         {
                             UseShellExecute = false,
                             CreateNoWindow = true
                         };
            var process = Process.Start(si);
            process.WaitForExit();
        }
    }
}
