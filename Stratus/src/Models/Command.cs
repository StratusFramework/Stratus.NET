using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models
{
	public interface ICommand
	{
		Result Execute();
		Result Revert();
	}

	public abstract class Command : ICommand
	{
		public virtual string name => GetType().Name.Replace(nameof(Command), string.Empty);
		public bool executed { get; private set; }

		public event Action onExecute;
		public event Action onRevert;

		protected abstract Result OnExecute();
		protected abstract Result OnRevert();

		public Result Execute()
		{
			if (executed)
			{
				return false;
			}

			var result = OnExecute();
			if (result)
			{
				onExecute?.Invoke();
			}
			return result;
		}

		public Result Revert()
		{
			if (!executed)
			{
				return false;
			}

			var result = OnRevert();
			if (result)
			{
				onRevert?.Invoke();
			}
			return result;
		}
	}

	public abstract class Command<T> : Command
	{
		public T target { get; }

		protected Command(T target)
		{
			this.target = target;
		}
	}

	public class CommandStack
	{
		private Stack<ICommand> executed = new();
		private Stack<ICommand> reverted = new();

		public int count => executed.Count;

		public event Action<ICommand> onExecuted;
		public event Action<ICommand> onReverted;

		public Result Execute(ICommand command)
		{
			var result = command.Execute();
			if (result)
			{
				executed.Push(command);
				onExecuted?.Invoke(command);
			}
			return result;
		}

		public Result Redo() 
		{
			if (reverted.Count == 0)
			{
				return false;
			}

			var command = reverted.Pop();
			return Execute(command);
		}

		public Result Undo()
		{
			if (executed.Count == 0)
			{
				return false;
			}

			var command = executed.Pop();
			var revert = command.Revert();
			if (revert)
			{
				reverted.Push(command);
				onReverted?.Invoke(command);
			}
			return revert;
		}

	}

	public abstract record CommandEvent<TCommand> : Events.Event
		where TCommand : Command
	{
		public TCommand command { get; }

		protected CommandEvent(TCommand command)
		{
			this.command = command;
		}

		public override string ToString()
		{
			return command.ToString();
		}

	}
}
