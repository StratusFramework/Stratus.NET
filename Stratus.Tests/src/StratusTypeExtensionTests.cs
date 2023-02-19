using NUnit.Framework;

using Stratus.Extensions;

using System;
using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusTypeExtensionTests
	{
		private class MockDataObject_A
		{
			public string stringPublic;
			private string stringPrivate;
			private static string stringPrivateStatic;
			public static string stringPublicStatic;
			public string stringProperty { get; set; }
		}

		[Test]
		public void GetFieldIncludePrivate()
		{
			Type type = typeof(MockDataObject_A);

			Assert.NotNull(type.GetFieldIncludePrivate(nameof(MockDataObject_A.stringPublic)));
			Assert.NotNull(type.GetFieldIncludePrivate(nameof(MockDataObject_A.stringPublicStatic)));
			Assert.NotNull(type.GetField(nameof(MockDataObject_A.stringPublic)));
			Assert.NotNull(type.GetField(nameof(MockDataObject_A.stringPublicStatic)));

			Assert.NotNull(type.GetFieldIncludePrivate("stringPrivateStatic"));
			Assert.NotNull(type.GetFieldIncludePrivate("stringPrivate"));
			Assert.Null(type.GetField("stringPrivateStatic"));
			Assert.Null(type.GetField("stringPrivate"));

			Assert.Null(type.GetFieldIncludePrivate(nameof(MockDataObject_A.stringProperty)));
			Assert.Null(type.GetFieldIncludePrivate("NOT_REAL"));
		}

		[Test]
		public void IsArrayOrList()
		{
			int[] a = new int[] { };
			List<string> c = new List<string>();
			IList<string> b = c;
			Array d = a;

			Assert.IsTrue(a.GetType().IsArrayOrList());
			Assert.IsTrue(b.GetType().IsArrayOrList());
			Assert.IsTrue(c.GetType().IsArrayOrList());
			Assert.IsTrue(d.GetType().IsArrayOrList());

			int e = 5;
			string f = "aaa";
			Assert.IsFalse(e.GetType().IsArrayOrList());
			Assert.IsFalse(f.GetType().IsArrayOrList());

			Dictionary<int, int> g = new Dictionary<int, int>();
			HashSet<int> h = new HashSet<int>();
			Assert.IsFalse(g.GetType().IsArrayOrList());
			Assert.IsFalse(h.GetType().IsArrayOrList());
		}

		[Test]
		public void GetArrayOrListElementType()
		{
			int[] a = new int[] { };
			List<string> b = new List<string>();
			int c = 5;
			Assert.AreEqual(typeof(int), a.GetType().GetArrayOrListElementType());
			Assert.AreEqual(typeof(string), b.GetType().GetArrayOrListElementType());
			Assert.Null(c.GetType().GetArrayOrListElementType());
		}

		private class MockClassA 
		{ 
			public MockClassA()
			{
			}
		}

		private class MockClassB
		{
			private string value;
			public MockClassB(string s)
			{
				this.value = s;
			}
		}

		[Test]
		public void HasDefaultConstructor()
		{
			Assert.True(typeof(MockClassA).HasDefaultConstructor());
			Assert.False(typeof(MockClassB).HasDefaultConstructor());
		}
	}
}