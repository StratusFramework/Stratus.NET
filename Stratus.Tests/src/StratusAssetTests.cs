using NUnit.Framework;

using Stratus.Editor.Tests;
using Stratus.IO;

using System.Collections.Generic;

namespace Stratus.Tests.Editor
{
	public partial class StratusAssetTests : StratusTest
	{
		public class MockAsset
		{
			public string name;
			public string value;

			public MockAsset(string name, string value)
			{
				this.name = name;
				this.value = value;
			}
		}

		public class MockAssetReference : StratusAssetReference<MockAsset>
		{
		}

		public class MockAssetSource : CustomStratusAssetSource<MockAsset>
		{
			internal static MockAsset a = new MockAsset("a", "foo");
			internal static MockAsset b = new MockAsset("b", "bar");

			protected override IEnumerable<MockAsset> Generate()
			{
				yield return a;
				yield return b;
			}

			protected override string Name(MockAsset asset) => asset.name;
		}

		private class MockObject
		{
			public string name;
			[StratusAssetSource(sourceTypes = typeof(MockAssetSource))]
			public MockAssetReference reference = new MockAssetReference();
		}

		[Test]
		public void AddsAssetToFile()
		{
			MockAsset a = new MockAsset("a", "foobar");

			StratusAssetCollection<MockAsset> collection = new StratusAssetCollection<MockAsset>();
			collection.Add(a);

			AssetFile<MockAsset> file = new AssetFile<MockAsset>();
			file.AtTemporaryPath().WithJson();
			file.Serialize(a);

			var deserialization = file.Deserialize();
			Assert.True(deserialization);
			MockAsset aDeserialized = deserialization.result;
			AssertEqualFields(a, aDeserialized);
		}

		[Test]
		public void GetsAssetsFromSources()
		{
			MockObject obj = new MockObject();
			obj.reference.Set("a");

			var token = obj.reference.token;
			Assert.NotNull(token, $"Token from {obj.reference} was null");
			Assert.AreEqual(MockAssetSource.a, token.asset);
		}
	}
}