using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FriendsApi.Host.Infra.Logging
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;


        //private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        //{
        //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //    Formatting = Formatting.Indented
        //};


        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var exceptionJson = JsonSerializer.Serialize(exception);
            var message = $"{AllMessages(exception)} - {exceptionJson}";

            _logger.LogError(exception, message);

            var ret = new
            {
                StatusDescription = "Api generic error",
                ErrorCode = "ApiGenericError",
                Message = AllMessages(exception),
                Exception = _env.IsDevelopment() ? exception : null
            };

            var result = JsonSerializer.Serialize(ret);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }

        private string AllMessages(Exception ex)
        {
            var separator = "";
            var sb = new StringBuilder();
            while (ex != null)
            {
                sb.Append(separator + ex.Message);
                separator = " -> ";
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

    }
}
