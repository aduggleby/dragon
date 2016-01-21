using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using CommandLine;
using Dragon.Diagnostics.Modules;

namespace Dragon.Diagnostics
{
    public class Program
    {
        private string _log = "";

        public static void Main(string[] args)
        {
            try
            {
                new Program().Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Run(string[] args)
        {
            var options = new Options();
            var parser = new Parser(with => with.IgnoreUnknownArguments = true);
            var modules = GetModuleRunner();
            if (!parser.ParseArguments(args, options))
            {
                DebugMessage("Unable to parse arguments.");
                return;
            }
            DebugMessage("Dragon.Diagnostics " + Assembly.GetExecutingAssembly().GetName().Version);
            foreach (var module in modules)
            {
                try
                {
                    DebugMessage(string.Format("{2}{4}{0} - {1} starting...{4}{3}", GetCurrentTime(), module.Key, new string('=', 50), new string('-', 50), Environment.NewLine));
                    _log += module.Value(args, parser);
                    DebugMessage(string.Format("{2}{3}{0} - {1} done.{3}", GetCurrentTime(), module.Key, new string('-', 50), Environment.NewLine));
                }
                catch (Exception e)
                {
                    DebugMessage(e.Message);
                    DebugMessage(string.Format("{2}{3}{0} - {1} failed.{3}", GetCurrentTime(), module.Key, new string('-', 50), Environment.NewLine));
                }
            }

            var fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, _log);
            Process.Start("notepad.exe", fileName);
            Debug.Write("The information has been written to: " + fileName);
        }

        public static List<DiagnosticsOptionsBase> GetOptions()
        {
            return new List<DiagnosticsOptionsBase>
            {
                new TraceRouteOptions(),
                new BrowserOptions(),
                new HttpOptions(),
                new NetworkInterfaceOptions(),
                new OperatingSystemOptions(),
                new PingOptions(),
                new SslCertificateOptions(),
                new WebSocketOptions()
            };
        }

        public static Dictionary<string, Func<string[], Parser, string>> GetModuleRunner()
        {
            return new Dictionary<string, Func<string[], Parser, string>> {
                {
                    "TraceRoute", (args, parser) => {
                        var options = new TraceRouteOptions();
                        return VerifyArguments(parser, args, new TraceRouteModule(), options).Execute(options);
                    }
                },
                {
                    "NetworkInterface", (args, parser) =>
                    {
                        var options = new NetworkInterfaceOptions();
                        return VerifyArguments(parser, args, new NetworkInterfaceModule(), options).Execute(options);
                    }
                },
                {
                    "Browser", (args, parser) =>
                    {
                        var options = new BrowserOptions();
                        return VerifyArguments(parser, args, new BrowserModule(), options).Execute(options);
                    }
                },
                {
                    "OperatingSystem", (args, parser) =>
                    {
                        var options = new OperatingSystemOptions();
                        return VerifyArguments(parser, args, new OperatingSystemModule(), options).Execute(options);
                    }
                },
                {
                    "Ping", (args, parser) =>
                    {
                        var options = new PingOptions();
                        return VerifyArguments(parser, args, new PingModule(), options).Execute(options);
                    }
                },
                {
                    "SslCertificate", (args, parser) =>
                    {
                        var options = new SslCertificateOptions();
                        return VerifyArguments(parser, args, new SslCertificateModule(), options).Execute(options);
                    }
                },
                {
                    "WebSocket", (args, parser) =>
                    {
                        var options = new WebSocketOptions();
                        return VerifyArguments(parser, args, new WebSocketModule(), options).Execute(options);
                    }
                },
                {
                    "HTTP", (args, parser) =>
                    {
                        var options = new HttpOptions();
                        return VerifyArguments(parser, args, new HttpModule(), options).Execute(options);
                    }
                }
            };
        }

        #region helpers

        private static string GetCurrentTime()
        {
            return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        }

        private void DebugMessage(string message)
        {
            Console.WriteLine(message);
            _log += message + Environment.NewLine;
        }

        private static IDiagnosticModule<TOption> VerifyArguments<TOption>(Parser parser, string[] args, IDiagnosticModule<TOption> module, TOption options) where TOption : DiagnosticsOptionsBase
        {
            if (parser.ParseArguments(args, options))
            {
                return module;
            }
            throw new Exception("Unable to parse arguments.");
        }

        #endregion
    }
}
