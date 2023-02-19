using NUnit.Framework;

using Stratus.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Stratus.Editor.Tests
{
	public partial class StratusListExtensionsTests
	{
		[Test]
		public void RemoveNull()
		{
			List<string> values = new List<string>
				{
					null
				};
			Assert.AreEqual(1, values.RemoveNull());
		}

		[Test]
		public void AddRange()
		{
			string a = "12", b = "34", c = "56";
			List<string> values = new List<string>();
			values.AddRange(a, b, c);
			Assert.AreEqual(new string[] { a, b, c }, values.ToArray());
		}

		[Test]
		public void ForEachRemoveInvalid()
		{
			List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
			values.ForEachRemoveInvalid(
				(tdo) => tdo.value += 1,
				(tdo) => tdo.value < 5);
			Assert.True(values.Count == 1);
			Assert.True(values.First().name == "A" && values.First().value == 4);
		}

		[Test]
		public void RemoveInvalid()
		{
			List<TestDataObject> values = new List<TestDataObject>
				{
					new TestDataObject("A", 3),
					new TestDataObject("B", 6)
				};
			values.RemoveInvalid((tdo) => tdo.value < 5);
			Assert.True(values.Count == 1);
			Assert.True(values.First().name == "A");
		}

		[Test]
		public void AddRangeWhere()
		{
			TestDataObject a = new TestDataObject("A", 1);
			TestDataObject b = new TestDataObject("B", 2);

			List<TestDataObject> values = new List<TestDataObject>();
			values.AddRangeWhere((x) => x.value > 1, a, b);
			Assert.AreEqual(1, values.Count);
			Assert.AreEqual(b, values.First());
		}

		[Test]
		public void AddRangeUnique()
		{
			TestDataObject a = new TestDataObject("A", 1);

			List<TestDataObject> values = new List<TestDataObject>
				{
					a
				};
			values.AddRangeUnique(a, a);
			Assert.AreEqual(a, values.First());
			Assert.AreEqual(1, values.Count);
		}

		[TestCase(0, new string[] { null, null } )]
		[TestCase(1, "foo")]
		[TestCase(2, new string[] { null, "foo", null, "bar" })]
		public void AddRangeNotNull(int count, params string[] values)
		{
			List<string> a = new List<string>();
			a.AddRangeNotNull(values);
			Assert.AreEqual(count, a.Count);
		}

		[Test]
		public void AddIfNotNull()
		{
			List<string> values = new List<string>();
			Assert.False(values.AddIfNotNull(null));
			Assert.True(values.AddIfNotNull("foo"));
		}

	}
}