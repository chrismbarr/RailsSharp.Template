using System;
using LAN.Core.Eventing;

namespace RailsSharp.Backend
{
	public static class IMessageContextExtensions
	{
		//rails# 4a) Arbitrary grouping through extension methods that target a group
		public static void PublishToAllCustomerUsers(this IMessagingContext context, EventName eventName, ResponseBase respone, Guid customerId)
		{
			var groupName = "Customer:" + customerId;
			context.PublishToGroup(groupName, eventName, respone);
		}
	}
}