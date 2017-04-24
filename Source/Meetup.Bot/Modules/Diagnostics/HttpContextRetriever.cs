using System.Web;

namespace Meetup.Bot.Modules.Diagnostics
{
    public class HttpContextRetriever
    {
        public HttpContextBase GetContextBaseFromHttpApplication(object httpApplicationObject)
        {
            var application = (HttpApplication)httpApplicationObject;
            return new HttpContextWrapper(application.Context);
        }

        public HttpContextBase GetContextBaseFromCurrent()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}