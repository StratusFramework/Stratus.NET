using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Models;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusTreeModelTests : StratusTest
	{
		[Test]
		public static void TreeModelCanAddElements()
		{
			var root = new StratusTreeElement { name = "Root", depth = -1 };

			List<string> expected = new List<string>();
			expected.Add(root.name);

			var tree = new List<StratusTreeElement>();
			tree.Add(root);

			void validate()
			{
				Assert.AreEqual(expected.Count, tree.Count);
				AssertEquality(expected.ToArray(), tree.ToStringArray(e => e.name));
				for (int i = 0; i < expected.Count; ++i)
				{
					Assert.AreEqual(expected[i], tree[i].name);
				}
			}

			var model = new StratusTreeModel<StratusTreeElement>(tree);
			validate();

			// Root, A
			var a = model.AddElement(new StratusTreeElement { name = "A" }, root, 0);
			expected.Add(a.name);
			validate();

			// Root, B, A
			var b = model.AddElement(new StratusTreeElement { name = $"B" }, root, 0);
			expected.Insert(1, b.name);
			validate();

			// Root, C, B, A
			var c = model.AddElement(new StratusTreeElement { name = $"C" }, root, 0);
			expected.Insert(1, c.name);
			validate();

			// Root, C, B, D, A, 
			var d = model.AddElement(new StratusTreeElement { name = "D" }, root.children[1], 0);
			expected.Insert(3, d.name);
			validate();

			// Assert order is correct
			for (int i = 0; i < expected.Count; ++i)
			{
				Assert.AreEqual(expected[i], tree[i].name);
			}

			// Assert depths are valid
			StratusTreeElement.Assert(tree);
		}

		[Test]
		public static void TreeModelCanRemoveElements()
		{
			var root = new StratusTreeElement { name = "Root", depth = -1 };
			var listOfElements = new List<StratusTreeElement>();
			listOfElements.Add(root);

			var model = new StratusTreeModel<StratusTreeElement>(listOfElements);
			model.AddElement(new StratusTreeElement { name = "Element" }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new StratusTreeElement { name = "Sub Element" }, root.children[1], 0);

			model.RemoveElements(new[] { root.children[1].children[0], root.children[1] });

			// Assert order is correct
			string[] namesInCorrectOrder = { "Root", "Element 2", "Element" };
			Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
			for (int i = 0; i < namesInCorrectOrder.Length; ++i)
			{
				Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].name);
			}

			// Assert depths are valid
			StratusTreeElement.Assert(listOfElements);
		}
	}
}