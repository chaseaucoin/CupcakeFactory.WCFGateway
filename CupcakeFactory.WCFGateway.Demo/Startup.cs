using CustomerService.Contracts;
using DiscoverableService.Contracts;
using Microsoft.Owin;
using Owin;
using Swashbuckle.Application;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace CupcakeFactory.WCFGateway.Demo
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Filters.Add(new AuthorizeAttribute());

            config.ConfigureWcfControllers()
                .AddService<ICustomerService>(serviceConfig => serviceConfig.GetHttpProxy("localhost", 9060))
                .AddService<ISampleService>(serviceConfig => serviceConfig.GetHttpProxy("localhost", 9061))
                .Activate();



            var version = "1.0";
            var environment = "Dev";

            config
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion(version, "CupcakeFactory.WCFGateway.Demo - " + environment);
                    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
                    var commentsFile = Path.Combine(baseDirectory, commentsFileName);

                    c.IncludeXmlComments(commentsFile);

                    var xmlModelFiles = new DirectoryInfo(baseDirectory).EnumerateFiles()
                        .Where(file => file.Name.ToUpper().Contains(".XML"))
                        .Where(file => file.Name.ToUpper().Contains("CONTRACTS"));

                    foreach (var file in xmlModelFiles)
                    {
                        c.IncludeXmlComments(file.FullName);
                    }

                })
                .EnableSwaggerUi("{*assetPath}", c =>
                { });

            app.UseWebApi(config);

            app.Run(RedirectToApiDoc);
        }

        public Task RedirectToApiDoc(IOwinContext context)
        {
            context.Response.StatusCode = 302;
            context.Response.Headers.Set("Location", context.Request.Uri.GetLeftPart(UriPartial.Authority) + "/index");

            return Task.FromResult(true);
        }
    }
}
