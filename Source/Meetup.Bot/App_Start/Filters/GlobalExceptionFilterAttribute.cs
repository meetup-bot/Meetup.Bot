using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Meetup.Bot.Modules.Diagnostics;

namespace Meetup.Bot.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Response != null) return;

            DiagnosticsErrorSetter.SetError(context.Exception);
            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}