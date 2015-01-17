using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;

[assembly: OwinStartup(typeof(MModalClientSPA.App_Start.Startup))]

namespace MModalClientSPA.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // any connection or hub wireup and configuration should go here
            //var hubConfiguration = new HubConfiguration();
            //hubConfiguration.EnableDetailedErrors = true;
            //app.MapSignalR(hubConfiguration);

            app.Map("/signalr", map =>
                {
                    map.UseCors(CorsOptions.AllowAll);
                    var hubConfiguration = new HubConfiguration
                    {

                    };

                    map.RunSignalR(hubConfiguration);
                });
        }
    }
}