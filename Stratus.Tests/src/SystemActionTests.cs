using NUnit.Framework;
using System.Linq;
using Stratus.Extensions;
using System;

namespace Stratus.Tests
{
	public class SystemActionTests
	{
		[Test]
		public void AppendsAction()
		{
			var text = string.Empty;

			var first = "foo";
			var second = "bar";
			var expected = first + second;

			Action a1 = () => text += first;
			Action a2 = () => text += second;
			Action write = a1.Append(a2);
			
			write();
			Assert.That(text, Is.EqualTo(expected));
		}

		[Test]
		public void PrependsAction()
		{
			var text = string.Empty;

			var first = "foo";
			var second = "bar";
			var expected = first + second;

			Action a1 = () => text += first;
			Action a2 = () => text += second;
			Action write = a2.Prepend(a1);

			write();
			Assert.That(text, Is.EqualTo(expected));
		}
	}
}
