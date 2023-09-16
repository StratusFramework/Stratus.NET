using Stratus.Logging;
using Stratus.Models.Maps.Actions;
using Stratus.Numerics;
using Stratus.Search;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps
{
	public interface ISpatialModel
	{
	}

	public interface IMap2D : ISpatialModel
	{
		IGrid2D grid { get; }
		GridRange GetRange(IActor2D actor);
		GridPath GetPath(Vector2Int origin, Vector2Int position);
		IEnumerable<ActorAction> GetActions(IActor2D actor);
	}

	public abstract class Map2D : IMap2D, IStratusLogger
	{
		public abstract IGrid2D grid { get; }
		public CellLayout cellLayout { get; protected set; }

		public abstract GridRange GetRange(IActor2D actor);
		public abstract GridPath GetPath(Vector2Int origin, Vector2Int position);
		public GridPath GetPath(IActor2D actor, Vector2Int position) => GetPath(actor.cellPosition, position);
		public abstract IEnumerable<ActorAction> GetActions(IActor2D actor);
	}

	public abstract class Map2D<TData, TLayer> : Map2D
		where TData : class
		where TLayer : Enum
	{
		public class Grid : Grid2D<TLayer>
		{
			public Grid(Bounds2D grid, CellLayout layout) : base(grid, layout)
			{
			}

			public Grid(Vector2Int size, CellLayout layout) : base(size, layout)
			{
			}
		}

		protected Grid2D _grid;
		public abstract TLayer actorLayer { get; }
		public override IGrid2D grid => _grid;

		public Map2D(Func<Grid> ctor)
		{
			_grid = ctor();
			cellLayout = _grid.cellLayout;
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

		public override GridPath GetPath(Vector2Int origin, Vector2Int position)
		{
			return _grid.SearchPath(origin, position);
		}

		public override IEnumerable<ActorAction> GetActions(IActor2D actor)
		{
			// Move
			GridRange range = GetRange(actor);
			if (range.valid)
			{
				yield return new MoveActorAction(actor, range);
			}

			// Wait
			yield return new WaitActorAction(actor);
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
			if (!grid.Contains(DefaultMapLayer.Terrain, pos))
			{
				return TraversableStatus.Invalid;
			}

			if (grid.Contains(DefaultMapLayer.Wall, pos))
			{
				return TraversableStatus.Blocked;
			}

			if (grid.Contains(DefaultMapLayer.Object, pos))
			{
				return TraversableStatus.Blocked;
			}

			if (grid.TryGet(DefaultMapLayer.Portal, pos, out IPortal2D portal))
			{
				return portal.open ? TraversableStatus.Valid : TraversableStatus.Blocked;
			}

			if (_grid.Contains(DefaultMapLayer.Actor, pos))
			{
				return TraversableStatus.Occupied;
			}

			return TraversableStatus.Valid;
		}

		public override IEnumerable<ActorAction> GetActions(IActor2D actor)
		{
			// Base
			foreach (var action in base.GetActions(actor))
			{
				yield return action;
			}

			// Range 1
			var offset_1 = actor.Offset(1, cellLayout);

			// Portal
			var portals = grid.GetAll<IPortal2D>(DefaultMapLayer.Portal, offset_1);
			if (portals.Any())
			{
				yield return new PortalActorAction(actor, portals);
			}

			// Attack
			var targets = grid.GetAll<IActor2D>(DefaultMapLayer.Actor, offset_1);
			if (targets.Any())
			{
				yield return new AttackActorAction(actor, targets);
			}
		}
	}
}
