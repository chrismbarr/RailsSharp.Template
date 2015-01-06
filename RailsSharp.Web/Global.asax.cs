using System;
using System.Diagnostics;
using System.Security.Authentication;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using LAN.Core.DependencyInjection;
using LAN.Core.DependencyInjection.SimpleInjector;
using LAN.Core.Eventing;
using LAN.Core.Eventing.SignalR;
using RailsSharp.Backend;
using SimpleInjector;

namespace RailsSharp.Web
{
    public class MvcApplication : HttpApplication
    {
	    /// <exception cref="ApplicationException">Will be thrown if the application is missing any of its dependencies after DI wireup. </exception>
	    protected void Application_Start()
        {
			//note: DI: lightweight container abstraction over simple injector.  The abstraction was more to limit my usage than anything else.
			var container = new SimpleInjectorContainer(new Container());
			ContainerRegistry.DefaultContainer = container;

			SignalREventHub.ExceptionOccured += SignalREventHubOnExceptionOccured;

			ControllerBuilder.Current.SetControllerFactory(new DIControllerFactory(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			
			DIConfig.BindAllTypes(container);

	        var handlerRepository = new DIHandlerRepository(container);
			EventHandlerConfig.RegisterAllHandlers(handlerRepository);
			container.RegisterSingleton<IHandlerRepository>(handlerRepository);

			var result = container.AreAllRequiredDependenciesRegistered();
			if (result.IsMissingDependencies) throw new ApplicationException(result.Message);
        }

	    private void SignalREventHubOnExceptionOccured(object sender, SignalRExceptionEventArgs e)
	    {
		    Debugger.Break();
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
