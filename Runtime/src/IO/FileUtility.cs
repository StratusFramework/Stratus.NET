using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Stratus.IO
{
	public static class FileUtility
	{
		/// <summary>
		/// Returns true if the file exists
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool FileExists(string filePath) => File.Exists(filePath);

		/// <summary>
		/// Given a filename, changes its extension
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static string ChangeExtension(string fileName, string extension) => Path.ChangeExtension(fileName, extension);

		/// <summary>
		/// Given a filename, changes its extension
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static string RemoveExtension(string fileName) => Path.GetFileNameWithoutExtension(fileName);

		/// <summary>
		/// Deletes the file at the given file path.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static bool DeleteFile(string filePath)
		{
			if (!FileExists(filePath))
			{
				return false;
			}

			File.Delete(filePath);
			return true;
		}

		public static bool DeleteFileOrDirectory(string path)
		{
			return DeleteFile(path) || DeleteDirectory(path);
		}

		/// <summary>
		/// Deletes the file at the given file path.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool DeleteDirectory(string path)
		{
			if (!Directory.Exists(path))
			{
				return false;
			}

			Directory.Delete(path);
			return true;
		}

		public static byte[] FileReadAllBytes(string filePath)
		{
			if (!FileExists(filePath))
			{
				return null;
			}
			byte[] result = File.ReadAllBytes(filePath);
			return result;
		}

		public static string GetFileName(string filePath, bool extension = true)
		{
			return extension ? Path.GetFileName(filePath) : Path.GetFileNameWithoutExtension(filePath);
		}

		/// <summary>
		/// Given a file or directory path, ensures the directories to it exist by creating them
		/// when needed
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static bool EnsureDirectoryAt(string path, bool clean = false)
		{
			var dir = new FileInfo(path).Directory;
			dir = Directory.CreateDirectory(dir.FullName);
			return true;
		}

		/// <summary>
		/// Combines multiple paths together
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		public static string CombinePath(params string[] paths)
		{
			string result = string.Empty;
			for (int i = 0; i < paths.Length; i++)
			{
				string p = paths[i];
				result = Path.Combine(result, p);
			}
			return result;
		}

		/// <summary>
		/// There are following custom format specifiers y (year), 
		/// M (month), d (day), h (hour 12), H (hour 24), m (minute), 
		/// s (second), f (second fraction), F (second fraction, 
		/// trailing zeroes are trimmed), t (P.M or A.M) and z (time zone).
		/// </summary>
		/// <returns></returns>
		public static string GetTimestamp(string format = "yyyy-MM-dd_HH-mm")
		{
			return DateTime.Now.ToString(format);
		}
	}

	public class StratusTimestampParameters
	{
		public bool year = true;
		public bool month = true;
		public bool day = true;

		public bool hour = true;
		public bool minute = true;
		public bool second = false;

		public char separator = '_';

		public override string ToString()
		{
			List<string> values = new List<string>();

			if (year) values.Add("yyyy");
			if (month) values.Add("MM");
			if (day) values.Add("dd");
			if (hour) values.Add("HH");
			if (minute) values.Add("mm");
			if (second) values.Add("ss");

			string format = values.Join(separator);

			return DateTime.Now.ToString(format);
		}

		public static readonly StratusTimestampParameters defaultValue = new StratusTimestampParameters();
	}
}
