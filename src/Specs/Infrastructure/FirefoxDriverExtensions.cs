using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium.Firefox;

namespace Specs.Infrastructure
{
    public static class FirefoxDriverExtensions 
    {

        public static void CreateProfile(this FirefoxProfileManager manager, string profileName)
        {
            var executableType = Type.GetType("OpenQA.Selenium.Firefox.Internal.Executable, WebDriver", true);
            var executable = Activator.CreateInstance(executableType, new object[] { null });
            var property = executableType.GetProperty("ExecutablePath");
            var executablePath = (string) property.GetValue(executable, new object[0]);

            var builder = new Process();
            builder.StartInfo.FileName = executablePath;
            builder.StartInfo.Arguments = "--verbose -CreateProfile " + profileName;
            builder.StartInfo.RedirectStandardError = true;
            builder.StartInfo.UseShellExecute = false;
            builder.StartInfo.EnvironmentVariables.Add("MOZ_NO_REMOTE", "1");
            builder.Start();
            builder.WaitForExit(Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds));

            if (!builder.HasExited)
                builder.Kill();

        }

        public static FirefoxProfile GetCurrentProfile(this FirefoxDriver driver)
        {
            var property = typeof (FirefoxDriver).GetProperty("Profile", BindingFlags.Instance | BindingFlags.NonPublic);
            var profile = (FirefoxProfile) property.GetValue(driver, new object[0]);
            return profile ??
                   GetProfilesInUse().FirstOrDefault() ??
                   new FirefoxProfileManager().GetProfile(WebDriverFactory.FirefoxProfileName);
        }

        public static bool IsRunning(this FirefoxProfile profile)
        {
            var dir = profile.ProfileDirectory;

            if (string.IsNullOrEmpty(dir))
                return false;

            return new[]
                       {
                           "parent.lock",
                           "lock",
                           ".parentlock"
                       }
                .Select(filename => Path.Combine(dir, filename))
                .Any(File.Exists);
        }

        private static IEnumerable<FirefoxProfile> GetProfilesInUse()
        {
            var mgr = new FirefoxProfileManager();
            return mgr.ExistingProfiles
                .Select(mgr.GetProfile)
                .Where(p => p.IsRunning());
        }

    }
}
