using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace RailsSharp.Backend.Users
{
	public class FakeUserRepoAndServiceStub : IUserCreationService, IUserRepository
	{
		private readonly Guid _customerId;
		private readonly ConcurrentBag<User> _users;

		public FakeUserRepoAndServiceStub()
		{
			_customerId = Guid.NewGuid();
			this._users = new ConcurrentBag<User>
				{
					new User{Email = "user@abc.com", Roles = new []{RoleEnum.User}, CustomerId = this._customerId}
				};
		}

		public void Create(string email, string password)
		{
			this._users.Add(new User { Email = email, Roles = new[] { RoleEnum.User }, CustomerId = this._customerId });
		}

		public bool Authenticate(string email, string password)
		{
			return password == "pa55word" && this._users.Any(x => x.Email == email);
		}

		public User Find(string email)
		{
			return this._users.First(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
		}

		public IEnumerable<User> List(Guid customerId)
		{
			foreach (var user in _users)
			{
				user.CustomerId = customerId; //temp to get around guid generation
			}
			return this._users.Where(x => x.CustomerId == customerId);
		}
	}
}
