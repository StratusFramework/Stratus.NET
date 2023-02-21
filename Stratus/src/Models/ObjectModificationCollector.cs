using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models
{
	/// <summary>
	/// Collects modifications for a given object, providing an interface for managing them
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="TObjectModification"></typeparam>
	/// <typeparam name="TModificationSource"></typeparam>
	public abstract class ObjectModificationCollector<TObject, TObjectModification> : IEnumerable<TObjectModification>
		where TObjectModification : IObjectModification<TObject>
	{
		#region Fields
		internal IEnumerable<TObjectModification> modifications => _modificationsByType.SelectMany(m => m.Value);

		internal Dictionary<string, List<TObjectModification>> _modificationsByLabel
			= new Dictionary<string, List<TObjectModification>>();

		internal Dictionary<Type, List<TObjectModification>> _modificationsByType
			= new Dictionary<Type, List<TObjectModification>>(); 
		#endregion

		#region Properties
		public TObject target { get; }
		public IReadOnlyDictionary<string, List<TObjectModification>> modificationsByLabel => _modificationsByLabel;
		public IReadOnlyDictionary<Type, List<TObjectModification>> modificationsByType => _modificationsByType;
		public int selectionsLeft => modifications.Sum(m => m.selectionsLeft);
		#endregion

		#region Constructors
		public ObjectModificationCollector(TObject target)
		{
			this.target = target;
		}
		#endregion

		#region Virtual
		protected abstract void OnModificationAdded(TObjectModification modification); 
		#endregion

		#region Setters
		/// <summary>
		/// Adds the modifications for a given label
		/// </summary>
		public void Add(string label, IEnumerable<TObjectModification> modifications)
		{
			// Remove the previous if present
			if (_modificationsByLabel.ContainsKey(label))
			{
				Remove(label);
			}
			_modificationsByLabel.Add(label, new List<TObjectModification>());
			_modificationsByLabel[label].AddRange(modifications);

			foreach (var mod in modifications)
			{
				Type type = mod.GetType();
				if (!_modificationsByType.ContainsKey(type))
				{
					_modificationsByType.Add(type, new List<TObjectModification>());
				}

				_modificationsByType[type].Add(mod);
				OnModificationAdded(mod);
				mod.Apply();
			}
		}

		public void Add(string label, params TObjectModification[] modifications)
			=> Add(label, (IEnumerable<TObjectModification>)modifications);

		/// <summary>
		/// Removes all the modifications applied from the given object (by using its type name)
		/// </summary>
		public void Remove(object source)
		{
			Remove(source.GetType().Name);
		}

		/// <summary>
		/// Removes all the modifications applied from the given source label
		/// </summary>
		public void Remove(string label)
		{
			if (_modificationsByLabel.ContainsKey(label))
			{
				foreach (var val in _modificationsByLabel[label])
				{
					Type type = val.GetType();
					_modificationsByType[type].Remove(val);
					val.Revert();
				}
				_modificationsByLabel.Remove(label);
			}
		}
		#endregion

		#region Accessors
		public int Count(Type type)
		{
			return _modificationsByType.GetValueOrDefault(type).Count;
		}

		public int CountSelectors(Type type)
		{
			return _modificationsByType.
				GetValueOrDefault(type).
				Sum(m => m.selectorCount);
		}

		public int CountSelectionsLeft(Type type)
		{
			return _modificationsByType.
				GetValueOrDefault(type).
				Sum(m => m.selectionsLeft);
		}

		public TObjectModification[] GetAvailable()
		{
			return modifications.Where(m => m.hasSelectionsLeft).ToArray();
		}

		public int CountSelectionsLeft<T>() where T : TObjectModification
		{
			return CountSelectionsLeft(typeof(T));
		}

		public bool HasSelectionsLeft(Type type) => CountSelectionsLeft(type) > 0;
		public bool HasSelectionsLeft<T>() where T : TObjectModification
			=> HasSelectionsLeft(typeof(T));

		public int CountSelections<T>() where T : TObjectModification
			=> CountSelectors(typeof(T));

		public IEnumerable<TObjectModification> Get(Type type)
		{
			return _modificationsByType.GetValueOrDefault(type);
		}

		public T[] Get<T>(Predicate<T> predicate = null) where T : TObjectModification
		{
			var mods = Get(typeof(T));
			if (mods == null)
			{
				return null;
			}

			var result = mods.Select(c => (T)c);
			if (predicate != null)
			{
				result = result.Where(m => predicate(m));
			}
			return result.ToArray();
		}

		public T GetFirst<T>(Predicate<T> predicate = null) where T : TObjectModification
		{
			var values = Get<T>(predicate);
			if (values == null)
			{
				return default;
			}
			return values.FirstOrDefault();
		}

		/// <summary>
		/// </summary>
		/// <returns>All the selections of the given modification, ordered by those with the least possible selections</returns>
		public StratusValueSelector<TValue>[] GetSelectors<TM2, TValue>()
			where TM2 : ObjectModification<TObject, TValue>, TObjectModification
		{
			TM2[] mods = Get<TM2>();
			return mods
				.SelectMany(m => m.selectors)
				.OrderBy(s => s.length)
				.ToArray();
		}

		/// <summary>
		/// </summary>
		/// <returns>All the selections of the given modification, ordered by those with the least possible selections</returns>
		public StratusValueSelector<TValue>[] GetAvailableSelectors<TModification, TValue>()
			where TModification : ObjectModification<TObject, TValue>, TObjectModification
		{
			return GetSelectors<TModification, TValue>()
				.Where(s => !s.hasBeenSelected).ToArray();
		}

		/// <summary>
		/// </summary>
		/// <returns>All the modifications with selectors where at least one contains the given value</returns>
		public TModification[] Get<TModification, TValue>(TValue value)
			where TModification : ObjectModification<TObject, TValue>, TObjectModification
		{
			TModification[] mods = Get<TModification>();
			return mods
				.Where(m => m.ContainsAny(value))
				.OrderBy(m => m.GetSelector(value).length)
				.ToArray();
		}

		/// <summary>
		/// Selects the value among available selections, prioritizing those with the least amount of options
		/// </summary>
		/// <returns>True if the value could be selected</returns>
		public bool Select<TModification, TValue>(TValue value)
			where TModification : ObjectModification<TObject, TValue>, TObjectModification
		{
			var mods = Get<TModification, TValue>(value);
			if (mods.IsNullOrEmpty<TModification>())
			{
				return false;
			}

			return mods.Any(m => m.Select(value));
		}

		/// <summary>
		/// </summary>
		/// <returns>True if the given values can be selected from among the remaining selectors</returns>
		public bool CanSelect<TModification, TValue>(params TValue[] values)
			where TModification : ObjectModification<TObject, TValue>, TObjectModification
		{
			HashSet<TValue> available = new HashSet<TValue>(values);
			var selectors = GetAvailableSelectors<TModification, TValue>();
			foreach (var s in selectors)
			{
				foreach (var v in available)
				{
					if (s.ContainsAll(v))
					{
						available.Remove(v);
						break;
					}
				}
			}
			return available.Count == 0;
		}
		#endregion

		#region Enumerable
		IEnumerator<TObjectModification> IEnumerable<TObjectModification>.GetEnumerator()
		{
			return _modificationsByLabel.SelectMany(m => m.Value).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _modificationsByLabel.SelectMany(m => m.Value).GetEnumerator();
		}
		#endregion
	}
}
