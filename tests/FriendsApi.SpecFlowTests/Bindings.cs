using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace FriendsApi.SpecFlowTests
{
   
    [Binding]
    [ExcludeFromCodeCoverage]
    public class BaseSpecFlowBinding
    {
        private readonly GlobalContext _globalContext;
        private readonly HttpClient _client;

        protected BaseSpecFlowBinding(GlobalContext globalContext)
        {
            _globalContext = globalContext;
            _client = globalContext.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(Setup.HttpNamedClients.DefaultApi.ToString());
        }

        [Given(@"an http client")]
        public void Givenanhttpclient()
        {
            _globalContext.HttpClient = _client;
        }

        [Then(@"I receive a success code")]
        public void ThenIReceiveASuccessCode()
        {
            _globalContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }
        [Then(@"I receive a created code")]
        public void ThenIReceiveACreatedCode()
        {
            _globalContext.Response.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }
        
        [Then(@"I receive a nocontent code")]
        public void ThenIReceiveANocontentCode()
        {
            _globalContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

    }
}
