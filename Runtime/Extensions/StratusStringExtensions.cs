using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Stratus.Extensions
{
	/// <summary>
	/// Encloses a string. For example, "dog" -> "<dog>"
	/// </summary>
	public enum StratusStringEnclosure
	{
		/// <summary>
		/// foo
		/// </summary>
		None,
		/// <summary>
		/// (foo)
		/// </summary>
		Parenthesis,
		/// <summary>
		/// 'foo'
		/// </summary>
		Quote,
		/// <summary>
		/// "foo"
		/// </summary>
		DoubleQuote,
		/// <summary>
		/// [foo]
		/// </summary>
		SquareBracket,
		/// <summary>
		/// {foo}
		/// </summary>
		CurlyBracket,
		/// <summary>
		/// <foo>
		/// </summary>
		AngleBracket
	}

	public static class StratusStringExtensions 
    {
		private static StringBuilder stringBuilder = new StringBuilder();
		public static readonly string[] newlineSeparators = new string[]
		{
			"\r\n",
			"\n",
			"\r",
		};

		public const char newlineChar = '\n';
		public const char whitespace = ' ';
		public const char underscore = '_';
		public const string newlineString = "\n";


		/// <summary>
		/// Returns true if the string is null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Returns true if the string is neither null or empty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsValid(this string str)
		{
			return !str.IsNullOrEmpty();
		}

		/// <summary>
		/// Appends a sequence of strings to the end of this string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static string Append(this string str, IEnumerable<string> sequence)
		{
			StringBuilder builder = new StringBuilder(str);
			foreach (string item in sequence)
			{
				builder.Append(item);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Appends the sequence to the given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static string Append(this string str, params string[] sequence)
		{
			return str.Append((IEnumerable<string>)sequence);
		}

		/// <summary>
		/// Encloses the given string. For example, with <see cref="StratusStringEnclosure.SquareBracket"/>: "foo" -> "[foo]"
		/// </summary>
		/// <param name="input"></param>
		/// <param name="enclosure"></param>
		/// <param name="escape">Whether the enclosure should be escaped by <see cref="Regex.Escape(string)"/></param>
		/// <returns></returns>
		public static string Enclose(this string input, StratusStringEnclosure enclosure, bool escape = false)
		{
			switch (enclosure)
			{
				case StratusStringEnclosure.Parenthesis:
					return escape ? $"{Regex.Escape("(")}{input}{Regex.Escape(")")}" : $"({input})";
				case StratusStringEnclosure.Quote:
					return escape ? $"{Regex.Escape("'")}{input}{Regex.Escape("'")}" : $"'{input}'";
				case StratusStringEnclosure.DoubleQuote:
					return escape ? $"{Regex.Escape("\"")}{input}{Regex.Escape("\"")}" : $"\"{input}\"";
				case StratusStringEnclosure.SquareBracket:
					return escape ? $"{Regex.Escape("[")}{input}{Regex.Escape("]")}" : $"[{input}]";
				case StratusStringEnclosure.CurlyBracket:
					return escape ? $"{Regex.Escape("{")}{input}{Regex.Escape("}")}" : $"{{{input}}}";
				case StratusStringEnclosure.AngleBracket:
					return escape ? $"{Regex.Escape("<")}{input}{Regex.Escape(">")}" : $"<{input}>";
			}
			return input;
		}



		/// <summary>
		/// Sorts the array using the default <see cref="Array.Sort(Array)"/>
		/// </summary>
		public static string[] ToSorted(this string[] source)
		{
			string[] destination = new string[source.Length];
			Array.Copy(source, destination, source.Length);
			Array.Sort(destination);
			return destination;
		}

		/// <summary>
		/// Strips all newlines in the string, replacing them with spaces
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ReplaceNewLines(this string str, string replacement)
		{
			return str.Replace("\n", replacement);
		}

		public static string[] SplitNewlines(this string input, StringSplitOptions options = StringSplitOptions.None)
		{
			if (input.IsNullOrEmpty())
			{
				return new string[] { };
			}

			return input.Split(newlineSeparators, options);
		}

		public static IEnumerable<string> ReadNewlines(this string input)
		{
			if (input == null)
			{
				yield break;
			}

			using (System.IO.StringReader reader = new System.IO.StringReader(input))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					yield return line;
				}
			}
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the specified separator between each element or member.
		/// </summary>
		public static string Join(this IEnumerable<string> str, string separator)
		{
			return string.Join(separator, str);
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the specified separator between each element or member.
		/// </summary>
		public static string Join(this IEnumerable<string> str, char separator)
		{
			return string.Join(separator.ToString(), str);
		}

		public static string Join(this string str, IEnumerable<string> values)
		{
			return string.Join(str, values);
		}

		public static string Join(this string str, params string[] values)
		{
			return string.Join(str, values);
		}

		/// <summary>
		/// Counts the number of lines in this string (by splitting it)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int CountLines(this string str, StringSplitOptions options = StringSplitOptions.None)
		{
			return str.Count(x => x == newlineChar) + 1;
		}

		/// <summary>
		/// Converts a string to camel case. eg: "Hello There" -> "helloThere"
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToCamelCase(this string str)
		{
			if (!string.IsNullOrEmpty(str) && str.Length > 1)
			{
				return char.ToLowerInvariant(str[0]) + str.Substring(1);
			}
			return str;
		}

		/// <summary>
		/// Converts a string to title case. eg: "HelloThere" -> "Hello There")
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string input)
		{
			StringBuilder builder = new StringBuilder();
			bool previouslyUppercase = false;

			for (int i = 0; i < input.Length; i++)
			{
				char current = input[i];

				if ((current == underscore || current == whitespace)
					&& i + 1 < input.Length)
				{
					if (i > 0)
					{
						builder.Append(whitespace);
					}

					char next = input[i + 1];
					if (char.IsLower(next))
					{
						next = char.ToUpper(next, CultureInfo.InvariantCulture);
					}
					builder.Append(next);
					i++;
				}
				else
				{
					// Special case for first char
					if (i == 0)
					{
						builder.Append(current.ToUpper());						previouslyUppercase = true;
					}
					// Upper
					else if (current.IsUpper())
					{
						if (previouslyUppercase)
						{
							builder.Append(current.ToLower());
						}
						else
						{
							builder.Append(whitespace);
							builder.Append(current);
							previouslyUppercase = true;
						}

					}
					// Lower
					else
					{
						builder.Append(current);
						previouslyUppercase = false;
					}
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Removes null or empty strings from the sequence
		/// </summary>
		public static IEnumerable<string> TrimNullOrEmpty(this IEnumerable<string> sequence)
			=> sequence.TrimNullOrEmpty(null);

		/// <summary>
		/// Removes strings that are null, empty or that fail the predicate from the sequence
		/// </summary>
		public static IEnumerable<string> TrimNullOrEmpty(this IEnumerable<string> sequence, Predicate<string> predicate)
		{
			foreach (var item in sequence)
			{
				if (item.IsValid())
				{
					if (predicate != null && !predicate.Invoke(item))
					{
						continue;
					}
					yield return item;
				}
			}
		}

		/// <summary>
		/// Removes null or empty strings from the sequence
		/// </summary>
		public static string[] TrimNullOrEmpty(this string[] sequence) => ((IEnumerable<string>)sequence).TrimNullOrEmpty().ToArray();

		/// <summary>
		/// Removes strings that are null, empty or that fail the predicate from the sequence
		/// </summary>
		public static string[] TrimNullOrEmpty(this string[] sequence, Predicate<string> predicate) => ((IEnumerable<string>)sequence).TrimNullOrEmpty(predicate).ToArray();

		/// <summary>
		/// Strips Unity's rich text from the given string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string StripRichText(this string input)
		{
			const string pattern = @"</?(b|i|size(=?.*?)|color(=?.*?))>";
			return Regex.Replace(input, pattern, string.Empty);
		}

		/// <summary>
		/// If the string exceeds the given length, truncates any characters at the cutoff length,
		/// appending the replacement string to the end instead
		/// </summary>
		/// <param name="input"></param>
		/// <param name="length"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string Truncate(this string input, int length, string replacement = "...")
		{
			if (input.Length > length)
			{
				input = $"{input.Substring(0, length)}{replacement}";
			}
			return input;
		}

		/// <summary>
		/// Uppercases the first character of this string
		/// </summary>
		public static string UpperFirst(this string input)
		{
			switch (input)
			{
				case null: throw new ArgumentNullException(nameof(input));
				case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input.First().ToString().ToUpper() + input.Substring(1);
			}
		}

		/// <summary>
		/// Appends all the lines to the given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="lines"></param>
		/// <returns></returns>
		public static string AppendLines(this string str, params string[] lines)
		{
			stringBuilder.Clear();

			stringBuilder.Append(str);
			if (lines.Length > 0)
			{
				lines.ForEach(x => stringBuilder.Append($"{Environment.NewLine}{x}"));
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Given a string and a dictionary of replacements, replaces all instances of each replacement found in the string
		/// </summary>
		/// <param name="input"></param>
		/// <param name="replacements"></param>
		/// <returns></returns>
		public static string Replace(this string input, Dictionary<string, string> replacements)
		{
			stringBuilder.Clear();
			stringBuilder.Append(input);
			foreach (var replacement in replacements)
			{
				stringBuilder.Replace(replacement.Key, replacement.Value);
			}
			return stringBuilder.ToString();
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the newline separator between each element or member.
		/// </summary>
		public static string JoinLines(this IEnumerable<string> str)
		{
			return string.Join(newlineString, str);
		}		
	}
}
