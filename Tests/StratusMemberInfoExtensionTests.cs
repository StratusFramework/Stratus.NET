using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Models;
using Stratus.Serialization;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stratus.Editor.Tests
{
	public class StratusMemberInfoExtensionTests
	{
		[ClassDescription(classDescription)]
		internal class MockDataObject
		{
			[MemberDescription(nameDescription)]
			public string name;
			[SerializeField]
			public int value;
			[MemberDescription(inverseValueDescription)]
			public int inverseValue => -this.value;

			public const string classDescription = "A test class used for the unit tests";
			public const string nameDescription = "The name of the object";
			public const string inverseValueDescription = "The inverse value";

			public MockDataObject(string name, int value)
			{
				this.name = name;
				this.value = value;
			}

			public override string ToString()
			{
				return this.name;
			}

			public void Boop1(int n, int b)
			{
				n.Iterate(() => Console.WriteLine(this.value + b));
			}

			public int Boop2(int c, int d)
			{
				return c + d;
			}
		}

		private static Type testType = typeof(MockDataObject);

		[Test]
		public void GetDescription()
		{
			PropertyInfo inverseValueProperty = testType.GetProperty(nameof(MockDataObject.inverseValue));
			Assert.AreEqual(inverseValueProperty.GetDescription(), MockDataObject.inverseValueDescription);
			Assert.NotNull(inverseValueProperty.GetAttribute<MemberDescriptionAttribute>());
			Assert.AreEqual(testType.GetDescription(), MockDataObject.classDescription);

			FieldInfo nameField = testType.GetFieldIncludePrivate(nameof(MockDataObject.name));
			Assert.AreEqual(nameField.GetDescription(), MockDataObject.nameDescription);
		}

		[Test]
		public void GetValue()
		{
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			const int value = 7;
			MockDataObject a = new MockDataObject("A", value);
			Assert.AreEqual(value, valueField.GetValue<int>(a));
		}

		[Test]
		public void HasAttribute()
		{
			FieldInfo nameField = testType.GetFieldIncludePrivate(nameof(MockDataObject.name));
			Assert.True(nameField.HasAttribute<MemberDescriptionAttribute>());
			Assert.True(nameField.HasAttribute(typeof(MemberDescriptionAttribute)));
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			Assert.False(valueField.HasAttribute<MemberDescriptionAttribute>());
		}

		[Test]
		public void MapAttributes()
		{
			FieldInfo valueField = testType.GetFieldIncludePrivate(nameof(MockDataObject.value));
			Dictionary<Type, Attribute> map = valueField.MapAttributes();
			Assert.True(map.ContainsKey(typeof(SerializeFieldAttribute)));
		}

		[Test]
		public void GetFullName()
		{
			Assert.AreEqual("Boop1(int n, int b)", testType.GetMethod(nameof(MockDataObject.Boop1)).GetFullName());
			Assert.AreEqual("Boop2(int c, int d)", testType.GetMethod(nameof(MockDataObject.Boop2)).GetFullName());
		}

		[Test]
		public void GetParameterNames()
		{
			Assert.AreEqual("int n, int b", testType.GetMethod(nameof(MockDataObject.Boop1)).GetParameterNames());
			Assert.AreEqual("int c, int d", testType.GetMethod(nameof(MockDataObject.Boop2)).GetParameterNames());
		}

		[Test]
		public void GetExtensionMethods()
		{
			var extMethods = typeof(MockDataObject).GetExtensionMethods().ToDictionary(x => x.Name, false);
			Assert.True(extMethods.ContainsKey(nameof(MockDataObjectExtensions.BoopForExtensions)));
		}

		[Test]
		public void IsExtensionMethod()
		{
			var methods = typeof(MockDataObject).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToDictionary(x => x.Name);
			Assert.True(methods.ContainsKey(nameof(MockDataObject.Boop1)));
			Assert.False(methods[nameof(MockDataObject.Boop1)].IsExtensionMethod());
			Assert.True(methods.ContainsKey(nameof(MockDataObject.Boop2)));
			Assert.False(methods[nameof(MockDataObject.Boop2)].IsExtensionMethod());

			var extMethods = typeof(MockDataObject).GetExtensionMethods().ToDictionary(x => x.Name, false);
			Assert.True(extMethods.ContainsKey(nameof(MockDataObjectExtensions.BoopForExtensions)));
			Assert.True(extMethods[nameof(MockDataObjectExtensions.BoopForExtensions)].IsExtensionMethod());
		}

		public enum MockEnum
		{
			None = 0,
			A = 2,
			B = 4,
			C = 8,
			D = 16
		}

		[TestCase(MockEnum.A)]
		[TestCase(MockEnum.B)]
		[TestCase(MockEnum.C)]
		[TestCase(MockEnum.D)]
		[TestCase(MockEnum.A | MockEnum.B)]
		public void EnumHasFlag(MockEnum value)
		{
			MockEnum mock = MockEnum.None;
			Assert.False(mock.HasFlag(value));
			mock |= value;
			Assert.True(mock.HasFlag(value));
		}
	}

	internal static class MockDataObjectExtensions
	{
		public static void BoopForExtensions(this StratusMemberInfoExtensionTests.MockDataObject obj)
		{
		}
	}

}