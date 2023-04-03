using Stratus.Events;
using Stratus.Numerics;

using System;
using System.Numerics;

namespace Stratus.Models.Maps
{
	public interface IObject2D
	{
		/// <summary>
		/// The name of the object
		/// </summary>
		string name { get; }
		/// <summary>
		/// The current position on the grid
		/// </summary>
		Vector2Int cellPosition { get; }
	}

	public interface IActor2D : IObject2D
	{
		/// <summary>
		/// How much the actor can change its position.
		/// This could be its movement speed, etc...
		/// </summary>
		int range { get; }
		/// <summary>
		/// Invoked whenever the actor's position changes
		/// </summary>
		event Action<Vector2Int> onMoved;
	}

	

	public abstract class Actor2DCommand : Command
	{
		public IActor2D actor { get; }

		public Actor2DCommand(IActor2D actor)
		{
			this.actor = actor;
		}

		public TActor As<TActor>() where TActor : IActor2D => (TActor)actor;

	}

	public class MovementRangeEvent : RequestResponseEvent<IActor2D, GridRange>
	{
	}
}

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

	public abstract class ActionEvent<TAction> : Event
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




}
