using NUnit.Framework;

using Stratus.Systems;

using System.Numerics;

namespace Stratus.Editor.Tests
{
	public class StratusConsoleCommandTests : IConsoleCommandProvider
	{
		private enum MockEnum
		{
			Foo,
			Bar
		}

		private static string lastCommand => ConsoleCommand.lastCommand;

		[Test]
		public void FindsCommands()
		{
			var values = ConsoleCommand.commands.Value;
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		[ConsoleCommand("add", hidden = true)]
		public static int Add(int a, int b)
		{
			return (a + b);
		}

		[ConsoleCommand("addvector", hidden = true)]
		public static string AddVector(Vector3 a, Vector3 b)
		{
			return (a + b).ToString();
		}

		[ConsoleCommand("multfloat", hidden = true)]
		public static string MultiplyFloat(float a, float b, float c)
		{
			return (a * b * c).ToString();
		}

		[ConsoleCommand("flipbool", hidden = true)]
		public static bool FlipBoolean(bool value)
		{
			return !value;
		}

		[Test]
		public void TestLogMethod()
		{
			this.AssertCommandResult("log foo", "foo");
		}

		[TestCase("add 2 5", "7")]
		[TestCase("multfloat 2 5 3", 30)]
		[TestCase("flipbool true", false)]
		public void ExecutesMethodWithResult(string command, object expected)
		{
			AssertCommandResult(command, expected);
		}

		//------------------------------------------------------------------------/
		// Variables
		//------------------------------------------------------------------------/
		[ConsoleCommand(nameof(floatField), hidden = true)]
		private static float floatField;

		[ConsoleCommand(nameof(intField), hidden = true)]
		private static int intField;

		[ConsoleCommand(nameof(boolField), hidden = true)]
		private static bool boolField;

		[ConsoleCommand(nameof(stringField), hidden = true)]
		private static string stringField;

		[ConsoleCommand(nameof(vector3Field), hidden = true)]
		private static Vector3 vector3Field;

		[ConsoleCommand(nameof(intProperty), hidden = true)]
		private static int intProperty { get; set; }

		[ConsoleCommand(hidden = true)]
		private static int intGetProperty => 5;

		[ConsoleCommand(hidden = true)]
		private static MockEnum enumField;

		[TestCase(nameof(floatField), 5f)]
		[TestCase(nameof(intField), 7)]
		[TestCase(nameof(boolField), true)]
		[TestCase(nameof(stringField), "Hello there brown cat")]
		[TestCase(nameof(enumField), MockEnum.Foo)]
		public void FieldsCanBeSet(string member, object value)
		{
			AssertMemberSet(member, value);
		}

		[Test]
		public void Vector3CanBeSet()
		{
			this.AssertMemberSet(nameof(vector3Field), new Vector3(1f, 5.5f, 8.9f));
		}

		[TestCase(nameof(intProperty), 25)]
		public void PropertiesCanBeSet(string member, object value)
		{
			AssertMemberSet(member, value);
		}

		//------------------------------------------------------------------------/
		// Test Procedures
		//------------------------------------------------------------------------/
		private void InvokeCommand(string text)
		{
			ConsoleCommand.Submit(text);
		}

		private void AssertCommandResult(string text, object expected)
		{
			this.InvokeCommand(text);
			Assert.AreEqual(expected.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertMemberSet(string memberName, object value)
		{
			this.InvokeCommand($"{memberName} {value}");
			this.InvokeCommand($"{memberName}");
			Assert.AreEqual(value.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertGetProperty(string memberName, object value)
		{
			this.InvokeCommand($"{memberName} {value}");
			this.InvokeCommand($"{memberName}");
			Assert.AreNotEqual(value.ToString(), ConsoleCommand.latestResult);
		}

		private void AssertCommandResult(string text, float expected, float delta)
		{
			this.InvokeCommand(text);
			Assert.AreEqual(expected, float.Parse(ConsoleCommand.latestResult), delta);
		}



	}
}

