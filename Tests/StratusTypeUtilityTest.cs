using NUnit.Framework;

using Stratus.Extensions;
using Stratus.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Editor.Tests
{
	public class StratusTypeUtilityTest : StratusTest
	{
		private class MockA
		{
		}

		private class MockB : MockA
		{
		}

		[Test]
		public void GetsSubclassNameByTypeParameter()
		{
			var actual = StratusTypeUtility.SubclassNames<MockA>().ToHashSet();
			Assert.True(actual.Contains(nameof(MockB)));
		}

		[Test]
		public void GetsSubclassNameByType()
		{
			var actual = StratusTypeUtility.SubclassNames(typeof(MockA)).ToHashSet();
			Assert.True(actual.Contains(nameof(MockB)));
		}

		[TestCase(typeof(MockA), typeof(MockB))]
		public void SubclassesOf(Type baseType, params Type[] expected)
		{
			var actual = StratusTypeUtility.SubclassesOf(baseType);
			AssertEquality(expected, actual);
		}

		[AttributeUsage(AttributeTargets.Class)]
		private class MockAttribute : Attribute
		{
		}

		public interface MockInterface
		{
		}

		private class MockObject
		{
		}

		private class MockObject<T> : MockObject
		{
		}

		private class IntMockObject : MockObject<int>, MockInterface
		{
		}

		[Mock]
		private class StringMockObject : MockObject<string>, MockInterface
		{
		}

		[Test]
		public void FindsTypesDefinedFromGeneric()
		{
			Type baseType = typeof(MockObject<>);
			Type[] expected = new Type[]
			{
				typeof(IntMockObject),
				typeof(StringMockObject),
				typeof(IntCustomMockObject)
			};
			Type[] actual = StratusTypeUtility.TypesDefinedFromGeneric(baseType);
			AssertEquality(expected, actual);
		}

		[Test]
		public void GetsTypeDefinitionParameterMap()
		{
			Type baseType = typeof(MockObject<>);
			Dictionary<Type, Type[]> map = StratusTypeUtility.TypeDefinitionParameterMap(baseType);
			Assert.True(map.ContainsKey(typeof(int)));
			Assert.True(map[typeof(int)].Contains(typeof(IntMockObject)));
			Assert.True(map[typeof(string)].Length == 1 &&
				map[typeof(string)][0] == typeof(StringMockObject));
		}

		[TestCase(typeof(List<string>), typeof(string))]
		[TestCase(typeof(List<int>), typeof(int))]
		public void FindsCollectionElementType(Type collectionType, Type expected)
		{
			var collection = (ICollection)StratusObjectUtility.Instantiate(collectionType);
			var actual = StratusTypeUtility.GetElementType(collection);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void GetsTypesWithAttributes()
		{
			Type attrType = typeof(MockAttribute);
			var types = StratusTypeUtility.TypesWithAttribute(attrType).ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.AreEqual(typeof(StringMockObject), types[0]);
		}

		[TestCase("System.Int32", typeof(int))]
		public void ResolvesTypeFromString(string typeName, Type expected)
		{
			Assert.AreEqual(expected, StratusTypeUtility.ResolveType(typeName));
		}

		[Test]
		public void InterfaceImplementations()
		{
			Type baseType = typeof(MockObject);
			Type interfaceType = typeof(MockInterface);
			var implementationTypes = StratusTypeUtility.InterfaceImplementations(baseType, interfaceType);
			var expected = new Type[] { typeof(IntMockObject), typeof(StringMockObject) };
			Assert.AreEqual(expected.Length, implementationTypes.Length);
			AssertEquality(expected, implementationTypes);
		}

		[TestCase(typeof(int), typeof(IntMockObject), typeof(IntCustomMockObject))]
		public void ImplementationssOfType(Type argument, params Type[] expected)
		{
			var actual = StratusTypeUtility.ImplementationsOf(typeof(MockObject<>), argument);
			Assert.AreEqual(expected.Length, actual.Length);
			AssertEquality(expected, actual);
		}

		private abstract class CustomMockObject<T> : MockObject<T>
		{
		}

		private class IntCustomMockObject : CustomMockObject<int>
		{
		}

		[TestCase(typeof(MockObject<>), typeof(IntMockObject))]
		[TestCase(typeof(MockObject<>), typeof(IntCustomMockObject))]
		public void GetsSubclassesOfGenericType(Type genericType, params Type[] expected)
		{
			var actual = StratusTypeUtility.SubclassesOf(genericType);
			Assert.That(actual.Length > 0);
			var actualSet = actual.ToHashSet();
			Assert.That(expected.All(e => actualSet.Contains(e)));
		}


	}
}