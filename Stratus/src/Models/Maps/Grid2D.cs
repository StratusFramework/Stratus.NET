using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Numerics;
using Stratus.Search;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps
{
	public interface IGrid2D
	{
		CellLayout cellLayout { get; }
		GridPath SearchPath(Vector2Int start, Vector2Int end);
		Result ContainsCell(Vector2Int position);
		Result Contains(Layer layer, Vector2Int position);
		IObject2D Get(Layer layer, Vector2Int position);
		bool TryGet<TObject>(Layer layer, Vector2Int position, out TObject obj)
			where TObject : IObject2D
		{
			var _obj = Get(layer, position);
			if (_obj == null)
			{
				obj = default;
				return false;
			}
			obj = (TObject)_obj;
			return true;
		}
		bool TryGet(Layer layer, Vector2Int position, out IObject2D obj)
			=> TryGet<IObject2D>(layer, position, out obj);
		IEnumerable<TObject> GetAll<TObject>(Layer layer, IEnumerable<Vector2Int> positions)
			where TObject : IObject2D
			=> positions.Select(p => Get(layer, p)).Where(o => o != null).Cast<TObject>();
		Result Set(IObject2D reference, Vector2Int position);
	}

	/// <summary>
	/// Runtime data structure for managing a 2D map, along with its possible layers
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="Layer"></typeparam>
	public class Grid2D : IGrid2D
	{
		#region Fields
		private Dictionary<Layer, Bictionary<Vector2Int, IObject2D>> objectsByLayer { get; }
			= new Dictionary<Layer, Bictionary<Vector2Int, IObject2D>>();
		private Bictionary<Layer, Type> typesByLayer = new Bictionary<Layer, Type>();
		#endregion

		#region Properties
		/// <summary>
		/// The underlying grid structure
		/// </summary>
		public Bounds2D bounds { get; }
		/// <summary>
		/// The cell layout for this grid
		/// </summary>
		public CellLayout cellLayout { get; }
		/// <summary>
		/// All the available layers for this map
		/// </summary>
		public Layer[] layers { get; }
		/// <summary>
		/// By default, the base layer is the first enumerated value.
		/// </summary>
		protected virtual Layer baseLayer => layers[0];
		/// <summary>
		/// The total number of objects being tracked across all layers
		/// </summary>
		public int count => objectsByLayer.Sum(kvp => kvp.Value.Count);
		#endregion

		#region Static Properties
		protected static readonly Type baseType = typeof(IObject2D);
		#endregion

		#region Events
		public event Action<string> onLog;
		#endregion

		#region Constructors
		public Grid2D(Layer[] layers, Bounds2D grid, CellLayout layout)
		{
			this.layers = layers;
			this.cellLayout = layout;
			this.bounds = grid;
			foreach (var layer in layers)
			{
				objectsByLayer.Add(layer, new Bictionary<Vector2Int, IObject2D>());
			}
		}

		public Grid2D(Layer[] layers, Vector2Int size, CellLayout layout)
			: this(layers, new Bounds2D(size), layout)
		{
		}
		#endregion

		#region Accessors
		public Result ContainsCell(Vector2Int position)
		{
			if (!bounds.Contains(position))
			{
				return new Result(false, $"Map does not contain cell {position}");
			}
			return new Result(true, $"Map contains cell {position}");
		}

		public Result ContainsCell(int x, int y) => ContainsCell(new Vector2Int(x, y));

		/// <summary>
		/// Fills all the cells of a given layer with the given objects
		/// </summary>
		/// <returns></returns>
		/// <remarks>The objects to be added must still be unique/remarks>
		public Result Fill<UObject>(Layer layer, Func<UObject> ctor)
			where UObject : IObject2D
		{
			var typeCheck = IsValid(layer, typeof(UObject));
			if (!typeCheck)
			{
				return typeCheck;
			}

			Clear(layer);
			foreach (var position in bounds.cells)
			{
				objectsByLayer[layer].Add(position, ctor());
			}
			return true;
		}

		/// <summary>
		/// Sets the given <typeparamref name="TObject"/> on the layer at the position
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		/// <param name="move">Whether the object is being moved from a previous position</param>
		/// <returns></returns>
		public Result Set(IObject2D obj, Vector2Int position)
		{
			// Check the type
			Layer layer = obj.layer;
			var typeCheck = IsValid(layer, obj);
			if (!typeCheck)
			{
				return typeCheck;
			}

			if (Contains(layer, obj))
			{
				Remove(layer, obj);
			}

			if (Contains(layer, position))
			{
				return new Result(false, $"{Get(layer, position)} is already present in the layer at {position}");
			}

			objectsByLayer[layer].Add(position, obj);
			return new Result(true, $"Set {obj} onto {layer} at position {position}");
		}

		public Result Remove(Layer layer, IObject2D obj)
		{
			if (!Contains(layer, obj))
			{
				return new Result(false, $"{obj} is not present in the layer {layer}");
			}

			return objectsByLayer[layer].Remove(obj);
		}

		public Result Contains(Layer layer, IObject2D obj)
		{
			return objectsByLayer[layer].Contains(obj);
		}

		public Result Contains(Layer layer, Vector2Int position)
		{
			return objectsByLayer[layer].Contains(position);
		}

		public Vector2Int? GetPosition(Layer layer, IObject2D obj)
		{
			if (!Contains(layer, obj))
			{
				return null;
			}

			return objectsByLayer[layer][obj];
		}

		public IObject2D Get(Layer layer, Vector2Int position)
		{
			if (!Contains(layer, position))
			{
				return null;
			}

			return objectsByLayer[layer][position];
		}

		public UObject Get<UObject>(Layer layer, Vector2Int position)
			where UObject : IObject2D
		{
			return (UObject)Get(layer, position);
		}

		public UObject? Get<UObject>(Vector2Int position)
			where UObject : IObject2D
		{
			Type t = typeof(UObject);
			if (!typesByLayer.Contains(t))
			{
				throw new Exception($"There's no layer associated with the type {t}");
			}

			return (UObject)Get(typesByLayer[t], position);
		}

		public IObject2D[] GetAll(Vector2Int position)
		{
			return layers.Select(l => Get(l, position)).Where(o => o != null).ToArray();
		}

		public Result ForEach<UObject>(Layer layer, Action<UObject> action)
			where UObject : IObject2D
		{
			bool check = IsValid(layer, typeof(UObject));
			if (!check)
			{
				return check;
			}

			foreach (var kvp in objectsByLayer[layer])
			{
				action((UObject)kvp.Value);
			}

			return true;
		}

		public Result ForEach<UObject>(Layer layer, Predicate<UObject> action)
			where UObject : IObject2D
		{
			bool check = IsValid(layer, typeof(UObject));
			if (!check)
			{
				return check;
			}

			foreach (var kvp in objectsByLayer[layer])
			{
				if (!action((UObject)kvp.Value))
				{
					return new Result(false, $"Predicate failed for {kvp.Value}");
				}
			}

			return true;
		}

		public int Count(Layer layer)
		{
			return objectsByLayer[layer].Count;
		}

		public void Clear(Layer layer)
		{
			objectsByLayer[layer].Clear();
		}
		#endregion

		#region Validation
		public void Associate<UObject>(Layer layer)
			where UObject : IObject2D
		{
			typesByLayer.Add(layer, typeof(UObject));
		}

		/// <returns>Whether that object is valid for the given layer</returns>
		public Result IsValid(Layer layer, IObject2D obj)
		{
			return IsValid(layer, obj.GetType());
		}

		/// <returns>Whether that type is valid for the given layer</returns>
		public Result IsValid(Layer layer, Type type)
		{
			if (!typesByLayer.Contains(layer) || typesByLayer[layer] == type)
			{
				return true;
			}
			return new Result(false, $"{type} is not valid for layer {layer}");
		}
		#endregion

		public virtual TraversableStatus IsTraversible(Vector2Int position)
		{
			if (!ContainsCell(position))
			{
				return TraversableStatus.Blocked;
			}

			return TraversableStatus.Valid;
		}

		public UObject[] GetObjectsInRange<UObject>(Vector2Int position, GridSearchRangeArguments args, Layer layer)
			where UObject : IObject2D
		{
			var range = GetRange(position, args);
			return range.Where(kvp => Contains(layer, kvp.Key)).Select(kvp => Get(layer, kvp.Key)).Cast<UObject>().ToArray();
		}

		public TObject[] GetObjectsInRange<TObject>(TObject obj, GridSearchRangeArguments args)
			where TObject : IObject2D
		{
			Layer layer = obj.layer;
			Vector2Int? position = GetPosition(layer, obj);
			if (!position.HasValue)
			{
				return null;
			}

			return GetObjectsInRange<TObject>(position.Value, args, layer);
		}

		/// <summary>
		/// Returns a given range starting from a cell
		/// </summary>
		public GridRange GetRange(Vector2Int center, GridSearchRangeArguments args)
		{
			GridRange values = null;
			args.traversalCostFunction = GetTraversalCost;
			args.traversableFunction += (pos) =>
			{
				if (!ContainsCell(pos))
				{
					return TraversableStatus.Blocked;
				}

				return TraversableStatus.Valid;
			};

			values = GridSearch.GetRange(center, args, cellLayout);

			// If the min range is not 0...
			if (args.minimum > 0)
			{
				// Remove those whose is less than min range
				GridRange filtered = new GridRange();
				foreach (KeyValuePair<Vector2Int, float> kvp in values)
				{
					float cost = kvp.Value;
					if (cost >= args.minimum)
					{
						filtered.Add(kvp.Key, kvp.Value);
					}
				}
				return filtered;
			}

			return values;
		}

		public GridRange GetRange(Layer layer, IObject2D obj, GridSearchRangeArguments args)
		{
			Vector2Int? position = GetPosition(layer, obj);
			if (!position.HasValue)
			{
				StratusLog.Warning($"Could not find range {obj}. Could not find its position within the layer {layer}. ({objectsByLayer[layer].Count} objects)");
				return new GridRange();
			}

			args.traversableFunction += (pos) =>
			{
				// If there's another object in the same layer at the position, don't
				// TODO: Perhaps return whether we can go through the object or not
				var objAtPos = Get(layer, pos);
				if (objAtPos != null)
				{
					return TraversableStatus.Occupied;
				}

				return TraversableStatus.Valid;
			};

			return GetRange(position.Value, args);
		}

		public GridRange GetRange(Layer layer, IObject2D obj, int range) => GetRange(layer, obj, new GridSearchRangeArguments(range));
		public GridRange GetRange(Vector2Int center, int range) => GetRange(center, new GridSearchRangeArguments(range));

		/// <summary>
		/// By default, the cost to travel to a given cell is 1. Override to account for difficult terrain and the like.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public virtual float GetTraversalCost(Vector2Int position)
		{
			return 1;
		}

		public GridPath SearchPath(Vector2Int start, Vector2Int end) => SearchPath(start, end, IsTraversible);
		public GridPath SearchPath(Vector2Int start, Vector2Int end, TraversalPredicate<Vector2Int> isTraversible)
		{
			return GridSearch.FindPath(start, end, cellLayout, isTraversible);
		}
	}

	public class Grid2D<TLayer> : Grid2D
		where TLayer : Enum
	{
		private static TLayer[] _layers => EnumUtility.Values<TLayer>().ToArray();

		public Grid2D(Bounds2D grid, CellLayout layout) : base(Layer.Convert(_layers), grid, layout)
		{
		}

		public Grid2D(Vector2Int size, CellLayout layout) : base(Layer.Convert(_layers), size, layout)
		{
		}
	}
}