using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;

namespace FriendsApi.Host.Infra.Logging
{
    public static class LogExtensions
    {

        public static void LogSetup(this IApplicationBuilder builder, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            var loggerConf = new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration); // mssqlserver serilog : cannot set by conf the column options, so i have to register by code : the mssqlserver serilog must not be present in the list of WriteTo serilog config section 

            Log.Logger = Enrich(loggerConf, httpContextAccessor).CreateLogger();

        }
        public static IServiceCollection AddLoggingRegistrations(this IServiceCollection services, IConfigurationSection appSettings)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }


        private static LoggerConfiguration Enrich(LoggerConfiguration loggerConf, IHttpContextAccessor httpContextAccessor)
        {
            var version = "0.0.0.0";
            var assembly = Assembly.GetEntryAssembly();
            var serviceName = assembly.GetName().Name;

            var tmp = assembly.GetCustomAttributes<AssemblyFileVersionAttribute>().SingleOrDefault();
            if (tmp != null)
                version = tmp.Version;

            return loggerConf
                .Enrich.With(new CallContextEnricher(httpContextAccessor))
                .Enrich.WithMachineName() //Serilog.Enrichers.Environment
                .Enrich.WithEnvironmentUserName() //Serilog.Enrichers.Environment
                .Enrich.WithProcessId() //Serilog.Enrichers.Process
                .Enrich.WithThreadId() //Serilog.Enrichers.Thread
                .Enrich.WithMemoryUsage()
                //.Enrich.With<MyEnricher>()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceVersion", version)
                .Enrich.WithProperty("ServiceName",
                    serviceName.Length < 50
                        ? serviceName
                        : serviceName.Substring(serviceName.Length - 50));
        }
    }
}
