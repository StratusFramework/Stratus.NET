using NUnit.Framework;

using Stratus.Data;

namespace Stratus.Tests
{
	public class SymbolTests
	{
		[TestCase(7, VariantType.Integer)]
		[TestCase("foobar", VariantType.String)]
		[TestCase(24.7f, VariantType.Float)]
		[TestCase(true, VariantType.Boolean)]
		public void CreatesReferenceForType(object value, VariantType expected)
		{
			const string key = "foobar";
			var symbol = new Symbol(key);
			symbol.Set(value);
			var reference = symbol.ToReference();
			Assert.That(reference.key, Is.EqualTo(symbol.key));
			Assert.That(reference.type, Is.EqualTo(symbol.type));
		}
	}

	public class SymbolTableTests
	{
		[Test]
		public void AddsSymbolsToTable()
		{
			var first = new Symbol("foo", 7);
			var second = new Symbol("bar", true);

			var table = new SymbolTable();
			table.Add(first);
			table.Add(second);
		}
	}
}

