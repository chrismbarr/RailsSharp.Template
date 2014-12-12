using System;
using System.Diagnostics.Contracts;
using System.Security.Principal;

namespace RailsSharp.Backend
{
	[ContractClass(typeof(CustomPrincipleContract))]
	public interface ICustomPrinciple : IPrincipal
	{
		bool IsInRole(RoleEnum role);
		Guid CustomerId { get; set; }
	}

	[ContractClassFor(typeof(ICustomPrinciple))]
	abstract class CustomPrincipleContract : ICustomPrinciple
	{
		public bool IsInRole(string role)
		{
			Contract.Requires(role != null);
			throw new System.NotImplementedException();
		}

		public IIdentity Identity { get; private set; }
		public bool IsInRole(RoleEnum role)
		{
			throw new System.NotImplementedException();
		}

		public Guid CustomerId { get; set; }
	}
}