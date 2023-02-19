using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using Stratus.Utilities;

namespace Stratus.Editor.Tests
{
	public class StratusMathTest
	{
		[TestCase(1, 1)]
		[TestCase(2, 2)]
		[TestCase(3, 6)]
		[TestCase(4, 24)]
		public void Factorial(int n, int expected)
		{
			Assert.AreEqual(expected, MathUtility.Factorial(n));
		}

		[TestCase(3, 2, true, 9)]
		public void Permutations(int n, int r, bool repeating, int expected)
		{
			Assert.AreEqual(expected, MathUtility.Permutations(n, r, repeating));
		}
	}
}