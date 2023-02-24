
using FluentValidation.WebApi;
using Serilog;
using System;
using System.Web.Hosting;
using System.Web.Http;

namespace Cms.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //Fluent Validation
            FluentValidationModelValidatorProvider.Configure(GlobalConfiguration.Configuration );

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            string filePath = HostingEnvironment.ApplicationPhysicalPath + "Logs/logs.txt";

            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .Enrich.FromLogContext()
                 .WriteTo.File(filePath)
                 .CreateLogger();

            try
            {
                Log.Information("Starting Host.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }

        }

    }
}
