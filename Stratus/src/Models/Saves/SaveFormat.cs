using Stratus.IO;

using System.Collections.Generic;
using System.IO;

namespace Stratus.Models.Saves
{
	public abstract class SaveFormat
	{
		/// <summary>
		/// The extension used by main save file (of type <see cref="Save"/>)
		/// </summary>
		public string extension { get; set; }

		public abstract string ComposeSavePath(string path, string fileName);
		public abstract IEnumerable<SaveFileInfo> GetSaveFiles(string path);


		protected SaveFormat(string extension)
		{
			this.extension = extension;
		}

		/// <summary>
		/// Generates a save name for this save. By default it will be the time stamp
		/// </summary>
		/// <returns></returns>
		protected virtual string GenerateSaveName()
		{
			return FileUtility.GetTimestamp();
		}

		/// <summary>
		/// Saves the data to the default path in the application's persistent path
		/// using the specified name
		/// </summary>
		public string GenerateSaveFilePath(string path, string fileName)
		{
			string filePath = ComposeSavePath(path, fileName);
			return filePath;
		}
	}

	/// <summary>
	/// Save files are stored at uncompressed at given path with a given extension
	/// </summary>
	public class DefaultSaveFormat : SaveFormat
	{
		/// <summary>
		/// Whether to create subdirectories for each save
		/// </summary>
		public bool createDirectoryPerSave { get; set; }

		public DefaultSaveFormat(bool createDirectoryPerSave = false, string extension = Save.defaultExtension)
			: base(extension)
		{
			this.createDirectoryPerSave = createDirectoryPerSave;
		}

		public override string ComposeSavePath(string path, string fileName)
		{
			if (createDirectoryPerSave)
			{
				string subDirectory = FileUtility.RemoveExtension(fileName);
				return FileUtility.CombinePath(path, subDirectory, FileUtility.ChangeExtension(fileName, extension));
			}

			return FileUtility.CombinePath(path, FileUtility.ChangeExtension(fileName, extension));
		}

		public override IEnumerable<SaveFileInfo> GetSaveFiles(string path)
		{
			IEnumerable<SaveFileInfo> get(string directoryPath)
			{
				var directory = new DirectoryInfo(directoryPath);
				if (directory.Exists)
				{
					var files = Directory.GetFiles(directoryPath, $"*{extension}", SearchOption.TopDirectoryOnly);
					for (int i = 0; i < files.Length; i++)
					{
						string file = files[i];
						yield return new SaveFileInfo(file);
					}
				}
			}

			if (createDirectoryPerSave)
			{
				var directories = Directory.GetDirectories(path);
				foreach (var directoryPath in directories)
				{
					foreach (var file in get(directoryPath))
					{
						yield return file;
					}
				}
			}
			else
			{
				foreach (var file in get(path))
				{
					yield return file;
				}
			}

		}
	}
}