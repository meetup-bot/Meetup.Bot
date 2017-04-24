using System.Configuration;
using System.Web;
using System.Web.Http;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Meetup.Bot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterFilters(GlobalConfiguration.Configuration.Filters);

            var pathFormat = ConfigurationManager.AppSettings["Serilog.LogPath"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.WithThreadId()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile(new ElasticsearchJsonFormatter(renderMessage: true, inlineFields: true),
                                     pathFormat,
                                     retainedFileCountLimit: 5,
                                     fileSizeLimitBytes: null)
                .Enrich.WithProperty("OriginatesFrom", "Meetup.Bot")
                .CreateLogger();
        }
    }
}
