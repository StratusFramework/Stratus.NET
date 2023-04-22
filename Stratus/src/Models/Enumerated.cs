using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models
{
	public class Enumerated
	{
		public string name { get; }

		public Enumerated(string name)
		{
			this.name = name;
		}

		public Enumerated(Enum value)
		{
			this.name = value.ToString();
		}

		public override string ToString()
		{
			return name;
		}

		public bool Equals(Enumerated? x, Enumerated? y)
		{
			return x.name == y.name;
		}

		public override bool Equals(object? obj)
		{
			return name == ((Enumerated)obj).name;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}

		public static implicit operator string(Enumerated value) => value.name;
		public static implicit operator Enumerated(string name) => new Enumerated(name);
		public static implicit operator Enumerated(Enum value) => new Enumerated(value);		

		public static Enumerated[] Convert<TEnum>(IEnumerable<TEnum> values)
			where TEnum : Enum
			=> values.Select(v => new Enumerated(v.ToString())).ToArray();
	}
}