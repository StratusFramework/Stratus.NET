using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps
{
	public class Layer
	{
		public string name { get; }

		public Layer(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return name;
		}

		public bool Equals(Layer? x, Layer? y)
		{
			return x.name == y.name;
		}

		public override bool Equals(object? obj)
		{
			return name == ((Layer)obj).name;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();	
		}

		public static implicit operator Layer(string name) => new Layer(name);
		public static implicit operator Layer(Enum value) => new Layer(value.ToString());

		public static Layer[] Convert<TEnum>(IEnumerable<TEnum> values) 
			where TEnum : Enum
			=> values.Select(v => new Layer(v.ToString())).ToArray();
	}
}