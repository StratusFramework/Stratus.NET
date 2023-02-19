using NUnit.Framework;

using Stratus.Models;
using Stratus.Models.Graph;

namespace Stratus.Tests.Editor
{
	public class StratusSerializedTreeTests
	{
		private class MockElement : TreeElement
		{
			internal int number;
			internal float factor;
		}

		private class MockData : IStratusNamed
		{
			internal string name;
			internal int i;
			internal bool b;
			internal string s;

			string IStratusNamed.name => name;
		}

		private class MockDataElement : TreeElement<MockData>
		{
			public MockDataElement()
			{
			}

			public MockDataElement(MockData data) : base(data)
			{
			}
		}

		[Test]
		public void NewTreeHasRoot()
		{
			var tree = new StratusSerializedTree<MockElement>();
			Assert.That(tree.Count == 0);
			Assert.That(tree.maxDepth == 0);
			Assert.NotNull(tree.root);
			Assert.AreEqual(-1, tree.root.depth);
		}

		[Test]
		public void TreeAddsElementAtDefaultDepth()
		{
			var tree = new StratusSerializedTree<MockElement>();
			MockElement a = new MockElement()
			{
				name = "a",
				number = 7
			};
			tree.AddElement(a);
			Assert.AreEqual(a, tree.elements[1]);
			Assert.AreEqual(a.number, tree.elements[1].number);
			Assert.That(tree.Count == 1);
		}

		[Test]
		public void TreeAddsDataAtDefaultDepth()
		{
			var tree = new StratusSerializedTree<MockDataElement>();
			MockData data = new MockData()
			{
				name = "foo",
				i = 7
			};
			MockDataElement a = new MockDataElement(data);
			Assert.AreEqual(data.name, a.name);
			tree.AddElement(a);
			Assert.AreEqual(a, tree.elements[1]);
			Assert.NotNull(tree.elements[1].data);
			Assert.That(tree.Count == 1);
		}
	}

}