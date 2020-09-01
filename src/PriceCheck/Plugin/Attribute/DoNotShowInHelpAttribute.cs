using System;

namespace PriceCheck
{
	[AttributeUsage(AttributeTargets.Method)]
	public class DoNotShowInHelpAttribute : Attribute
	{
	}
}