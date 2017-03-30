using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Meetup.Bot.AcceptanceTests
{
    public class IISHelper
    {
        private IISExpress _restWebServer;
        private readonly string _sitePath;
        private readonly string _appHostConfigPath;
        private readonly string _siteName;

        public IISHelper(string siteName, string localPublishedWebsitesPath, string appHostConfigPath)
        {
            _sitePath = localPublishedWebsitesPath;
            _appHostConfigPath = appHostConfigPath;
            _siteName = siteName;
        }

        public void Start()
        {
            _restWebServer = StartWebServer(_siteName);
        }

        public void Stop()
        {
            _restWebServer?.Dispose();
            _restWebServer = null;
        }

        private IISExpress StartWebServer(string siteName)
        {
            var doc = XDocument.Load(_appHostConfigPath);
            var sites = doc.Descendants("sites").First();
            var physicalPath = sites
                .Descendants("site")
                .First(e => e.Attribute("name").Value == siteName)
                .Descendants("virtualDirectory")
                .First()
                .Attributes("physicalPath")
                .First();

            physicalPath.Value = _sitePath;
            var defaults = sites.Descendants("siteDefaults").First();
            var logFileDirectory = defaults
                .Descendants("logFile")
                .First()
                .Attributes("directory")
                .First();
            var logRoot = Path.Combine(_sitePath, "logs");
            logFileDirectory.Value = logRoot;
            var traceRoot = defaults
                .Descendants("traceFailedRequestsLogging")
                .First()
                .Attributes("directory")
                .First();
            traceRoot.Value = Path.Combine(logRoot, "TraceLogFiles");
            doc.Save(_appHostConfigPath, SaveOptions.None);

            return new IISExpress(new Parameters
            {
                Config = _appHostConfigPath,
                Site = siteName,
                Systray = true
            });
        }
    }

    internal class Parameters
    {
        public int Port { get; set; }

        public string Config { get; set; }

        public TraceLevel Trace { get; set; }

        public string Site { get; set; }

        public string Path { get; set; }

        public string SiteId { get; set; }

        public bool Systray { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (Port != 0)
                stringBuilder.Append(" /port:" + Port.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrEmpty(Config))
                stringBuilder.Append(" /config:" + Config);
            if (Trace != TraceLevel.None)
                stringBuilder.Append(" /trace:" + Trace.ToString().ToLower());
            if (!string.IsNullOrEmpty(Site))
                stringBuilder.Append(" /site:" + Site);
            if (!string.IsNullOrEmpty(Path))
                stringBuilder.Append(" /path:" + Path);
            if (!string.IsNullOrEmpty(SiteId))
                stringBuilder.Append(" /siteid:" + SiteId);
            if (!Systray)
                stringBuilder.Append(" /systray:false");
            return stringBuilder.ToString();
        }
    }

    internal enum TraceLevel
    {
        None,
        Info,
        Warning,
        Error,
    }

    internal class IISExpress : IDisposable
    {
        private const string IisexpressExe = "c:\\Program Files\\IIS Express\\iisexpress.exe";
        private readonly ProcessEnvelope _processEnvelope;

        public IISExpress(Parameters parameters)
        {
            if (!File.Exists(IisexpressExe))
                throw new InvalidOperationException("IIS Express not found on filesystem");

            _processEnvelope = new ProcessEnvelope(Process.Start(new ProcessStartInfo
            {
                FileName = IisexpressExe,
                Arguments = parameters?.ToString() ?? ""
            }));
        }

        public void Dispose()
        {
            _processEnvelope.Dispose();
        }

        private class ProcessEnvelope
        {
            private readonly Process _process;

            public ProcessEnvelope(Process process)
            {
                _process = process;
            }

            public void Dispose()
            {
                try
                {
                    NativeMethods.PostMessage(GetHandleRef(), 0x12, IntPtr.Zero, IntPtr.Zero);
                    var waitCount = 0;
                    while (!_process.HasExited)
                    {
                        if (waitCount < 5)
                        {
                            Thread.Sleep(1000);
                            ++waitCount;
                            continue;
                        }
                        Console.WriteLine(@"IIS Express did not shutdown cleanly in good time, going aggressive....");
                        _process.Kill();
                        break;
                    }
                    _process.Dispose();
                }
                catch (NullReferenceException) { }
            }

            private HandleRef GetHandleRef()
            {
                var ptr = NativeMethods.GetTopWindow(IntPtr.Zero);

                while (ptr != IntPtr.Zero)
                {
                    uint num;
                    NativeMethods.GetWindowThreadProcessId(ptr, out num);
                    if (_process.Id == num)
                        return new HandleRef(null, ptr);

                    ptr = NativeMethods.GetWindow(ptr, 2);
                }

                throw new NullReferenceException("Process window not found.");
            }

            private static class NativeMethods
            {
                [DllImport("user32.dll", SetLastError = true)]
                internal static extern IntPtr GetTopWindow(IntPtr hWnd);

                [DllImport("user32.dll", SetLastError = true)]
                internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

                [DllImport("user32.dll", SetLastError = true)]
                internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

                [DllImport("user32.dll", SetLastError = true)]
                internal static extern bool PostMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);
            }
        }
    }
}