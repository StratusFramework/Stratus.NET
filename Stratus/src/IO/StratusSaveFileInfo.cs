using Stratus.Extensions;
using Stratus.IO;

//using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// File information about a save
	/// </summary>
	public class StratusSaveFileInfo
	{
		public string path { get; private set; }
		public string name { get; private set; }
		public string directoryPath { get; private set; }

		public bool valid => path.IsValid();

		public StratusSaveFileInfo(string filePath)
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

}