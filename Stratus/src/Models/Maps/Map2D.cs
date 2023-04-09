using Stratus.Data;
using Stratus.Logging;
using Stratus.Numerics;
using Stratus.Search;

using System;

namespace Stratus.Models.Maps
{
	public interface ISpatialModel
	{
	}

	public interface IMap2D : ISpatialModel
	{
		IGrid2D grid { get; }
		GridRange GetRange(IActor2D actor);
		GridPath GetPath(IActor2D actor, Vector2Int position);
	}

	public abstract class Map2D : IMap2D, IStratusLogger
	{
		public abstract IGrid2D grid { get; }

		public abstract GridRange GetRange(IActor2D actor);
		public abstract GridPath GetPath(IActor2D actor, Vector2Int position);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TMap">The tilemap</typeparam>
	/// <typeparam name="TLayer">The layer definition</typeparam>
	/// <typeparam name="TObject">The object type</typeparam>
	/// <typeparam name="TData">The tile data type</typeparam>
	public abstract class Map2D<TData, TLayer> : Map2D
		where TData : class
		where TLayer : Enum
	{
		public class Grid : Grid2D<IObject2D, TLayer>
		{
			public Grid(Bounds2D grid, CellLayout layout) : base(grid, layout)
			{
			}

			public Grid(Vector2Int size, CellLayout layout) : base(size, layout)
			{
			}
		}

		protected Grid _grid;
		public abstract TLayer actorLayer { get; }
		public override IGrid2D grid => _grid;

		public Map2D(Func<Grid> ctor)
		{
			_grid = ctor();
		}

		protected abstract TraversableStatus CanTraverse(IActor2D actor, Vector2Int pos);

		public TObject? Get<TObject>(Vector2Int position, TLayer layer)
			where TObject : IObject2D
		{
			var info = _grid.Get(layer, position);
			if (info == null || info is not TObject)
			{
				return default;
			}

			return (TObject)info;
		}

		public bool TryGet<TObject>(Vector2Int position, TLayer layer, out TObject? obj)
			where TObject : IObject2D
		{
			obj = default;

			var reference = _grid.Get(layer, position);
			if (reference == null || reference == null)
			{
				return false;
			}

			if (reference is not TObject)
			{
				return false;
			}

			obj = (TObject)reference;
			if (obj == null)
			{
				return false;
			}
			return true;
		}

		public override GridRange GetRange(IActor2D actor)
		{
			return _grid.GetRange(actorLayer,
				actor,
				new GridSearchRangeArguments(0, actor.range)
				{
					traversableFunction = pos => CanTraverse(actor, pos)
				});
		}

		public override GridPath GetPath(IActor2D actor, Vector2Int position)
		{
			return _grid.SearchPath(actor.cellPosition, position);
		}
	}

	public abstract class Map2D<TData>
		: Map2D<TData, DefaultMapLayer>

		where TData : class
	{
		protected Map2D(Func<Grid> ctor) : base(ctor)
		{
		}

		public override DefaultMapLayer actorLayer => DefaultMapLayer.Actor;


		protected override TraversableStatus CanTraverse(IActor2D actor, Vector2Int pos)
		{
			if (!_grid.Contains(DefaultMapLayer.Terrain, pos))
			{
				//StratusLog.Info($"No terrain at {pos} ({_grid.Count(DefaultMapLayer.Terrain)})");
				return TraversableStatus.Invalid;
			}

			if (_grid.Contains(DefaultMapLayer.Wall, pos))
			{
				//StratusLog.Info($"Wall at {pos}");
				return TraversableStatus.Blocked;
			}

			if (_grid.Contains(DefaultMapLayer.Object, pos))
			{
				return TraversableStatus.Blocked;
			}

			if (_grid.Contains(DefaultMapLayer.Actor, pos))
			{
				//StratusLog.Info($"Actor already at {pos}");
				return TraversableStatus.Occupied;
			}

			return TraversableStatus.Valid;
		}
	}

	public abstract class TileMapReference
	{
		public abstract Vector2Int size { get; }
		public abstract int layerCount { get; }
	}

	public abstract class TileMapReference<T> : TileMapReference
		where T : class
	{
		public T tileMap { get; }

		protected TileMapReference(T tileMap)
		{
			this.tileMap = tileMap;
		}
	}
}
