using Stratus.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stratus
{
	public class StratusSaveFileQuery : StratusAssetQuery<StratusSaveFileInfo>
	{
		public StratusSaveFileQuery(Func<IList<StratusSaveFileInfo>> getAssetsFunction, Func<StratusSaveFileInfo, string> keyFunction) : base(getAssetsFunction, keyFunction)
		{
		}
	}
}

namespace Stratus.IO
{
	public abstract class StratusFileNamingConvention
	{
		protected StratusFileNamingConvention(string prefix)
		{
			this.prefix = prefix;
		}

		public string prefix { get; private set; }
		public const char prefixSeparator = '_';
		public abstract string GenerateFileName(StratusSaveFileQuery files);
	}

	public class StratusIncrementalFileNamingConvention : StratusFileNamingConvention
	{
		public StratusIncrementalFileNamingConvention(string prefix) : base(prefix)
		{
		}

		private StratusSortedList<int, StratusSaveFileInfo> filesByIndex { get; set; }

		public const string indexPattern = @"(?<index>\d+)";
		public const string indexCaptureGroupName = "index";

		public override string GenerateFileName(StratusSaveFileQuery files)
		{
			if (filesByIndex == null)
			{
				filesByIndex = new StratusSortedList<int, StratusSaveFileInfo>(x => ParseIndex(x.name));
				filesByIndex.AddRange(files.assets);
			}
			else if (!files.updated)
			{
				filesByIndex.Clear();
				filesByIndex.AddRange(files.assets);
			}

			int index = 0;
			if (filesByIndex.IsValid())
			{
				var lastIndex = filesByIndex.Last().Key;
				index = lastIndex + 1;
			}
			return $"{prefix}{prefixSeparator}{index}";
		}

		public static int ParseIndex(string fileName)
		{
			MatchCollection matches = Regex.Matches(fileName, indexPattern);
			if (matches.Count > 0)
			{
				return int.Parse(matches[matches.Count - 1].Value);
			}
			else
			{
				//StratusDebug.LogWarning($"Failed to parse index from {fileName}");
			}
			return -1;
		}
	}

	public class StratusDateTimeFileNamingConvention : StratusFileNamingConvention
	{
		public static readonly StratusTimestampParameters timestampParameters = new StratusTimestampParameters();

		public StratusDateTimeFileNamingConvention(string prefix) : base(prefix)
		{
		}

		public override string GenerateFileName(StratusSaveFileQuery files)
		{
			return $"{prefix}{prefixSeparator}{timestampParameters}";
		}
	}
}