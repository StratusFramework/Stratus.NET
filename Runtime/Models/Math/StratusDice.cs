using Stratus.Extensions;
using Stratus.Models.Math;

using System;
using System.Linq;

namespace Stratus.Models
{
	public enum StratusDie
	{
		d1,
		d4,
		d6,
		d8,
		d10,
		d12,
		d20
	}

	/// <summary>
	/// The roll of a single die
	/// </summary>
	public class StratusDieRoll
	{
		public StratusDieRoll(StratusDie die, int roll)
		{
			this.die = die;
			this.roll = roll;
		}

		public StratusDie die { get; }
		public int roll { get; }
	}

	/// <summary>
	/// The modifier for a given roll
	/// </summary>
	public class StratusDiceRollModifier
	{
		public StratusDiceRollModifier(string label, int value)
		{
			this.label = label;
			this.value = value;
		}

		public string label { get; }
		public int value { get; }

		public static implicit operator StratusDiceRollModifier(int value) => new StratusDiceRollModifier(string.Empty, value);
	}

	/// <summary>
	/// The roll of multiple dice, along with any modifiers
	/// </summary>
	public class StratusDiceRoll
	{
		public string label { get; }
		public StratusDieRoll[] dice { get; }
		public StratusDiceRollModifier[] modifiers { get; private set; }
		public int total { get; private set; }

		public StratusDiceRoll(string label, params StratusDieRoll[] rolls)
		{
			this.label = label;
			this.dice = rolls;
			this.total = rolls.Sum(r => r.roll);
		}

		public StratusDiceRoll(Enum label, params StratusDieRoll[] rolls)
			: this(label.ToString(), rolls)
		{
		}

		public StratusDiceRoll WithModifiers(params StratusDiceRollModifier[] modifiers)
		{
			this.modifiers = modifiers;
			this.total = total + modifiers.Sum(m => m.value);
			return this;
		}
	}

	/// <summary>
	/// Generates dice rolls based on the set arguments
	/// </summary>
	[Serializable]
	public class StratusDiceRollSource
	{
		public string label { get; }
		public StratusDie die { get; }
		public int n { get; }
		public StratusDiceRollModifier[] modifiers { get; private set; }

		public StratusDiceRollSource(Enum label, StratusDie die, int n = 1)
			: this(label.ToString(), die, n)
		{
		}

		public StratusDiceRollSource(string label, StratusDie die, int n = 1)
		{
			this.label = label;
			this.n = n;
			this.die = die;
		}

		public StratusDiceRollSource WithModifiers(params StratusDiceRollModifier[] modifiers)
		{
			this.modifiers = modifiers;
			return this;
		}

		public int GetTotalModifiers() => modifiers.Sum(m => m.value);

		public StratusDiceRoll Roll()
		{
			return StratusDice.Roll(label, die, n).WithModifiers(modifiers);
		}
	}

	/// <summary>
	/// Provides utility functions for generating dice rolls
	/// </summary>
	public static class StratusDice
	{
		private static bool initialized { get; set; }
		private static (string label, Func<int, int> func)? onNextRoll;

		/// <summary>
		/// Forces the result of the next roll returned by the <see cref="Roll"/> function. 
		/// This can have many uses (such as unit testing, difficulty modification, etc)
		/// </summary>
		/// <param name="func"></param>
		public static void NextRoll(Func<int, int> func, string label = null)
		{
			onNextRoll = (label, func);
		}

		public static void NextRoll(Func<int, int> func, Enum label)
		{
			NextRoll(func, label.ToString());
		}

		public static int Roll(int die, string label = null)
		{
			Initialize();
			int roll = Random(die);
			ModifyRoll(label, ref roll);
			return roll;
		}

		private static bool ModifyRoll(string label, ref int roll, bool consume = true)
		{
			// If there's a function set to modify the next roll
			if (onNextRoll.HasValue)
			{
				// If the function can target any roll
				// or if is to targeted at specific rolls and the requested roll has a matching label
				string nextRollLabel = onNextRoll.Value.label;
				if (nextRollLabel == null ||
					(nextRollLabel != null && nextRollLabel == label))
				{
					roll = onNextRoll.Value.func(roll);
					if (consume)
					{
						onNextRoll = null;
					}
					return true;
				}
			}
			return false;
		}

		public static StratusDiceRoll Roll(string label, params StratusDie[] dice)
		{
			StratusDieRoll[] rolls = new StratusDieRoll[dice.Length];
			bool modified = false;
			for (int i = 0; i < dice.Length; i++)
			{
				StratusDie die = dice[i];
				int roll = Roll(die.ToInteger());
				modified = ModifyRoll(label, ref roll, false);
				rolls[i] = new StratusDieRoll(die, roll);
			}
			if (modified)
			{
				onNextRoll = null;
			}
			return new StratusDiceRoll(label, rolls);
		}

		public static StratusDiceRoll Roll(string label, StratusDie die, int n)
		{
			return Roll(label, n.For(() => die).ToArray());
		}

		public static int Roll(int dice, Enum label) => Roll(dice, label.ToString());

		private static int Random(int die)
		{
			return RandomUtility.Range(1, die);
		}

		private static void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
			}
		}

		public static int ToInteger(this StratusDie die)
		{
			int result = 0;
			switch (die)
			{
				case StratusDie.d1:
					result = 1;
					break;
				case StratusDie.d4:
					result = 4;
					break;
				case StratusDie.d6:
					result = 6;
					break;
				case StratusDie.d8:
					result = 8;
					break;
				case StratusDie.d10:
					result = 10;
					break;
				case StratusDie.d12:
					result = 12;
					break;
				case StratusDie.d20:
					result = 20;
					break;
			}
			return result;
		}
	}
}