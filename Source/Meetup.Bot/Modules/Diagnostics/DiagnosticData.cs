namespace Meetup.Bot.Modules.Diagnostics
{
    public class DiagnosticData
    {
        public string Endpoint { get; set; }
        public string Version { get; set; }
        public string Host { set; get; }
        public string Port { get; set; }
        public string Method { get; set; }
        public int StatusCode { get; set; }
        public int ElapsedMilliseconds { get; set; }
        public string UserAgent { get; set; }
        public ErrorData ErrorData { get; set; }
        public string RemoteAddress { get; set; }
        public string RemotePort { get; set; }
    }
}