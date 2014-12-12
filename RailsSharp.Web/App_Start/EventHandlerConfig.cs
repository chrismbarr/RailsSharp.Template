using LAN.Core.Eventing;
using RailsSharp.Backend.Users;
using RailsSharp.Backend.Users.List;

namespace RailsSharp.Web
{
	public class EventHandlerConfig
	{
		public static void RegisterAllHandlers(IHandlerRepository handlerRepository)
		{
			//rails# 5) Event Handlers are configured to accept a given event
			handlerRepository.AddHandler<UserListHandler>(UserEvents.ListRequest);
		}
	}
}