using System;
using System.Web;

namespace Meetup.Bot.Modules.Diagnostics
{
    public static class DiagnosticsErrorSetter
    {
        public static void SetError(Exception exception)
        {
            if (HttpContext.Current == null) return;

            HttpContext.Current.Items["Error"] = exception.Message;
            HttpContext.Current.Items["ErrorStackTrace"] = exception.StackTrace;
        }

        public static void SetError(string exceptionMessage, string stackTrace)
        {
            if (HttpContext.Current == null) return;

            HttpContext.Current.Items["Error"] = exceptionMessage;
            HttpContext.Current.Items["ErrorStackTrace"] = stackTrace;
        }
    }
}