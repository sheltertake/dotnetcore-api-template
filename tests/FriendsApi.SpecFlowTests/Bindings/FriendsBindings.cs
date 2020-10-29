using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FriendsApi.NSwagClient.Proxies;
using TechTalk.SpecFlow;

namespace FriendsApi.SpecFlowTests.Bindings
{
    [ExcludeFromCodeCoverage]
    [Binding]
    public class FriendsBindings
    {
        public class FriendsContext
        {
            public IEnumerable<string> Friends { get; set; }
            public string Friend { get; set; }
            public SwaggerResponse<ICollection<Friend>> GetResponse { get; internal set; }
            public SwaggerResponse<Friend> CreateResponse { get; set; }
            public SwaggerResponse<Friend> GetByIdResponse { get; internal set; }
            public SwaggerResponse NoContentResponse { get; internal set; }
            public Friend CreateRequest { get; set; }
            public Friend UpdateRequest { get; internal set; }
            public int DeletedId { get; set; }
            public SwaggerException<ProblemDetails> SwaggerException { get; set; }
        }

        private readonly GlobalContext _globalContext;
        private readonly FriendsContext _context;

        public FriendsBindings(GlobalContext globalContext, FriendsContext context)
        {
            _globalContext = globalContext;
            _context = context;
        }

        [When(@"I send a friends request")]
        public async Task WhenISendAFriendsRequest()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _context.GetResponse = await client.FriendsGetAsync();
        }

