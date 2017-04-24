using System.Web.Http.Filters;
using Meetup.Bot.Filters;

namespace Meetup.Bot
{
    public static class FilterConfig
    {
        public static void RegisterFilters(HttpFilterCollection filters)
        {
            filters.Add(new GlobalExceptionFilterAttribute());
        }
    }
}