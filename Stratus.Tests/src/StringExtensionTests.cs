using NUnit.Framework;

using Stratus.Extensions;

using System;

namespace Stratus.Tests
{
	public class StringExtensionTests 
    {
		[Test]
		public void EnclosesLine()
		{
			string input = "foo";
			Assert.AreEqual("(foo)", input.Enclose(StratusStringEnclosure.Parenthesis));
			Assert.AreEqual("[foo]", input.Enclose(StratusStringEnclosure.SquareBracket));
			Assert.AreEqual("{foo}", input.Enclose(StratusStringEnclosure.CurlyBracket));
			Assert.AreEqual("<foo>", input.Enclose(StratusStringEnclosure.AngleBracket));
			Assert.AreEqual("\"foo\"", input.Enclose(StratusStringEnclosure.DoubleQuote));
			Assert.AreEqual("'foo'", input.Enclose(StratusStringEnclosure.Quote));
		}

		[Test]
		public void IsNullOrEmpty()
		{
			string value = null;
			Assert.True(value.IsNullOrEmpty());
			value = string.Empty;
			Assert.True(value.IsNullOrEmpty());
			value = "Boo!";
			Assert.True(value.IsValid());
			value = "";
			Assert.False(value.IsValid());
			value = null;
			Assert.False(value.IsValid());
		}

		[Test]
		public void JoinsLines()
		{
			string[] values = new string[]
			{
				"A",
				"B",
				"C"
			};
			Assert.AreEqual("A B C", values.Join(" "));
			Assert.AreEqual("A,B,C", values.Join(","));
			Assert.AreEqual("A\nB\nC", values.JoinLines());
		}

		[Test]
		public void AppendLines()
		{
			string cat = "cat", dog = "dog", bird = "bird";
			Assert.AreEqual($"{dog}{Environment.NewLine}{cat}", dog.AppendLines(cat));
			Assert.AreEqual($"{dog}{Environment.NewLine}{cat}{Environment.NewLine}{bird}", dog.AppendLines(cat, bird));
			Assert.AreEqual($"{dog}{bird}{cat}", dog.Append(bird, cat));
		}

		[Test]
		public void TrimNullOrEmpty()
		{
			string predicatedString = "Waaagh";
			string[] input = new string[]
			{
				"Foo",
				"Bar",
				"",
				predicatedString,
				null,
				"Ya!"
			};

			string[] output;
			output = input.TrimNullOrEmpty();
			Assert.AreEqual(4, output.Length);
			output = input.TrimNullOrEmpty(x => !x.Contains(predicatedString));
			Assert.AreEqual(3, output.Length);
			Assert.False(output.Contains(predicatedString));
		}

		[Test]
		public void ToTitleCase()
		{
			void TestTitleCase(string value, string expected) => Assert.AreEqual(expected, value.ToTitleCase());
			TestTitleCase("COOL_MEMBER_NAME", "Cool Member Name");
			TestTitleCase("war and peace", "War And Peace");
			TestTitleCase("cool_class_name", "Cool Class Name");
			TestTitleCase("_cool_class_name", "Cool Class Name");
			TestTitleCase("_coolClassName", "Cool Class Name");
		}

		[Test]
		public void TestUpperFirst()
		{
			string value = "cat";
			Assert.AreEqual("Cat", value.UpperFirst());
		}

		[Test]
		public void Truncate()
		{
			string input = "Hello there brown cow";

			string text;
			text = input.Truncate(5);
			Assert.AreEqual("Hello...", text);

			text = input.Truncate(11);
			Assert.AreEqual("Hello there...", text);

			text = input.Truncate(input.Length);
			Assert.AreEqual(input, text);

			text = input.Truncate(5, "!!!");
			Assert.AreEqual("Hello!!!", text);
		}

		[TestCase("hello\nthere\ncat", 3)]
		[TestCase("hello", 1)]
		[TestCase("\rHi!\r\nHo!\nHi!\r\nHo!", 4)]
		[TestCase("", 1)]
		public void CountLines(string input, int count)
		{
			Assert.AreEqual(count, input.CountLines());
		}

		[TestCase("hello\nthere\ncat", "hello there cat")]
		public void ReplaceNewlines(string input, string expected)
		{
			string actual = input.ReplaceNewLines(" ");
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SortsSequenceOfStrings()
		{
			string a = "a", b = "b", c = "c";
			string[] input = new string[]
			{
				a, c, b
			};
			string[] output = input.ToSorted();
			Assert.AreEqual(output[0], a);
			Assert.AreEqual(output[1], b);
			Assert.AreEqual(output[2], c);
		}

		[TestCase("hi\nho", StringSplitOptions.None, "hi", "ho")]
		[TestCase("hi\nho\r\nhi\rho", StringSplitOptions.None, "hi", "ho", "hi", "ho")]
		[TestCase("hi\n\r\nhi\rho", StringSplitOptions.None, "hi", "", "hi", "ho")]
		public void SplitNewlines(string input, StringSplitOptions options, params string[] expected)
		{
			string[] actual = input.SplitNewlines(options);
			Assert.AreEqual(expected, actual);
		}

		[TestCase("pancakelord", "pancake", "lord")]
		[TestCase(" space ", "space", "  ")]
		public void Removes(string input, string substr, string expected)
		{
			Assert.That(input.Remove(substr), Is.EqualTo(expected));
		}
	}
}
