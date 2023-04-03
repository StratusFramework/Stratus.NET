using Stratus.Events;
using Stratus.Numerics;

using System.Collections.Generic;
using System.Linq;

namespace Stratus.Models.Maps.Actions
{
	public abstract class ActorAction
	{
		public IActor2D actor { get; }
		public virtual string name => GetType().Name.Replace(nameof(ActorAction), string.Empty);

		public ActorAction(IActor2D actor)
		{
			this.actor = actor;
		}

		public TActor As<TActor>() where TActor : IActor2D => (TActor)actor;
	}

	public class ActionEvent<TAction> : Event
		where TAction : ActorAction
	{
		public TAction action { get; }

		protected ActionEvent(TAction action)
		{
			this.action = action;
		}

		public override string ToString()
		{
			return action.ToString();
		}
	}

	public class PreviewActionEvent : ActionEvent<ActorAction>
	{
		public PreviewActionEvent(ActorAction action) : base(action)
		{
		}
	}

	public class CancelPreviewActionEvent : ActionEvent<ActorAction>
	{
		public CancelPreviewActionEvent(ActorAction action) : base(action)
		{
		}
	}

	public class MoveActorAction : ActorAction
	{
		public Vector2Int sourcePosition;
		public Vector2Int targetPosition;

		public MoveActorAction(IActor2D actor) : base(actor)
		{
		}
	}

	public class MovementRangeEvent : RequestResponseEvent<IActor2D, GridRange>
	{
	}

	public class MoveActorEvent : ActionEvent<MoveActorAction>
	{
		public MoveActorEvent(MoveActorAction action) : base(action)
		{
		}
	}

	public class WaitActorAction : ActorAction
	{
		public WaitActorAction(IActor2D actor) : base(actor)
		{
		}
	}

	public class TargetedActorAction : ActorAction
	{
		public Vector2Int[] offset { get; private set; }
		public Vector2Int[] targetCells => offset.Select(o => o + actor.cellPosition).ToArray();

		private static readonly Vector2Int[] squareOffsetTemplate
			= new Vector2Int[]
			{
				new Vector2Int(-1,0),
				new Vector2Int(1,0),
				new Vector2Int(0,1),
				new Vector2Int(0,-1),
			};

		public TargetedActorAction(IActor2D actor) : base(actor)
		{
		}

		/// <summary>
		/// The target area is a square around the actor
		/// </summary>
		/// <param name="range">The range of the square</param>
		public void SetSquareTarget(int range = 1)
		{
			List<Vector2Int> result = new List<Vector2Int>();
			for (int n = 1; n <= range; ++n)
			{
				foreach (var t in squareOffsetTemplate)
				{
					result.Add(t * n);
				}
			}
			offset = result.ToArray();
		}
	}


}