        [When(@"I send a create friend request")]
        public async Task WhenISendACreateFriendRequest()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _context.CreateRequest = new Friend()
            {
                Name = "SpecFlowFriend"
            };
            _context.CreateResponse = await client.FriendsPostAsync(_context.CreateRequest);
        }

        [When(@"I send a get by id friend request using the created one")]
        public async Task WhenISendAGetByIdFriendRequestUsingTheCreatedOneAsync()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _context.GetByIdResponse = await client.FriendsGetAsync(_context.CreateResponse.Result.Id);
        }
        [When(@"I send a get by id friend request using the deleted one")]
        public async Task WhenISendAGetByIdFriendRequestUsingTheDeletedOne()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            try
            {
                _context.GetByIdResponse = await client.FriendsGetAsync(_context.DeletedId);
            }
            catch (SwaggerException<ProblemDetails> e)
            {
                _context.SwaggerException = e;
            }
            
        }
        [Then(@"I receive not found response")]
        public void ThenIReceiveNotFoundResponse()
        {
            _context.SwaggerException.Should().NotBeNull();
            _context.SwaggerException.StatusCode.Should().Be(404);
        }
        [When(@"I Send an update friend request using the created one")]
        public async Task WhenISendAnUpdateFriendRequestUsingTheCreatedOneAsync()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _context.UpdateRequest = new Friend()
            {
                Name = _context.CreateResponse.Result.Name + "Updated"
            };
            _context.NoContentResponse = await client.FriendsPutAsync(_context.CreateResponse.Result.Id, _context.UpdateRequest);
        }

        [When(@"I Send a delete friend request using the created one")]
        public async Task WhenISendADeleteFriendRequestUsingTheCreatedOne()
        {
            var client = new FriendsApiClient(_globalContext.HttpClient);
            _context.DeletedId = _context.CreateResponse.Result.Id;
            _context.NoContentResponse = await client.FriendsDeleteAsync(_context.DeletedId);
        }

        [Then(@"I receive a friends response")]
        public void ThenIReceiveAFriendsResponse()
        {
            _context.GetResponse.Should().NotBeNull();
            _context.GetResponse.StatusCode.Should().Be(200);
        }
        [Then(@"I receive a new friend response")]
        public void ThenIReceiveANewFriendResponse()
        {
            _context.CreateResponse.Should().NotBeNull();
            _context.CreateResponse.StatusCode.Should().Be(201);
        }
        [Then(@"I receive a valid friend response")]
        public void ThenIReceiveAValidFriendResponse()
        {
            _context.GetByIdResponse.Should().NotBeNull();
            _context.GetByIdResponse.StatusCode.Should().Be(200);
        }
        [Then(@"I receive a success nocontent code")]
        public void ThenIReceiveASuccessUpdateCode()
        {
            _context.NoContentResponse.Should().NotBeNull();
            _context.NoContentResponse.StatusCode.Should().Be(204);
        }
       
        [Then(@"The response contains friends")]
        public void ThenTheResponseContainsFriends()
        {
            _context.GetResponse.Result.Should().NotBeEmpty();
        }
        [Then(@"the new friend response is valid")]
        public void ThenTheNewFriendResponseIsValid()
        {
            _context.CreateResponse.Result.Id.Should().BeGreaterThan(0);
            _context.CreateResponse.Result.Name.Should().NotBeEmpty();
        }
        [Then(@"The friend is equal to the created one")]
        public void ThenTheFriendIsEqualToTheCreatedOne()
        {
            _context.CreateResponse.Result.Name.Should().Be(_context.CreateRequest.Name);
        }
        [Then(@"The friend is equal to the updated one")]
        public void ThenTheFriendIsEqualToTheUpdatedOne()
        {
            _context.GetByIdResponse.Result.Name.Should().Be(_context.UpdateRequest.Name);
        }



        //        [When(@"I Send a values request")]
        //        public async Task WhenISendAFriendsRequest()
        //        {
        //            var client = new FriendsApiClient(_globalContext.HttpClient);
        //            _globalContext.Response = await client.FriendsGetAsync();
        //        }

        //        [When(@"I Send a value request sending ""(.*)"" as parameter")]
        //        public async Task WhenISendAFriendRequestSendingAsParameter(int id)
        //        {
        //            var client = new FriendsApiClient(_globalContext.HttpClient);
        //            _globalContext.Response = await client.FriendGetAsync(id);
        //        }

        //        [When(@"I Send a create value request sending ""(.*)""")]
        //        public async Task WhenISendACreateFriendRequestSending(string value)
        //        {
        //            var client = new FriendsApiClient(_globalContext.HttpClient);
        //            _globalContext.Response = await client.FriendsPostAsync(value);
        //        }

        //        [When(@"I Send an update value request sending ""(.*)"" as parameter and ""(.*)"" as value")]
        //        public async Task WhenISendAnUpdateFriendRequestSendingAsParameterAndAsFriend(int id, string value)
        //        {
        //            var client = new FriendsApiClient(_globalContext.HttpClient);
        //            _globalContext.Response = await client.FriendPutAsync(id, value);
        //        }

        //        [When(@"I Send a delete value request sending ""(.*)"" as parameter")]
        //        public async Task WhenISendADeleteFriendRequestSendingAsParameter(int id)
        //        {
        //            var client = new FriendsApiClient(_globalContext.HttpClient);
        //            _globalContext.Response = await client.FriendDeleteAsync(id);
        //        }
        //        [Then(@"I receive a values response")]
        //        public void ThenIReceiveAFriendsResponse()
        //        {
        //            var response = (SwaggerResponse<ICollection<string>>)_globalContext.Response;
        //            response.Result.Should().BeOfType<List<string>>();
        //        }
        //        [Then(@"I receive a value response")]
        //        public void ThenIReceiveAFriendResponse()
        //        {
        //            var response = (SwaggerResponse<string>)_globalContext.Response;
        //            response.Result.Should().BeOfType<string>();
        //        }
        //        [Then(@"The value should be ""(.*)""")]
        //        public void ThenTheFriendShouldBe(string value)
        //        {
        //            var response = (SwaggerResponse<string>)_globalContext.Response;
        //            response.Result.Should().BeEquivalentTo(value);
        //        }

        //        [Then(@"I can read the response and deserialize it")]
        //        public void ThenICanReadTheResponseAndDeserializeIt()
        //        {
        //#pragma warning disable S125 // Sections of code should not be commented out
        //            // var response = (SwaggerResponse<string>)_globalContext.Response;
        //            // response.Result.Should().BeEquivalentTo(value);
        //#pragma warning restore S125 // Sections of code should not be commented out
        //        }

    }
}