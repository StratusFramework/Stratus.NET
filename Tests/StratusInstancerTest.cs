using NUnit.Framework;

using System;


namespace Stratus.Editor.Tests
{
	public class StratusInstancerTest
	{
		public abstract class A { }

		public class B : A { }
		public class B1 : B { }
		public abstract class B2 : B { }

		public class C : A { }
		public class D { }

		[TestCase(typeof(A), false)]
		[TestCase(typeof(B), true)]
		[TestCase(typeof(B1), true)]
		[TestCase(typeof(B2), false)]
		[TestCase(typeof(C), true)]
		public void GetsInstanceByType(Type type, bool instanced)
		{
			var instancer = new StratusTypeInstancer<A>();
			Assert.That((instancer.Get(type) != null) == instanced);
		}

		[TestCase(nameof(B))]
		[TestCase(nameof(B1))]
		[TestCase(nameof(C))]
		public void GetsInstanceByName(string name)
		{
			var instancer = new StratusTypeInstancer<A>();
			Assert.NotNull(instancer.Get(name));
		}
	}
}