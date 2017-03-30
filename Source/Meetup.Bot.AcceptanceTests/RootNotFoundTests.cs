using System;
using System.Net;
using NUnit.Framework;
using RestSharp;

namespace Meetup.Bot.AcceptanceTests
{
    [TestFixture]
    public class RootNotFoundTests
    {
        private IRestResponse<bool> _response;
        private const string BaseUrl = "http://localhost:65240/";

        [OneTimeSetUp]
        public void Given()
        {
            var restRequest = new RestRequest(Method.GET);
            var restClient = new RestClient
            {
                BaseUrl = new Uri(BaseUrl)
            };

            _response = restClient.Execute<bool>(restRequest);
        }

        [Test]
        public void ThenTheResponseStatusCodeIsNotFound()
        {
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
