using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Meetup.Bot.Controllers
{
    public class RootController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }
    }
}
