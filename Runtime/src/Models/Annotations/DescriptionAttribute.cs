using System;
using System.ComponentModel;

namespace Stratus.Models
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ClassDescriptionAttribute : DescriptionAttribute
	{
		public ClassDescriptionAttribute(string description) : base(description)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
	public class MemberDescriptionAttribute : DescriptionAttribute
	{
		public MemberDescriptionAttribute(string description) : base(description)
		{
		}
	}
}