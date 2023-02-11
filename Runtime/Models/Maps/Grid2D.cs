using Stratus.Collections;

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
		StratusVector3Int[] SearchPath(StratusVector3Int start, StratusVector3Int end);
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
		public StratusVector3Int[] cells => _cells.Value;

		private Lazy<StratusVector3Int[]> _cells;

		public SquareGrid WithSize(int size)
		{
			return WithSize(new StratusVector3Int(size, size));
		}

		public SquareGrid WithSize(StratusVector3Int size)
		{
			xMin = yMin = 0;
			yMax = size.y - 1;
			xMax = size.x - 1;
			UpdateCells();
			return this;
		}

		private void UpdateCells()
		{
			_cells = new Lazy<StratusVector3Int[]>(() =>
			{
				List<StratusVector3Int> result = new List<StratusVector3Int>();
				for (int x = xMin; x <= xMax; x++)
				{
					for (int y = yMin; y <= yMax; y++)
					{
						result.Add(new StratusVector3Int(x, y));
					}
				}
				return result.ToArray();
			});
		}

		public bool Contains(StratusVector3Int position) => Contains(position.x, position.y);

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

		private StratusBictionary<TLayer, Type> typesByLayer = new StratusBictionary<TLayer, Type>();
		public HashSet<TObject> objects { get; }
		/// <summary>
		/// All the available layers for this map
		/// </summary>
		public readonly TLayer[] layers = StratusEnum.Values<TLayer>();
		/// <summary>
		/// By default, the base layer is the first enumerated value.
		/// </summary>
		protected virtual TLayer baseLayer => layers[0];

		private Dictionary<TLayer, StratusBictionary<StratusVector3Int, TObject>> objectPositionsByLayer { get; }
			= new Dictionary<TLayer, StratusBictionary<StratusVector3Int, TObject>>();
		protected static readonly Type baseType = typeof(TObject);
		#endregion

		#region Constructors
		public Grid2D(SquareGrid grid, CellLayout layout)
		{
			this.cellLayout = layout;
			this.grid = grid;
			foreach (var layer in layers)
			{
				objectPositionsByLayer.Add(layer, new StratusBictionary<StratusVector3Int, TObject>());
			}
		}

		public Grid2D(StratusVector3Int size, CellLayout layout)
			: this(new SquareGrid().WithSize(size), layout)
		{
		}
		#endregion

		#region Accessors
		public StratusOperationResult ContainsCell(StratusVector3Int position)
		{
			if (!grid.Contains(position))
			{
				return new StratusOperationResult(false, $"Map does not contain cell {position}");
			}
			return new StratusOperationResult(true, $"Map contains cell {position}");
		}

		public StratusOperationResult ContainsCell(int x, int y) => ContainsCell(new StratusVector3Int(x, y));

		/// <summary>
		/// Fills all the cells of a given layer with the given objects
		/// </summary>
		/// <returns></returns>
		/// <remarks>The objects to be added must still be unique/remarks>
		public StratusOperationResult Fill<UObject>(TLayer layer, Func<UObject> ctor)
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

		public StratusOperationResult Set(TLayer layer, TObject obj, StratusVector3Int position, bool move = true)
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
					return new StratusOperationResult(false, $"{obj} is already present in the layer at {objectPositionsByLayer[layer][obj]}");
				}
			}

			if (Contains(layer, position))
			{
				return new StratusOperationResult(false, $"{Get(layer, position)} is already present in the layer at {position}");
			}

			objectPositionsByLayer[layer].Add(position, obj);
			return true;
		}

		public StratusOperationResult Set<UObject>(UObject obj, StratusVector3Int position, bool move = true)
			where UObject : TObject
		{
			TLayer layer = GetLayer<UObject>();
			return Set(layer, obj, position, move);
		}

		public StratusOperationResult Remove(TLayer layer, TObject obj)
		{
			if (!Contains(layer, obj))
			{
				return new StratusOperationResult(false, $"{obj} is not present in the layer {layer}");
			}

			return objectPositionsByLayer[layer].Remove(obj);
		}

		public StratusOperationResult Contains(TLayer layer, TObject obj)
		{
			return objectPositionsByLayer[layer].Contains(obj);
		}

		public StratusOperationResult Contains(TLayer layer, StratusVector3Int position)
		{
			return objectPositionsByLayer[layer].Contains(position);
		}

		public StratusVector3Int? GetPosition(TLayer layer, TObject obj)
		{
			if (!Contains(layer, obj))
			{
				return null;
			}

			return objectPositionsByLayer[layer][obj];
		}

		public TObject Get(TLayer layer, StratusVector3Int position)
		{
			if (!Contains(layer, position))
			{
				return null;
			}

			return objectPositionsByLayer[layer][position];
		}

		public UObject Get<UObject>(TLayer layer, StratusVector3Int position)
			where UObject : TObject
		{
			return (UObject)Get(layer, position);
		}

		public UObject Get<UObject>(StratusVector3Int position)
			where UObject : TObject
		{
			Type t = typeof(UObject);
			if (!typesByLayer.Contains(t))
			{
				throw new Exception($"There's no layer associated with the type {t}");
			}

			return (UObject)Get(typesByLayer[t], position);
		}

		public TObject[] GetAll(StratusVector3Int position)
		{
			return layers.Select(l => Get(l, position)).Where(o => o != null).ToArray();
		}

		public StratusOperationResult ForEach<UObject>(TLayer layer, Action<UObject> action)
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

		public StratusOperationResult ForEach<UObject>(TLayer layer, Predicate<UObject> action)
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
					return new StratusOperationResult(false, $"Predicate failed for {kvp.Value}");
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
		public StratusOperationResult IsValid(TLayer layer, TObject obj)
		{
			return IsValid(layer, obj.GetType());
		}

		/// <returns>Whether that type is valid for the given layer</returns>
		public StratusOperationResult IsValid(TLayer layer, Type type)
		{
			if (!typesByLayer.Contains(layer) || typesByLayer[layer] == type)
			{
				return true;
			}
			return new StratusOperationResult(false, $"{type} is not valid for layer {layer}");
		}
		#endregion

		public virtual StratusTraversableStatus IsTraversible(StratusVector3Int position)
		{
			if (!ContainsCell(position))
			{
				return StratusTraversableStatus.Blocked;
			}

			return StratusTraversableStatus.Valid;
		}

		public TObject[] GetObjectsInRange<UObject>(StratusVector3Int position, StratusGridSearchRangeArguments args, TLayer layer)
			where UObject : TObject
		{
			var range = GetRange(position, args);
			return range.Where(kvp => Contains(layer, kvp.Key)).Select(kvp => Get(layer, kvp.Key)).ToArray();
		}

		public TObject[] GetObjectsInRange<UObject>(UObject obj, StratusGridSearchRangeArguments args)
			where UObject : TObject
		{
			TLayer layer = GetLayer<UObject>();
			StratusVector3Int? position = GetPosition(layer, obj);
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
		public StratusGridRange GetRange(StratusVector3Int center, StratusGridSearchRangeArguments args)
		{
			StratusGridRange values = null;
			args.traversalCostFunction = GetTraversalCost;
			args.traversableFunction += (pos) =>
			{
				if (!ContainsCell(pos))
				{
					return StratusTraversableStatus.Blocked;
				}

				return StratusTraversableStatus.Valid;
			};

			values = StratusGridUtility.GetRange(center, args, cellLayout);

			// If the min range is not 0...
			if (args.minimum > 0)
			{
				// Remove those whose is less than min range
				StratusGridRange filtered = new StratusGridRange();
				foreach (KeyValuePair<StratusVector3Int, float> kvp in values)
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

		public StratusGridRange GetRange(TLayer layer, TObject obj, StratusGridSearchRangeArguments args)
		{
			StratusVector3Int? position = GetPosition(layer, obj);
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
					return StratusTraversableStatus.Occupied;
				}

				return StratusTraversableStatus.Valid;
			};

			return GetRange(position.Value, args);
		}

		public StratusGridRange GetRange(TLayer layer, TObject obj, int range) => GetRange(layer, obj, new StratusGridSearchRangeArguments(range));
		public StratusGridRange GetRange(StratusVector3Int center, int range) => GetRange(center, new StratusGridSearchRangeArguments(range));

		/// <summary>
		/// By default, the cost to travel to a given cell is 1. Override to account for difficult terrain and the like.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public virtual float GetTraversalCost(StratusVector3Int position)
		{
			return 1;
		}

		public StratusVector3Int[] SearchPath(StratusVector3Int start, StratusVector3Int end) => SearchPath(start, end, IsTraversible);
		public StratusVector3Int[] SearchPath(StratusVector3Int start, StratusVector3Int end, StratusTraversalPredicate<StratusVector3Int> isTraversible)
		{
			return StratusGridUtility.FindPath(start, end, cellLayout, isTraversible);
		}
	}

	public abstract class Grid2D<TObject> : Grid2D<TObject, DefaultMapLayer>
		where TObject : class
	{
		protected Grid2D(StratusVector3Int size, CellLayout layout) : base(size, layout)
		{
		}
	}
}