using NUnit.Framework;

using Stratus.Data;

namespace Stratus.Tests
{
	public class VariantTests
	{
		[TestCase(7, VariantType.Integer)]
		[TestCase("foobar", VariantType.String)]
		[TestCase(24.7f, VariantType.Float)]
		[TestCase(true, VariantType.Boolean)]
		public void SetsVariantForType(object value, VariantType expected)
		{
			var variant = new Variant();
			variant.Set(value);
			Assert.That(variant.type, Is.EqualTo(expected));
			Assert.That(variant.Get(), Is.EqualTo(value));
		}

		[TestCase(7, "foobar")]
		[TestCase(true, "pancakes")]
		public void ChangesVariantType(object first, object second)
		{
			var variant = new Variant();
			variant.Set(first);
			var firstType = variant.type;
			variant.Set(second);
			var secondType = variant.type;
			Assert.That(firstType, !Is.EqualTo(secondType));
		}
	}
}

