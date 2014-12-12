using LAN.Core.Eventing;

namespace RailsSharp.Backend.Users.List
{
	public class UserListResponse : ResponseBase
	{
		public UserListResponse(UserListRequest request)
			: base(request)
		{
		}

		public User[] Users { get; set; }
	}
}