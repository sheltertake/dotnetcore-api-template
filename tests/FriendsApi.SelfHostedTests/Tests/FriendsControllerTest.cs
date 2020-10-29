using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using FriendsApi.Host.Constants;
using FriendsApi.SelfHostedTests.Helpers;
using FriendsApi.Model.Types;
using System;
using System.Text.Json;
using System.Net;

namespace FriendsApi.SelfHostedTests.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class FriendsControllerTest
    {
        private static IHost Host;
        private static HttpClient Client;

        [SetUp]
        public static async Task SetUp()
        {
            Host = await TestHelper.CreateHost();
            Client = TestHelper.CreateClientAsync(Host);
        }

        [TearDown]
        public static void TearDown()
        {
            Client.Dispose();
            Host.Dispose();
        }


        [Test]
        public async Task FriendCrudTest()
        {
            
            var responseList = await Client.GetNotNullAsync<List<Friend>>(Routes.Friends);
            responseList.Should().NotBeNull();
            responseList.Should().HaveCountGreaterThan(0);

            var response = await Client.PostAsync(Routes.Friends, new Friend { Name = "Test" + new Random(100).Next() });
            var content = await response.Content.ReadAsStreamAsync();
            var dto = await JsonSerializer.DeserializeAsync<Friend>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            

            var route = GetRoute(dto.Id);
            await Client.PutAsync(route, new Friend
            {
                Id = dto.Id,
                Name = "Edited" + new Random(100).Next()
            });

            await Client.DeleteAsync(route);

            var last = await Client.GetAsync(route);
            last.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


        private static string GetRoute(int id)
        {
            var route = Routes.Friend.Replace("{id}", id.ToString());
            return route;
        }
    }
}
