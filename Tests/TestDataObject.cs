using Stratus.Extensions;

using System;

namespace Stratus.Editor.Tests
{
	[ClassDescription(classDescription)]
	public class TestDataObject
	{
		[MemberDescription(nameDescription)]
		public string name;
		//[HideInInspector, SerializeField]
		public int value;
		[MemberDescription(inverseValueDescription)]
		public int inverseValue => -this.value;

		public const string classDescription = "A test class used for the unit tests";
		public const string nameDescription = "The name of the object";
		public const string inverseValueDescription = "The inverse value";

		public TestDataObject(string name, int value)
		{
			this.name = name;
			this.value = value;
		}

		public override string ToString()
		{
			return this.name;
		}

		public void Boop(int n, int b)
		{
			n.Iterate(() => Console.WriteLine(this.value + b));
		}
	}
}
