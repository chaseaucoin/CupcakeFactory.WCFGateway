using Microsoft.Owin.Hosting;
using System;
using Topshelf;

namespace CupcakeFactory.WCFGateway.Demo
{
    class Program
    {
        class APIHostWrapper
        {
            private IDisposable app;

            public void Start()
            {
                var port = "8072";
                app = WebApp.Start<Startup>("http://*:" + port);
            }

            public void Stop()
            {
                app.Dispose();
            }
        }

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<APIHostWrapper>(s =>                        //2
                {
                    s.ConstructUsing(name => new APIHostWrapper());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());

                });

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetStartTimeout(TimeSpan.FromSeconds(120));

                x.SetDescription("CupcakeFactory.WCFGateway.Demo");
                x.SetDisplayName("CupcakeFactory.WCFGateway.Demo");
                x.SetServiceName("CupcakeFactory.WCFGateway.Demo");
            });
        }
    }
}
