using NUnit.Framework;

using Stratus.Extensions;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusCollectionExtensionsTests
	{
		[Test]
		public void IsEmpty()
		{
			List<string> values = null;
			values = new List<string>();
			Assert.True(values.IsNullOrEmpty());
			values.Add("Boo");
			Assert.False(values.IsNullOrEmpty());
		}

		[Test]
		public void IsNotEmpty()
		{
			List<string> values = null;
			Assert.False(values.IsValid());
			values = new List<string>();
			values.Add("Boo");
			Assert.True(values.IsValid());
		}

		[Test]
		public void IsNullOrEmpty()
		{
			string[] values = null;
			Assert.True(values.IsNullOrEmpty<string>());
			values = new string[] { };
			Assert.True(values.IsNullOrEmpty<string>());
			values = new string[] { "foo", "bar" };
			Assert.False(values.IsNullOrEmpty<string>());
		}

		[Test]
		public void IsValid()
		{
			string[] values = null;
			Assert.False(values.IsValid<string>());
			values = new string[] { };
			Assert.False(values.IsValid<string>());
			values = new string[] { "foo", "bar" };
			Assert.True(values.IsValid<string>());
		}

		[Test]
		public void PushRange()
		{
			Stack<int> values = new Stack<int>();
			values.PushRange(1, 2, 3);
			Assert.AreEqual(new int[] { 3, 2, 1 }, values.ToArray());
		}

		[Test]
		public void EnqueueRange()
		{
			Queue<int> values = new Queue<int>();
			values.EnqueueRange(1, 2, 3);
			Assert.AreEqual(new int[] { 1, 2, 3 }, values.ToArray());
		}

		[Test]
		public void TryContains()
		{
			IList<string> values;
			string value = "a";
			values = new string[] { value, "b", "c" };
			Assert.True(values.TryContains(value));
			values = new string[] { "b", "c" };
			Assert.False(values.TryContains(value));
			values = null;
			Assert.False(values.TryContains(value));
		}

		[Test]
		public void LenghOrZero()
		{
			string[] values;
			values = null;
			Assert.AreEqual(0, values.LengthOrZero());
			values = new string[] { };
			Assert.AreEqual(0, values.LengthOrZero());
			values = new string[] { "a" };
			Assert.AreEqual(1, values.LengthOrZero());
		}
	}
}