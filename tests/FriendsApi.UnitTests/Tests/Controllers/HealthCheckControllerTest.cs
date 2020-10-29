using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using FriendsApi.Host.Controllers;

namespace FriendsApi.UnitTests.Tests.Controllers
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class HealthCheckControllerTest
    {
        [Test]
        public void HealthCheckShouldReturnOkAsync()
        {
            var sut = new HealthCheckController();
            var response = sut.Get();
            response.Should().BeOfType<OkResult>();
        }
    }
}
