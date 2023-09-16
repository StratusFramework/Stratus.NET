using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Attributes
{
	public abstract class Score<TValue, TModifier>
		where TValue : struct
		where TModifier : Modifier<TValue>, new()
	{
		public TValue value;
		public List<TModifier> modifiers = new();

		public abstract TValue total { get; }

		public Score()
		{
		}

		public Score(TValue value)
		{
			this.value = value;
		}
	}

	public interface IModifier
	{
		string id { get; }
	}

	public class Modifier<T> : IModifier
	{
		public string id;
		public T value;

		string IModifier.id => id;
	}

	[Serializable]
	public class IntegerModifier : Modifier<int>
	{
	}


	[Serializable]
	public class IntegerScore : Score<int, IntegerModifier>
	{
		public override int total => value + modifiers.Sum(m => m.value);

		public IntegerScore()
		{
		}

		public IntegerScore(int value) : base(value)
		{
		}
	}
}
