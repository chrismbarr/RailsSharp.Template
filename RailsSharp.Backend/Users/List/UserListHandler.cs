using System.Linq;
using LAN.Core.Eventing;

namespace RailsSharp.Backend.Users.List
{
	public class UserListHandler : UserRoleHandlerBase<UserListRequest> //rails# 3a) Authorization is per event, letting it be tailored to the exact use case
	{
		private readonly IUserRepository _userRepository;
		private readonly IMessagingContext _messageContext;

		public UserListHandler(IUserRepository userRepository, IMessagingContext messageContext)
		{
			_userRepository = userRepository;
			_messageContext = messageContext;
		}

		//rails# 2) Server side events are handled asyc allowing for orcastration of your backend in any way that produces a the desigerd behavior.
		protected override void Invoke(UserListRequest request, ICustomPrinciple principal)
		{
			var users = _userRepository.List(principal.CustomerId);

			this._messageContext.PublishToClient(new EventName(UserEvents.ListResponse), new UserListResponse(request) { Users = users.ToArray() });
		}
	}
}