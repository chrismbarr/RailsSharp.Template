using System;
using System.Web.Mvc;
using System.Web.Routing;
using LAN.Core.DependancyInjection;

namespace RailsSharp.Web
{
	public class DIControllerFactory : DefaultControllerFactory
	{
		private readonly IContainer _container;

		public DIControllerFactory(IContainer container)
		{
			_container = container;
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			if (controllerType != null)
			{
				return (IController)_container.GetInstance(controllerType);
			}
			return null;
		}
	}
}