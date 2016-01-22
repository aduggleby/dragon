using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace Dragon.Diagnostics.Modules
{
    /// <summary>
    /// See <a href="http://automationoverflow.blogspot.co.at/2013/08/find-installed-browser-version-from.html">Adivishnu Guduru</a>.
    /// </summary>
    public class BrowserModule : DiagnosticsModuleBase<BrowserOptions>
    {
        protected override void ExecuteImpl(BrowserOptions options)
        {
            var browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet") ??
                                      Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            if (browserKeys == null)
            {
                DebugMessage("No browser has been detected.");
                return;
            }
            var browserNames = browserKeys.GetSubKeyNames();
            var versionLoggers = GetVersionLoggers();
            foreach (var browserName in browserNames)
            {
                DebugMessage(browserName);
                if (versionLoggers.ContainsKey(browserName))
                {
                    versionLoggers[browserName](new string(' ', 4));
                }
            }
        }

        private Dictionary<string, Action<string>> GetVersionLoggers()
        {
            return new Dictionary<string, Action<string>>
            {
                {"FIREFOX.EXE", LogFirefoxVersion},
                {"IEXPLORE.EXE", LogInternetExplorerVersion},
                {"CHROME.EXE", LogChromeVersion},
                {"Google Chrome", LogChromeVersion}
            };
        }

        private void LogFirefoxVersion(string indent)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"Software\" + GetWowNode() + @"Mozilla\Mozilla Firefox");
            if (key == null)
            {
                return;
            }
            DebugMessage(indent + key.GetValue("CurrentVersion"));
        }

        private void LogInternetExplorerVersion(string indent)
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer");
            if (key != null)
            {
                DebugMessage(indent + key.GetValue("Version"));
            }
            // Windows 8
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer");
            if (key != null)
            {
                DebugMessage(indent + key.GetValue("svcVersion"));
            }
        }

        private static string GetWowNode()
        {
            return (Environment.Is64BitOperatingSystem) ? @"Wow6432Node\" : string.Empty;
        }

        private void LogChromeVersion(string indent)
        {
            var keyPath = ((Registry.LocalMachine.OpenSubKey(@"Software\" + GetWowNode() + @"Google\Update\Clients") ??
                                    Registry.CurrentUser.OpenSubKey(@"Software\" + GetWowNode() + @"Google\Update\Clients")) ??
                                   Registry.LocalMachine.OpenSubKey(@"Software\Google\Update\Clients")) ??
                                  Registry.CurrentUser.OpenSubKey(@"Software\Google\Update\Clients");

            if (keyPath == null)
            {
                DebugMessage(indent + "unknown version");
                return;
            }
            var subKeys = keyPath.GetSubKeyNames();
            foreach (var subKey in subKeys)
            {
                var key = keyPath.OpenSubKey(subKey);
                if (key == null)
                {
                    continue;
                }
                var value = key.GetValue("name");
                if (value != null && value.ToString().Equals("Google Chrome", StringComparison.InvariantCultureIgnoreCase))
                {
                    DebugMessage(indent + key.GetValue("pv"));
                    break;
                }
            }
        }
    }
}
