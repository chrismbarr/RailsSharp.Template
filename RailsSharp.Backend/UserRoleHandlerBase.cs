using LAN.Core.Eventing;

namespace RailsSharp.Backend
{
	public abstract class UserRoleHandlerBase<TReq> : HandlerBase<TReq, ICustomPrinciple> where TReq : RequestBase
	{
		//rails# 3b) We can however make reusable authentication "contexts" for the handlers
		protected override bool IsAuthorized(TReq request, ICustomPrinciple principal)
		{
			return principal.IsInRole(RoleEnum.User);
		}
	}
}