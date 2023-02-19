using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Models.Graph;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
	public class StratusTreeModelTests : StratusTest
	{
		[Test]
		public static void TreeModelCanAddElements()
		{
			var root = new TreeElement { name = "Root", depth = -1 };

			List<string> expected = new List<string>();
			expected.Add(root.name);

			var tree = new List<TreeElement>();
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

			var model = new TreeModel<TreeElement>(tree);
			validate();

			// Root, A
			var a = model.AddElement(new TreeElement { name = "A" }, root, 0);
			expected.Add(a.name);
			validate();

			// Root, B, A
			var b = model.AddElement(new TreeElement { name = $"B" }, root, 0);
			expected.Insert(1, b.name);
			validate();

			// Root, C, B, A
			var c = model.AddElement(new TreeElement { name = $"C" }, root, 0);
			expected.Insert(1, c.name);
			validate();

			// Root, C, B, D, A, 
			var d = model.AddElement(new TreeElement { name = "D" }, root.children[1], 0);
			expected.Insert(3, d.name);
			validate();

			// Assert order is correct
			for (int i = 0; i < expected.Count; ++i)
			{
				Assert.AreEqual(expected[i], tree[i].name);
			}

			// Assert depths are valid
			TreeElement.Assert(tree);
		}

		[Test]
		public static void TreeModelCanRemoveElements()
		{
			var root = new TreeElement { name = "Root", depth = -1 };
			var listOfElements = new List<TreeElement>();
			listOfElements.Add(root);

			var model = new TreeModel<TreeElement>(listOfElements);
			model.AddElement(new TreeElement { name = "Element" }, root, 0);
			model.AddElement(new TreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new TreeElement { name = "Element " + root.childrenCount }, root, 0);
			model.AddElement(new TreeElement { name = "Sub Element" }, root.children[1], 0);

			model.RemoveElements(new[] { root.children[1].children[0], root.children[1] });

			// Assert order is correct
			string[] namesInCorrectOrder = { "Root", "Element 2", "Element" };
			Assert.AreEqual(namesInCorrectOrder.Length, listOfElements.Count, "Result count does not match");
			for (int i = 0; i < namesInCorrectOrder.Length; ++i)
			{
				Assert.AreEqual(namesInCorrectOrder[i], listOfElements[i].name);
			}

			// Assert depths are valid
			TreeElement.Assert(listOfElements);
		}
	}
}