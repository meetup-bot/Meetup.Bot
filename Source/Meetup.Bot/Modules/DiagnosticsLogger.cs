using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Meetup.Bot.Modules.Diagnostics;
using Serilog;

namespace Meetup.Bot.Modules
{
    public class DiagnosticsLogger : IHttpModule
    {
        private readonly HttpContextRetriever _httpContextRetriever;
        private Stopwatch _stopwatch;
        private readonly ILogger _log = Log.Logger.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public DiagnosticsLogger()
            : this(new HttpContextRetriever())
        {
        }

        internal DiagnosticsLogger(HttpContextRetriever httpContextRetriever)
        {
            _httpContextRetriever = httpContextRetriever;
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;
            context.EndRequest += EndRequest;
        }

        public void BeginRequest(object source, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            _stopwatch = sw;
        }

        public void EndRequest(object source, EventArgs e)
        {
            _stopwatch.Stop();

            var diagnosticData = BuildDiagnosticData(source);
            Task.Factory.StartNew(() => SendDiagnostics(diagnosticData));
        }

        private DiagnosticData BuildDiagnosticData(object source)
        {
            var context = _httpContextRetriever.GetContextBaseFromHttpApplication(source);

            var version = (string)context.Items["RequestVersion"] ?? "unknown";
            if (version == "unknown" && context.Request.Url.Segments.Length > 1)
            {
                version = context.Request.Url.Segments[1].TrimEnd('/').Replace(".", "_");
            }

            var diagnosticData = new DiagnosticData
            {
                Method = context.Request.HttpMethod,
                Endpoint = context.Request.RawUrl,
                Host = Environment.MachineName,
                Port = context.Request.ServerVariables["SERVER_PORT"],
                StatusCode = context.Response.StatusCode,
                Version = (string)version,
                ElapsedMilliseconds = (int)_stopwatch.Elapsed.TotalMilliseconds,
                UserAgent = context.Request.UserAgent,
                RemoteAddress = context.Request.ServerVariables["REMOTE_ADDR"],
                RemotePort = context.Request.ServerVariables["REMOTE_PORT"],
            };

            var errorText = (string)context.Items["Error"];

            if (string.IsNullOrEmpty(errorText)) return diagnosticData;

            var stackTrace = (string)context.Items["ErrorStackTrace"];
            diagnosticData.ErrorData = new ErrorData(errorText, stackTrace);
            context.Items.Remove("Error");
            context.Items.Remove("ErrorStackTrace");

            return diagnosticData;
        }

        private void SendDiagnostics(DiagnosticData diagnosticData)
        {
            var formatting = DiagnosticFormattingFactory.GetFormatting(diagnosticData);
            _log.Information(formatting.MessageTemplate, formatting.PropertyValues);
        }

        public void Dispose()
        {
        }
    }
}