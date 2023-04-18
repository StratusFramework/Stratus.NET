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

	public class TargetedActorAction<TObject> : ActorAction
	{
		public TObject[] targets { get; }

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
