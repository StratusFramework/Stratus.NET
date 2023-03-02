using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Models.Saves;

using System.Linq;
using System.Text.RegularExpressions;

namespace Stratus.IO
{
	public abstract class FileNamingConvention
	{
		protected FileNamingConvention(string prefix)
		{
			this.prefix = prefix;
		}

		public string prefix { get; private set; }
		public const char prefixSeparator = '_';
		public abstract string GenerateFileName(StratusSaveFileQuery files);
	}

	public class IncrementalFileNamingConvention : FileNamingConvention
	{
		public IncrementalFileNamingConvention(string prefix) : base(prefix)
		{
		}

		private AutoSortedList<int, SaveFileInfo> filesByIndex { get; set; }

		public const string indexPattern = @"(?<index>\d+)";
		public const string indexCaptureGroupName = "index";

		public override string GenerateFileName(StratusSaveFileQuery files)
		{
			if (filesByIndex == null)
			{
				filesByIndex = new AutoSortedList<int, SaveFileInfo>(x => ParseIndex(x.name));
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

	public class DateTimeFileNamingConvention : FileNamingConvention
	{
		public static readonly StratusTimestampParameters timestampParameters = new StratusTimestampParameters();

		public DateTimeFileNamingConvention(string prefix) : base(prefix)
		{
		}

		public override string GenerateFileName(StratusSaveFileQuery files)
		{
			return $"{prefix}{prefixSeparator}{timestampParameters}";
		}
	}
}