using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using FriendsApi.NSwagClient.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;

namespace FriendsApi.SpecFlowTests
{
    [ExcludeFromCodeCoverage]
    public class GlobalContext
    {
        public ServiceProvider ServiceProvider { get; set; }
        public HttpClient HttpClient { get; set; }
        public SwaggerResponse Response { get; set; }
    }

    [ExcludeFromCodeCoverage]
    [Binding]
    public class SetupScenario
    {
        private readonly Setup _setup;
        private readonly GlobalContext _context;

        public SetupScenario(Setup setup, GlobalContext context)
        {
            _setup = setup;
            _context = context;
        }

        [BeforeScenario]
        public void InitSetup()
        {
            _context.ServiceProvider = _setup.InitServiceProvider();
        }

    }

    [ExcludeFromCodeCoverage]
    public class Setup
    {
        public enum HttpNamedClients
        {
            DefaultApi
        }
        public class AppConfig
        {
            public Uri ApiUrl { get; set; }
        }


        public ServiceProvider InitServiceProvider()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.placeholders.json")
                .AddJsonFile($"appsettings.{environment.ToLowerInvariant()}.json", optional: true)
                .Build();


            var services = new ServiceCollection();
            var appSettings = configuration.GetSection("AppSettings");

            //microsoft.extensions.options.configurationextensions
            services.Configure<AppConfig>(appSettings);

            
            services.AddHttpClient(HttpNamedClients.DefaultApi.ToString(), c =>
            {
                var sp = services.BuildServiceProvider();
                var appOptions = sp.GetService<IOptions<AppConfig>>().Value;
                c.BaseAddress = new Uri(appOptions.ApiUrl.ToString());
            });
            return services.BuildServiceProvider();
        }
    }
}