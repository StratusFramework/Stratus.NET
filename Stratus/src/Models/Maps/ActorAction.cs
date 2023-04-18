using Stratus.Events;
using Stratus.Numerics;

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

	#region Move
	public class MoveActorAction : ActorAction
	{
		public Vector2Int sourcePosition;
		public Vector2Int targetPosition;
		public GridRange range { get; }

		public MoveActorAction(IActor2D actor, GridRange range) : base(actor)
		{
			this.range = range;
		}
	}

	public class MoveActorEvent : ActionEvent<MoveActorAction>
	{
		public MoveActorEvent(MoveActorAction action) : base(action)
		{
		}
	} 
	#endregion

	public class WaitActorAction : ActorAction
	{
		public WaitActorAction(IActor2D actor) : base(actor)
		{
		}
	}
}
