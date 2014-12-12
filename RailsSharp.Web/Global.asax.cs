using System;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using LAN.Core.DependancyInjection;
using LAN.Core.DependancyInjection.SimpleInjector;
using LAN.Core.Eventing;
using LAN.Core.Eventing.SignalR;
using RailsSharp.Backend;
using SimpleInjector;

namespace RailsSharp.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
			//note: DI: lightwieght container abstraction over simple injector.  The abstraction was more to limit my usage than anything else.
			var container = new SimpleInjectorContainer(new Container());
			ContainerRegistery.DefaultContainer = container;

			ControllerBuilder.Current.SetControllerFactory(new DIControllerFactory(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			
			DIConfig.BindAllTypes(container);

	        var handlerRepository = new DIHandlerRepository(container);
			EventHandlerConfig.RegisterAllHandlers(handlerRepository);
			container.RegisterSingleton<IHandlerRepository>(handlerRepository);
        }

		protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
		{
			var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie == null) return;

			var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
			if (authTicket == null) throw new AuthenticationException();
			var serializer = new JavaScriptSerializer();
			var serializeModel = serializer.Deserialize<CustomPrincipleSerializedModel>(authTicket.UserData);
			var newUser = new CustomPrinciple(authTicket.Name, serializeModel.Roles, Guid.Parse(serializeModel.CustomerId));
			HttpContext.Current.User = newUser;
		}
    }
}
