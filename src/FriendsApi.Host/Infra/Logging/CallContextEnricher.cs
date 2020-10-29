using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace FriendsApi.Host.Infra.Logging
{
    public class CallContextEnricher : ILogEventEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CallContextEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_httpContextAccessor.HttpContext != null)
            {

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("QueryString", _httpContextAccessor.HttpContext.Request.QueryString));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SeverityLevel", logEvent.Level));
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ServiceOperationName", logEvent.Properties["RequestPath"].ToString().Trim('"')));

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("HostName", _httpContextAccessor.HttpContext.Request.Host));
                if (logEvent.Properties.ContainsKey("SourceContext"))
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Logger", logEvent.Properties["SourceContext"].ToString().Trim('"')));
            }
        }
    }
}