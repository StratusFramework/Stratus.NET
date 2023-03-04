using Stratus.Collections;
using Stratus.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models
{
	public interface IObjectModification<TObject>
	{
		/// <summary>
		/// The object being modified
		/// </summary>
		public TObject target { get; }

		/// <summary>
		/// The number of available selectors
		/// </summary>
		public abstract int selectorCount { get; }
		/// <summary>
		/// The number of available selectors that have not made a selection yet
		/// </summary>
		public abstract int selectionsLeft { get; }
		/// <summary>
		/// /Whether there are selections left to be made
		/// </summary>
		public abstract bool hasSelectionsLeft { get; }

		/// <summary>
		/// Invoked whenever changes have been applied or reverted
		/// </summary>
		public event Action onSelection;

		/// <summary>
		/// Applies all modifications onto the object
		/// </summary>
		public abstract void Apply();
		/// <summary>
		/// Reverts all modifications made on the object
		/// </summary>
		public abstract void Revert();
	}

	public abstract class ObjectModification<TObject> : IObjectModification<TObject>
	{
		/// <summary>
		/// The object being modified
		/// </summary>
		public TObject target { get; }

		/// <summary>
		/// The number of available selectors
		/// </summary>
		public abstract int selectorCount { get; }
		/// <summary>
		/// The number of available selectors that have not made a selection yet
		/// </summary>
		public abstract int selectionsLeft { get; }
		/// <summary>
		/// /Whether there are selections left to be made
		/// </summary>
		public abstract bool hasSelectionsLeft { get; }

		/// <summary>
		/// Invoked whenever changes have been applied or reverted
		/// </summary>
		public event Action onSelection;

		/// <summary>
		/// Applies all modifications onto the object
		/// </summary>
		public abstract void Apply();
		/// <summary>
		/// Reverts all modifications made on the object
		/// </summary>
		public abstract void Revert();

		/// <summary>
		/// Invoke whenever the modification changes
		/// </summary>
		protected void OnSelection() => onSelection?.Invoke();

		public ObjectModification(TObject character)
		{
			this.target = character;
		}
	}
	
	/// <summary>
	/// Modifies an object by use of 1-many selectors of 1-many values
	/// </summary>
	/// <typeparam name="TObject">The object being modified</typeparam>
	/// <typeparam name="TValue">The type of value for the selection</typeparam>
	public abstract class ObjectModification<TObject, TValue> : ObjectModification<TObject>
	{
		public class Initializer
		{
			public Func<TObject, IEnumerable<ValueSelector<TValue>>> evaluationFunction { get; private set; }
			public StratusProvider<ValueSelector<TValue>[]> values { get; }

			public Initializer(params ValueSelector<TValue>[] values)
			{
				this.values = values;
			}

			public Initializer(IEnumerable<ValueSelector<TValue>> values)
			{
				this.values = values.ToArray();
			}

			public Initializer(IEnumerable<ValueSelector<TValue>> values,
				Func<TObject, IEnumerable<ValueSelector<TValue>>> evaluationFunction)
			{
				this.values = values.ToArray();
				this.evaluationFunction = evaluationFunction;
			}
		}

		/// <summary>
		/// The provided selectors
		/// </summary>
		protected ValueSelector<TValue>[] _selectors;
		/// <summary>
		/// If provided, evaluates more possible values
		/// </summary>
		protected Func<TObject, Initializer> _evaluationFunction;

		#region Properties
		/// <summary>
		/// Made selections
		/// </summary>
		private Dictionary<ValueSelector<TValue>, TValue> _selections { get; } = new Dictionary<ValueSelector<TValue>, TValue>();
		/// <summary>
		/// Available selectors
		/// </summary>
		public IReadOnlyList<ValueSelector<TValue>> selectors => _selectors;
		/// <summary>
		/// The total amount of selectors available (selected, not selected)
		/// </summary>
		public override int selectorCount => _selectors.Length;
		/// <summary>
		/// Whether there are selectors still unselected
		/// </summary>
		public override bool hasSelectionsLeft => _selectors.Any(x => !x.hasBeenSelected);
		/// <summary>
		/// The amount of possible selections left to be made
		/// </summary>
		public override int selectionsLeft => _selectors.Count(x => !x.hasBeenSelected);
		public bool evaluated { get; private set; } = true;
		public IEnumerable<ValueSelector<TValue>> remainingSelections =>
			_selectors.Where(v => v.hasBeenSelected);
		#endregion

		#region Virtual
		protected abstract bool Apply(TValue value);
		protected abstract bool Revert(TValue value);
		#endregion


		#region Constructors
		public ObjectModification(TObject target) : base(target)
		{
		}

		protected ObjectModification(TObject target, IEnumerable<ValueSelector<TValue>> values)
			: this(target)
		{
			Set(values);
		}

		public ObjectModification(TObject target, Initializer initializer) : base(target)
		{
			Initialize(initializer);
		}

		protected ObjectModification(TObject target, IEnumerable<TValue> values)
			: this(target)
		{
			Set(values);
		}

		protected ObjectModification(TObject target, params TValue[] values)
			: this(target, (IEnumerable<TValue>)values)
		{
		}
		#endregion

		#region Initialization
		protected void Initialize(Initializer initializer)
		{
			var values = initializer.values.value;
			if (initializer.evaluationFunction != null)
			{
				var evaluatedValues = initializer.evaluationFunction(target).ToArray();
				values = values.Concat(evaluatedValues);
			}
			Set(values);
		}

		protected virtual void Set(IEnumerable<ValueSelector<TValue>> values)
		{
			_selectors = values.ToArray();
		}

		protected virtual void Set(IEnumerable<TValue> values)
		{
			Set(values.Select(v => new ValueSelector<TValue>(v)));
		}
		#endregion

		/// <summary>
		/// Appplies any fixed values (those without possible choices)
		/// </summary>
		public override void Apply()
		{
			foreach (var s in selectors)
			{
				if (!s.hasMultipleValues)
				{
					Apply(s);
				}
			}
		}

		/// <summary>
		/// Reverts any values that has been applied
		/// </summary>
		public override void Revert()
		{
			foreach (var s in _selections.ToArray())
			{
				Revert(s.Key);
			}
		}

		/// <summary>
		/// </summary>
		/// <returns>True if a selector with ALL the given values exists</returns>
		public bool ContainsAll(params TValue[] values)
		{
			return _selectors.Any(s => s.hasMultipleValues && s.values.IsComparableByValues(values));
		}

		/// <summary>
		/// </summary>
		/// <returns>True if a selector with ANY of the given values exists</returns>
		public bool ContainsAny(params TValue[] values)
		{
			return _selectors.Any(s => s.hasMultipleValues && s.values.ContainsAny(values));
		}

		/// <summary>
		/// </summary>
		/// <returns>The selector that has all the given values</returns>
		public IEnumerable<ValueSelector<TValue>> GetSelectors(params TValue[] values)
		{
			return _selectors
				.Where(s => s.ContainsAll(values))
				.OrderBy(s => s.length);
		}

		/// <summary>
		/// </summary>
		/// <returns>The selector that has all the given values</returns>
		public ValueSelector<TValue> GetSelector(params TValue[] values)
		{
			return GetSelectors(values).First();
		}

		/// <summary>
		/// Iterates through all the selector for one that has the given value
		/// </summary>
		/// <param name="value"></param>
		/// <returns>True if it was selected</returns>
		public bool Select(TValue value)
		{
			return _selectors.Any(s =>
			{
				if (_selections.ContainsKey(s) || s.hasBeenSelected || !s.ContainsAll(value))
				{
					return false;
				}

				s.Select(value);
				Apply(s);
				return true;
			});
		}

		/// <summary>
		/// </summary>
		/// <returns>True if the given value can be selected among the possible selectors</returns>
		public bool CanSelect(TValue value)
		{
			return _selectors.Any(s =>
			{
				if (_selections.ContainsKey(s) || s.hasBeenSelected || !s.ContainsAll(value))
				{
					return false;
				}
				return true;
			});
		}

		/// <summary>
		/// Iterates through all possible selections for the one that has
		/// selected the given value, deselecting it
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Deselect(TValue value)
		{
			return _selectors.Any(s =>
			{
				if (!s.hasBeenSelected || !s.ContainsAll(value))
				{
					return false;
				}

				s.Deselect();
				Revert(s);
				return true;
			});
		}

		private bool Apply(ValueSelector<TValue> selector)
		{
			if (!Apply(selector.selection))
			{
				return false;
			}

			OnSelection();
			_selections.Add(selector, selector.selection);
			return true;
		}

		private bool Revert(ValueSelector<TValue> selector)
		{
			OnSelection();
			TValue value = _selections[selector];
			_selections.Remove(selector);
			return Revert(value);
		}
	}

	public abstract class ObjectModification<TObject, TKey, TValue> : ObjectModification<TObject, Tuple<TKey, TValue>>
	{
		public class Item
		{
			public Item(TKey key, TValue value)
			{
				this.key = key;
				this.value = value;
			}

			public TKey key { get; }
			public TValue value { get; }
		}

		private Dictionary<TKey, TValue> original = new Dictionary<TKey, TValue>();

		protected ObjectModification(TObject character) : base(character)
		{
		}

		public ObjectModification(TObject character, IEnumerable<Tuple<TKey, TValue>> values)
			: base(character, values)
		{
		}

		public ObjectModification(TObject character, Initializer initializer) : base(character, initializer)
		{
		}

		protected override bool Apply(Tuple<TKey, TValue> value)
		{
			original.Add(value.Item1, GetValue(value.Item1));
			SetValue(value.Item1, value.Item2);
			return true;
		}

		protected override bool Revert(Tuple<TKey, TValue> value)
		{
			return Apply(new Tuple<TKey, TValue>(value.Item1, original[value.Item1]));
		}

		protected abstract TValue GetValue(TKey key);
		protected abstract void SetValue(TKey key, TValue proficiency);
	}

	/// <summary>
	/// Defines the object as being part of what defines a <see cref="Character"/>
	/// </summary>
	public interface TObjectModificationSource<TObject, TModification>
		where TModification : IObjectModification<TObject>
	{
	}
}
