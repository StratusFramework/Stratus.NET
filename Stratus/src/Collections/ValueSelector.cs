using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	/// <summary>
	/// Provides a way to define a selection that is either fixed (1 value) 
	/// or that requires the user to select one of the possible values (> 1 value).
	/// </summary>
	/// <typeparam name="TValue"></typeparam>
	public class ValueSelector<TValue> 
	{
		public virtual TValue[] values { get; private set; }
		public TValue selection { get; private set; }
		public bool hasMultipleValues => values.LengthOrZero() > 1;
		public bool hasBeenSelected { get; private set; }
		public int length => values.Length;

		public event Action<TValue> onSelection;
		public event Action onDeselection;

		public ValueSelector()
		{
		}

		public ValueSelector(IEnumerable<TValue> values)
			: this(values.ToArray())
		{
		}

		public ValueSelector(params TValue[] values)
		{
			this.values = values;
			if (this.values.Length == 1)
			{
				Select(values[0]);
			}
		}

		public override string ToString()
		{
			string result = string.Empty;
			result = values.Take(5).ToStringJoin().Enclose(StratusStringEnclosure.SquareBracket);
			return result;
		}

		public bool ContainsAll(params TValue[] values) => values.ContainsAll(values);

		public void Select(TValue value)
		{
			if (!values.Contains(value))
			{
				throw new ArgumentOutOfRangeException($"The value {value} is not among those possible values {values.ToStringJoin()}");
			}

			this.selection = value;
			onSelection?.Invoke(value);
			hasBeenSelected = true;
		}

		public void Deselect()
		{
			this.selection = default;
			hasBeenSelected = false;
			onDeselection?.Invoke();
		}

		public ValueSelector<TValue> From(Func<object, TValue> evaluation)
		{
			return this;
		}
	}

	public class StratusValueEvaluationException : Exception
	{
		public StratusValueEvaluationException(string message) : base(message)
		{
		}
	}

	public class StratusValueSelection<TValue, TEvaluatedObject> : ValueSelector<TValue>
	{
		private Func<TEvaluatedObject, TValue[]> evaluationFunction;
		public override TValue[] values
		{
			get
			{
				if (_values == null
					&& evaluationFunction != null)
				{
					throw new StratusValueEvaluationException($"The possible values were not evaluated. Make sure the {nameof(Evaluate)} function is invoked on this object");
				}
				return _values;
			}
		}

		private TValue[] _values;

		public StratusValueSelection()
		{
		}

		public StratusValueSelection(params TValue[] values) : base(values)
		{
		}

		public StratusValueSelection(Func<TEvaluatedObject, TValue[]> evaluationFunction)
		{
			Set(evaluationFunction);
		}

		protected void Set(Func<TEvaluatedObject, TValue[]> evaluationFunction)
		{
			this.evaluationFunction = evaluationFunction;
		}

		/// <summary>
		/// Evaluates the possible values
		/// </summary>
		/// <param name="source"></param>
		/// <returns>True if the values were already set or if the evaluation was executed</returns>
		public bool Evaluate(TEvaluatedObject source)
		{
			if (_values != null)
			{
				if (evaluationFunction == null)
				{
					return true;
				}
				return false;
			}
			_values = evaluationFunction(source);
			return true;
		}
	}

}