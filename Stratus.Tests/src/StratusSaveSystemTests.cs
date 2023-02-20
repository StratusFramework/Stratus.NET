using NUnit.Framework;

using Stratus.IO;
using Stratus.Models.Saves;
using Stratus.Serialization;

using System.IO;

namespace Stratus
{
	public class StratusSaveSystemTests
	{
		internal class MockSaveData
		{
			public int a;
			public string b;
			public bool c;
		}

		internal class MockSave : Save<MockSaveData>
		{
			public MockSave()
			{
			}

			public MockSave(MockSaveData data) : base(data)
			{
			}

			public override void OnAfterDeserialize()
			{
			}

			public override void OnBeforeSerialize()
			{
			}
		}
		
		internal class MockSaveSystem : SaveSystem<MockSave, JsonObjectSerializer>
		{
			public MockSaveSystem(SaveSystemConfiguration configuration) : base(Path.GetTempPath(), configuration)
			{
			}
		}

		private MockSaveData data = new MockSaveData();

		[SetUp]
		public void Setup()
		{
		}

		[SetUp]
		public void TearDown()
		{
		}

		private MockSaveSystem GetDefaultSaveSystem(bool createDirectoryPerSave)
		{
			var namingConvention = new IncrementalFileNamingConvention("SAVE");
			var format = new DefaultSaveFormat(createDirectoryPerSave);
			var configuration = new SaveSystemConfiguration(format, namingConvention);
			configuration.debug = true;
			configuration.folder = "MockData";
			return new MockSaveSystem(configuration);
		}

		[Test]
		public void CreatesSave()
		{
			var saveSystem = GetDefaultSaveSystem(false);

			int a = 7;
			data.a = a;
			var save = new MockSave(data);

			Assert.True(saveSystem.Save(save));
			Assert.True(save.serialized);

			string expectedPath = FileUtility.CombinePath(saveSystem.saveDirectoryPath,
				FileUtility.ChangeExtension(save.file.name, saveSystem.configuration.format.extension));
			Assert.True(FileUtility.FileExists(expectedPath), $"No save file at {expectedPath}");

			var saveAgain = saveSystem.Load(save.file.name);
			Assert.NotNull(saveAgain);
			Assert.False(saveAgain.dataLoaded);
			Assert.True(saveAgain.LoadData());
			Assert.AreEqual(a, saveAgain.data.a);
		}

		// TODO: Move to Unity test?
		//[Test]
		//public void CreatesSaveWitSnapshot()
		//{
		//	var saveSystem = GetDefaultSaveSystem(false);
		//	var save = new MockSave(data);
		//	save.SetSnapshot(Texture2D.whiteTexture);
		//	Assert.True(saveSystem.Save(save));
		//	Assert.True(save.serialized);

		//	string expectedPath = FileUtility.CombinePath(saveSystem.saveDirectoryPath,
		//		FileUtility.GetFileName(save.snapshotFilePath));
		//	Assert.True(FileUtility.FileExists(expectedPath));
		//}

		[Test]
		public void CreatesSubdirectoryPerSave()
		{
			var saveSystem = GetDefaultSaveSystem(true);
			saveSystem.configuration.format = new DefaultSaveFormat(true);

			var save = new MockSave(data);
			Assert.True(saveSystem.Save(save));
			Assert.True(save.serialized);

			string expectedPath = FileUtility.CombinePath(saveSystem.saveDirectoryPath,
				FileUtility.RemoveExtension(save.file.name),
				FileUtility.ChangeExtension(save.file.name, saveSystem.configuration.format.extension));
			Assert.True(FileUtility.FileExists(expectedPath));
		}

		[TestCase("SAV_001.sav", 1)]
		[TestCase("SAV99_001.dat", 1)]
		[TestCase("SAVE45", 45)]
		[TestCase("SAVE45.save", 45)]
		[TestCase("SAVE34_44_45.save", 45)]
		[TestCase("SAVE_3_4_1999_7.save", 7)]
		public void ParsesIndexFromFileName(string fileName, int expected)
		{
			int actual = IncrementalFileNamingConvention.ParseIndex(fileName);
			Assert.AreEqual(expected, actual);
		}

		[TestCase("SAV_001.sav", 2)]
		[TestCase("SAV_10.sav", 11)]
		public void IncrementIndexFromFileName(string fileName, int expected)
		{
			int actual = IncrementalFileNamingConvention.ParseIndex(fileName);
			Assert.AreEqual(expected, actual + 1);
		}
	}

}