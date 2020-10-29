using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using FriendsApi.Host.Infra.Logging;
using FriendsApi.Host.Ioc;

namespace FriendsApi.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            IConfigurationSection appSettings = Configuration.GetSection("AppSettings");

            services
                // 0) configuration    
                //.Configure<UnityConfig>(appSettings)
                //.AddSingleton(x => x.GetService<IOptions<UnityConfig>>().Value)

                // 1) logging
                .AddLoggingRegistrations(appSettings)

                //2) DataBase
                .AddDabaseConnection(Configuration.GetConnectionString("db"))

                // 3) swagger 
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "FriendsApi Api", Version = "v1" });

                    c.DescribeAllParametersInCamelCase();
                    
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FriendsApi.Host.xml");
                    c.IncludeXmlComments(filePath);
                    var filePath2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FriendsApi.Model.xml");
                    c.IncludeXmlComments(filePath2);
                })

                // 4) Add api registrations
                .AddApplicationServices()

                // 5) Add Mvc (NB: Last)
                .AddControllers(options =>
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                })
                //.AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<ContactValidator>())
                ;
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            Log.Information("Env:" + env.EnvironmentName);
            //app.UseHttpsRedirection(); ///TODO: https

            // 1) operation id header 
            
            // 2) logging
            app.LogSetup(Configuration, httpContextAccessor);
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseRouting();

            // 3) authentication
            app.UseAuthentication();
            app.UseAuthorization();

            // 4) routing + controllers
            app.UseEndpoints(b => b.MapControllers());

            // 5) swagger
            app.UseSwagger((c) =>
            {
                //c.PreSerializeFilters.Add((document, request) =>
                //{
                //    document.Servers = new List<OpenApiServer> { new OpenApiServer { Url = settings.ThisBaseHost.ToString() } };
                //});
            });

            // 6) swagger UI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "FriendsApi Api");
            });
        }

    }
}
