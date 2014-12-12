using LAN.Core.DependancyInjection;
using LAN.Core.Eventing;
using LAN.Core.Eventing.SignalR;
using RailsSharp.Backend.Users;
using RailsSharp.Web.Controllers;

namespace RailsSharp.Web
{
	public class DIConfig
	{
		public static void BindAllTypes(IContainer container)
		{
			//note: DI: here is where we wireup all types, this keeps the notion of a di container safely away from our business logic.

			var fakeRepo = new FakeUserRepoAndServiceStub();

			container.RegisterSingleton<IUserRepository>(fakeRepo);
			container.RegisterSingleton<IUserCreationService>(fakeRepo);

			container.Bind<ISignalRSerializer, CamelCaseSignalRSerializer>(true);
			container.Bind<IMessagingContext, SignalRMessagingContext>(true);
			container.Bind<ISignalRGroupRegistrar, GroupRegistrar>(true);

			//rails# 4c) Group Services can be injected to handle group membership inflight.
			container.Bind<IGroupJoinService, SignalRGroupService>(true);
			container.Bind<IGroupLeaveService, SignalRGroupService>(true);

			container.Bind<AccountController, AccountController>(false);
			container.Bind<HomeController, HomeController>(false);
		}
	}
}