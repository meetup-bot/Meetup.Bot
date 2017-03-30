using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Meetup.Bot.Startup))]

namespace Meetup.Bot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
