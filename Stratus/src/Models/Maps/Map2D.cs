using Stratus.Logging;
using Stratus.Numerics;
using Stratus.Search;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models.Maps
{
	public interface IMap2D
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
	public abstract class Map2D<TMap, TObject, TData, TLayer> : Map2D
		where TMap : class
		where TObject : class, IObject2D
		where TData : class
		where TLayer : Enum
	{
		protected Grid2D<CellReference<TObject, TData>, TLayer> _grid;

		public abstract TLayer actorLayer { get; }
		public override IGrid2D grid => _grid;
		public TMap tileMap { get; private set; }

		public Map2D(TMap tileMap)
		{
			this.tileMap = tileMap;
			Initialize();
		}

		public abstract void Initialize();
		protected abstract TraversableStatus CanTraverse(IActor2D actor, Vector2Int pos);

		public TNode? Get<TNode>(Vector2Int position, TLayer layer)
			where TNode : TObject
		{
			var info = _grid.Get(layer, position);
			if (info == null)
			{
				return default;
			}
			return (TNode)info.actor;
		}

		public bool TryGet<UObject>(Vector2Int position, TLayer layer, out UObject? obj)
			where UObject : TObject
		{
			obj = default;

			var reference = _grid.Get(layer, position);
			if (reference == null || reference.actor == null)
			{
				return false;
			}

			obj = (UObject)reference.actor;
			if (obj == null)
			{
				return false;
			}
			return true;
		}

		public override GridRange GetRange(IActor2D actor)
		{
			return _grid.GetRange(actorLayer,
				new CellReference<TObject, TData>((TObject)actor),
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

	public abstract class Map2D<TMap, TObject, TData>
		: Map2D<TMap, TObject, TData, DefaultMapLayer>

		where TMap : class
		where TObject : class, IObject2D
		where TData : class
	{
		public override DefaultMapLayer actorLayer => DefaultMapLayer.Actor;

		protected Map2D(TMap tileMap) : base(tileMap)
		{
		}

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
