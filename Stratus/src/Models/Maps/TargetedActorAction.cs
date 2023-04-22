using Stratus.Numerics;

using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps.Actions
{
	public class TargetedActorAction : ActorAction
	{
		public Vector2Int[] targetCells { get; }

		public TargetedActorAction(IActor2D actor, IEnumerable<Vector2Int> cells) : base(actor)
		{
			this.targetCells = cells.ToArray();
		}
	}

	public interface ITargetedAction
	{
		IObject2D[] targets { get; }
	}

	public class TargetedActorAction<TObject> : ActorAction, ITargetedAction
	{
		public TObject[] targets { get; }
		IObject2D[] ITargetedAction.targets => targets.Cast<IObject2D>().ToArray();

		public TargetedActorAction(IActor2D actor, IEnumerable<TObject> targets) : base(actor)
		{
			this.targets = targets.ToArray();
		}
	}

	public class AttackActorAction : TargetedActorAction<IActor2D>
	{
		public AttackActorAction(IActor2D actor, IEnumerable<IActor2D> targets) : base(actor, targets)
		{
		}
	}

	public class PortalActorAction : TargetedActorAction<IPortal2D>
	{
		public PortalActorAction(IActor2D actor, IEnumerable<IPortal2D> targets) : base(actor, targets)
		{
		}
	}
}
