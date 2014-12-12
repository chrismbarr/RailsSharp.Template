using System;
using System.Diagnostics.Contracts;
using System.Security.Principal;

namespace RailsSharp.Backend
{
	public class CustomPrinciple : GenericPrincipal, ICustomPrinciple
	{
		public CustomPrinciple(string username, string[] roles, Guid customerId)
			: base(new GenericIdentity(username), roles)
		{
			Contract.Requires(username != null);
			Contract.Requires(roles != null);
			this.CustomerId = customerId;
		}

		public bool IsInRole(RoleEnum role)
		{
			return this.IsInRole(role.ToString());
		}

		public Guid CustomerId { get; set; }
	}
}