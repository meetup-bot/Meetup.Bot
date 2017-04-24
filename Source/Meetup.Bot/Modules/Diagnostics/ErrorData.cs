namespace Meetup.Bot.Modules.Diagnostics
{
    public class ErrorData
    {
        public string Exception { get; private set; }
        public string StackTrace { get; private set; }

        public ErrorData(string exception, string stackTrace)
        {
            Exception = exception;
            StackTrace = stackTrace;
        }
    }
}