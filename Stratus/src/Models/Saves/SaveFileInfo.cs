using Stratus.Extensions;
using Stratus.IO;

using System.Collections.Generic;
using System;

namespace Stratus.Models.Saves
{
	/// <summary>
	/// File information about a save
	/// </summary>
	public class SaveFileInfo
	{
		public string path { get; private set; }
		public string name { get; private set; }
		public string directoryPath { get; private set; }

		public bool valid => path.IsValid();

		public SaveFileInfo(string filePath)
		{
			this.path = filePath;
			this.name = FileUtility.GetFileName(filePath);
		}

		public bool Delete()
		{
			if (directoryPath.IsValid())
			{
				return FileUtility.DeleteDirectory(directoryPath);
			}
			return FileUtility.DeleteFile(path);
		}

		public override string ToString()
		{
			return path;
		}
	}

	public class StratusSaveFileQuery : StratusAssetQuery<SaveFileInfo>
	{
		public StratusSaveFileQuery(Func<IList<SaveFileInfo>> getAssetsFunction, Func<SaveFileInfo, string> keyFunction) : base(getAssetsFunction, keyFunction)
		{
		}
	}
}