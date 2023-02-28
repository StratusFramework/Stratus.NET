using NUnit.Framework;

using Stratus.Data;

namespace Stratus.Tests
{
	public class KeyVariantPairTests 
	{
		[TestCase(7, VariantType.Integer)]
		[TestCase("foobar", VariantType.String)]
		[TestCase(24.7f, VariantType.Float)]
		[TestCase(true, VariantType.Boolean)]
		public void CreatesKeyVariantPair(object value, VariantType expected)
		{
			const string key = "foo";
			var pair = new KeyVariantPair<string>(key);
			pair.Set(value);
			Assert.That(pair.type, Is.EqualTo(expected));
			Assert.That(pair.Get(), Is.EqualTo(value));
		}	
	}
}

