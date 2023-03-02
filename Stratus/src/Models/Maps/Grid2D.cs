using Stratus.Collections;
using Stratus.Numerics;
using Stratus.Search;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps
{
	public abstract class GridBase<TObject>
		where TObject : class
	{
	}

	public interface IGrid2D
	{
		CellLayout cellLayout { get; }
		Vector3Int[] SearchPath(Vector3Int start, Vector3Int end);
	}

	public enum DefaultMapLayer
	{
		Terrain,
		Object,
		Event,
		Agent,
	}

	public class SquareGrid
	{
		public int xMax { get; private set; }
		public int yMax { get; private set; }
		public int xMin { get; private set; }
		public int yMin { get; private set; }
		public Vector3Int[] cells => _cells.Value;

		private Lazy<Vector3Int[]> _cells;

		public SquareGrid WithSize(int size)
		{
			return WithSize(new Vector3Int(size, size));
		}

		public SquareGrid WithSize(Vector3Int size)
		{
			xMin = yMin = 0;
			yMax = size.y - 1;
			xMax = size.x - 1;
			UpdateCells();
			return this;
		}

		private void UpdateCells()
		{
			_cells = new Lazy<Vector3Int[]>(() =>
			{
				List<Vector3Int> result = new List<Vector3Int>();
				for (int x = xMin; x <= xMax; x++)
				{
					for (int y = yMin; y <= yMax; y++)
					{
						result.Add(new Vector3Int(x, y));
					}
				}
				return result.ToArray();
			});
		}

		public bool Contains(Vector3Int position) => Contains(position.x, position.y);

		public bool Contains(int x, int y)
		{
			return (x >= xMin && x <= xMax)
				&& (y >= yMin && y <= yMax);
		}
	}

	/// <summary>
	/// Runtime data structure for managing a 2D map, along with its possible layers
	/// </summary>
	/// <typeparam name="TObject"></typeparam>
	/// <typeparam name="TLayer"></typeparam>
	public class Grid2D<TObject, TLayer> : GridBase<TObject>, IGrid2D
		where TLayer : Enum
		where TObject : class
	{
		#region Properties
		public SquareGrid grid { get; }
		public CellLayout cellLayout { get; }

		private Bictionary<TLayer, Type> typesByLayer = new Bictionary<TLayer, Type>();
		public HashSet<TObject> objects { get; }
		/// <summary>
		/// All the available layers for this map
		/// </summary>
		public readonly TLayer[] layers = EnumUtility.Values<TLayer>();
		/// <summary>
		/// By default, the base layer is the first enumerated value.
		/// </summary>
		protected virtual TLayer baseLayer => layers[0];

		private Dictionary<TLayer, Bictionary<Vector3Int, TObject>> objectPositionsByLayer { get; }
			= new Dictionary<TLayer, Bictionary<Vector3Int, TObject>>();
		protected static readonly Type baseType = typeof(TObject);
		#endregion

		#region Constructors
		public Grid2D(SquareGrid grid, CellLayout layout)
		{
			this.cellLayout = layout;
			this.grid = grid;
			foreach (var layer in layers)
			{
				objectPositionsByLayer.Add(layer, new Bictionary<Vector3Int, TObject>());
			}
		}

		public Grid2D(Vector3Int size, CellLayout layout)
			: this(new SquareGrid().WithSize(size), layout)
		{
		}
		#endregion

		#region Accessors
		public Result ContainsCell(Vector3Int position)
		{
			if (!grid.Contains(position))
			{
				return new Result(false, $"Map does not contain cell {position}");
			}
			return new Result(true, $"Map contains cell {position}");
		}

		public Result ContainsCell(int x, int y) => ContainsCell(new Vector3Int(x, y));

		/// <summary>
		/// Fills all the cells of a given layer with the given objects
		/// </summary>
		/// <returns></returns>
		/// <remarks>The objects to be added must still be unique/remarks>
		public Result Fill<UObject>(TLayer layer, Func<UObject> ctor)
			where UObject : TObject
		{
			var typeCheck = IsValid(layer, typeof(UObject));
			if (!typeCheck)
			{
				return typeCheck;
			}

			Clear(layer);
			foreach (var position in grid.cells)
			{
				objectPositionsByLayer[layer].Add(position, ctor());
			}
			return true;
		}

		public Result Set(TLayer layer, TObject obj, Vector3Int position, bool move = true)
		{
			// Check the type
			var typeCheck = IsValid(layer, obj);
			if (!typeCheck)
			{
				return typeCheck;
			}

			if (Contains(layer, obj))
			{
				if (move)
				{
					Remove(layer, obj);
				}
				else
				{
					return new Result(false, $"{obj} is already present in the layer at {objectPositionsByLayer[layer][obj]}");
				}
			}

			if (Contains(layer, position))
			{
				return new Result(false, $"{Get(layer, position)} is already present in the layer at {position}");
			}

			objectPositionsByLayer[layer].Add(position, obj);
			return true;
		}

		public Result Set<UObject>(UObject obj, Vector3Int position, bool move = true)
			where UObject : TObject
		{
			TLayer layer = GetLayer<UObject>();
			return Set(layer, obj, position, move);
		}

		public Result Remove(TLayer layer, TObject obj)
		{
			if (!Contains(layer, obj))
			{
				return new Result(false, $"{obj} is not present in the layer {layer}");
			}

			return objectPositionsByLayer[layer].Remove(obj);
		}

		public Result Contains(TLayer layer, TObject obj)
		{
			return objectPositionsByLayer[layer].Contains(obj);
		}

		public Result Contains(TLayer layer, Vector3Int position)
		{
			return objectPositionsByLayer[layer].Contains(position);
		}

		public Vector3Int? GetPosition(TLayer layer, TObject obj)
		{
			if (!Contains(layer, obj))
			{
				return null;
			}

			return objectPositionsByLayer[layer][obj];
		}

		public TObject Get(TLayer layer, Vector3Int position)
		{
			if (!Contains(layer, position))
			{
				return null;
			}

			return objectPositionsByLayer[layer][position];
		}

		public UObject Get<UObject>(TLayer layer, Vector3Int position)
			where UObject : TObject
		{
			return (UObject)Get(layer, position);
		}

		public UObject Get<UObject>(Vector3Int position)
			where UObject : TObject
		{
			Type t = typeof(UObject);
			if (!typesByLayer.Contains(t))
			{
				throw new Exception($"There's no layer associated with the type {t}");
			}

			return (UObject)Get(typesByLayer[t], position);
		}

		public TObject[] GetAll(Vector3Int position)
		{
			return layers.Select(l => Get(l, position)).Where(o => o != null).ToArray();
		}

		public Result ForEach<UObject>(TLayer layer, Action<UObject> action)
			where UObject : TObject
		{
			bool check = IsValid(layer, typeof(UObject));
			if (!check)
			{
				return check;
			}

			foreach (var kvp in objectPositionsByLayer[layer])
			{
				action((UObject)kvp.Value);
			}

			return true;
		}

		public Result ForEach<UObject>(TLayer layer, Predicate<UObject> action)
			where UObject : TObject
		{
			bool check = IsValid(layer, typeof(UObject));
			if (!check)
			{
				return check;
			}

			foreach (var kvp in objectPositionsByLayer[layer])
			{
				if (!action((UObject)kvp.Value))
				{
					return new Result(false, $"Predicate failed for {kvp.Value}");
				}
			}

			return true;
		}

		public int Count(TLayer layer)
		{
			return objectPositionsByLayer[layer].Count;
		}

		public void Clear(TLayer layer)
		{
			objectPositionsByLayer[layer].Clear();
		}
		#endregion

		#region Validation
		public void Associate<UObject>(TLayer layer)
			where UObject : TObject
		{
			typesByLayer.Add(layer, typeof(UObject));
		}

		/// <returns>Whether that object is valid for the given layer</returns>
		public Result IsValid(TLayer layer, TObject obj)
		{
			return IsValid(layer, obj.GetType());
		}

		/// <returns>Whether that type is valid for the given layer</returns>
		public Result IsValid(TLayer layer, Type type)
		{
			if (!typesByLayer.Contains(layer) || typesByLayer[layer] == type)
			{
				return true;
			}
			return new Result(false, $"{type} is not valid for layer {layer}");
		}
		#endregion

		public virtual TraversableStatus IsTraversible(Vector3Int position)
		{
			if (!ContainsCell(position))
			{
				return TraversableStatus.Blocked;
			}

			return TraversableStatus.Valid;
		}

		public TObject[] GetObjectsInRange<UObject>(Vector3Int position, GridSearchRangeArguments args, TLayer layer)
			where UObject : TObject
		{
			var range = GetRange(position, args);
			return range.Where(kvp => Contains(layer, kvp.Key)).Select(kvp => Get(layer, kvp.Key)).ToArray();
		}

		public TObject[] GetObjectsInRange<UObject>(UObject obj, GridSearchRangeArguments args)
			where UObject : TObject
		{
			TLayer layer = GetLayer<UObject>();
			Vector3Int? position = GetPosition(layer, obj);
			if (!position.HasValue)
			{
				return null;
			}

			return GetObjectsInRange<UObject>(position.Value, args, layer);
		}

		private TLayer GetLayer<UObject>() where UObject : TObject
		{
			Type type = typeof(UObject);
			TLayer layer = typesByLayer[type];
			return layer;
		}

		/// <summary>
		/// Returns a given range starting from a cell
		/// </summary>
		public GridRange GetRange(Vector3Int center, GridSearchRangeArguments args)
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

			values = GridUtility.GetRange(center, args, cellLayout);

			// If the min range is not 0...
			if (args.minimum > 0)
			{
				// Remove those whose is less than min range
				GridRange filtered = new GridRange();
				foreach (KeyValuePair<Vector3Int, float> kvp in values)
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

		public GridRange GetRange(TLayer layer, TObject obj, GridSearchRangeArguments args)
		{
			Vector3Int? position = GetPosition(layer, obj);
			if (!position.HasValue)
			{
				return null;
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

		public GridRange GetRange(TLayer layer, TObject obj, int range) => GetRange(layer, obj, new GridSearchRangeArguments(range));
		public GridRange GetRange(Vector3Int center, int range) => GetRange(center, new GridSearchRangeArguments(range));

		/// <summary>
		/// By default, the cost to travel to a given cell is 1. Override to account for difficult terrain and the like.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public virtual float GetTraversalCost(Vector3Int position)
		{
			return 1;
		}

		public Vector3Int[] SearchPath(Vector3Int start, Vector3Int end) => SearchPath(start, end, IsTraversible);
		public Vector3Int[] SearchPath(Vector3Int start, Vector3Int end, StratusTraversalPredicate<Vector3Int> isTraversible)
		{
			return GridUtility.FindPath(start, end, cellLayout, isTraversible);
		}
	}

	public abstract class Grid2D<TObject> : Grid2D<TObject, DefaultMapLayer>
		where TObject : class
	{
		protected Grid2D(Vector3Int size, CellLayout layout) : base(size, layout)
		{
		}
	}
}