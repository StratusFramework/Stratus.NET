using NUnit.Framework;

using Stratus.Collections;

using System.Collections.Generic;

namespace Stratus.Tests
{
	public class BictionaryTests
	{
		[TestCase("a", 7)]
		public void AddsValue(string first, int second)
		{
			var bictionary = new Bictionary<string, int>
			{
				{ first, second }
			};
			Assert.That(bictionary[first], Is.EqualTo(second));
			Assert.That(bictionary[second], Is.EqualTo(first));
		}

		[Test]
		public void RemovesValue()
		{
			const string first = "a";
			const int second = 7;

			var bictionary = new Bictionary<string, int>();			
			bictionary.Add(first, second);
			
			Assert.That(bictionary.Count, Is.EqualTo(1));
			bictionary.Remove(first);
			Assert.That(bictionary.Count, Is.EqualTo(0));

			bictionary.Add(first, second);

			Assert.That(bictionary.Count, Is.EqualTo(1));
			bictionary.Remove(second);
			Assert.That(bictionary.Count, Is.EqualTo(0));
		}
	}
}

