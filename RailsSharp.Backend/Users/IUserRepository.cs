using System;
using System.Collections.Generic;

namespace RailsSharp.Backend.Users
{
	public interface IUserRepository
	{
		bool Authenticate(string email, string password);
		User Find(string email);
		IEnumerable<User> List(Guid customerId);
	}
}