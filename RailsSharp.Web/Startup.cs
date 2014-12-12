using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RailsSharp.Web.Startup))]
namespace RailsSharp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			app.MapSignalR();
        }
    }
}
