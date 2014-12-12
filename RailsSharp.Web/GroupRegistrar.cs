using System.Collections.Generic;
using System.Threading.Tasks;
using LAN.Core.Eventing.SignalR;
using RailsSharp.Backend.Users;

namespace RailsSharp.Web
{
	internal class GroupRegistrar : ISignalRGroupRegistrar
	{
		private readonly IUserRepository _userRepository;

		public GroupRegistrar(IUserRepository userRepository)
		{
			this._userRepository = userRepository;
		}

		public Task<string[]> GetGroupsForUser(string email)
		{
			return Task.Factory.StartNew(() =>
			{
				//rails# 4b) arbitrary grouping by what ever logic makes sense.  This is where you decide how to group
				var user = this._userRepository.Find(email);
				var groups = new List<string> { "Customer:" + user.CustomerId };
				return groups.ToArray();
			});
		}
	}
}