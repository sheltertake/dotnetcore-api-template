using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FriendsApi.NSwagClient.Proxies;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace FriendsApi.SpecFlowTests.Bindings
{
    [ExcludeFromCodeCoverage]
    [Binding]
    public class HealthCheckBindings
    {
        private readonly GlobalContext _globalContext;

        public HealthCheckBindings(GlobalContext globalContext)
        {
            _globalContext = globalContext;
        }

        [When(@"I Send a request to the health check operation")]
        public async Task WhenISendARequestToTheHealthCheckOperation()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _globalContext.Response = await client.HealthcheckAsync();
        }

        [Then(@"I receive a success http code")]
        public void ThenIReceiveASuccessHttpCode()
        {
            _globalContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

    }
}
