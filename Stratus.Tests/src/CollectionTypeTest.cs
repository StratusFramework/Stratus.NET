using NUnit.Framework;

using Stratus.Types;

using System;
using System.Collections.Generic;

namespace Stratus.Tests
{
    public class CollectionTypeTest
    {
        [TestCase(typeof(List<int>), CollectionType.List)]
		[TestCase(typeof(HashSet<int>), CollectionType.HashSet)]
		[TestCase(typeof(Dictionary<string, int>), CollectionType.Dictionary)]
		public void DeducesCollectionType(Type type, CollectionType expected)
        {
            var actual = TypeUtility.Deduce(type);
            Assert.That(expected, Is.EqualTo(actual));
        }

		[TestCase(typeof(List<int>))]
		[TestCase(typeof(HashSet<int>))]
		[TestCase(typeof(Dictionary<string, int>))]
		public void IsCollectionType(Type type)
        {
            Assert.That(TypeUtility.IsCollection(type));
        }

		[TestCase(typeof(Dictionary<string, int>), typeof(string), typeof(int))]
		[TestCase(typeof(Dictionary<int, string>), typeof(int), typeof(string))]
		public void GetsKeyValueTypes(Type type, Type keyType, Type valueType)
        {
            var types = TypeUtility.GetKeyValueType(type);
            Assert.That(types.keyType, Is.EqualTo(keyType));
			Assert.That(types.valueType, Is.EqualTo(valueType));
		}
    }
}
