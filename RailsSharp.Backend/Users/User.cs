using System;
using System.Collections.Generic;

namespace RailsSharp.Backend.Users
{
	public class User
	{
		public string Email { get; set; }
		public IEnumerable<RoleEnum> Roles { get; set; }
		public Guid CustomerId { get; set; }
	}
}