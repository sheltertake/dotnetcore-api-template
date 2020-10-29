using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using FriendsApi.Host;
using FriendsApi.Host.Constants;
using FriendsApi.SelfHostedTests.Helpers;

namespace FriendsApi.SelfHostedTests.Tests
{
    [ExcludeFromCodeCoverage]
    public class HealthCheckControllerTests
    {
        [Test]
        public async Task HealthCheckShouldBeOkTest()
        {
            await TestHelper.EnsureSuccessStatusCodeAsync(Routes.HealthCheck);
        }

    }
}