using Stratus.Events;
using Stratus.Logging;

using System;

namespace Stratus.Inputs
{
	public interface IStratusInputLayerProvider
	{
		bool pushInputLayer { get; }
	}

	public abstract class InputLayer : IStratusLogger
	{
		#region Declarations
		public class PushEvent : Event
		{
			public InputLayer layer;

			public PushEvent(InputLayer layer)
			{
				this.layer = layer;
			}
		}

		public class PopEvent : Event
		{
			public InputLayer layer;

			public PopEvent(InputLayer layer)
			{
				this.layer = layer;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// An identifier for this input layer
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Whether this input layer blocks the pushes of additional input layers until its been popped
		/// </summary>
		public bool blocking { get; set; }
		/// <summary>
		/// Whether this layer should ignore being blocked by an existing blocking layer
		/// </summary>
		public bool ignoreBlocking { get; set; }
		/// <summary>
		/// Whether this input layer has been made active
		/// </summary>
		public bool active
		{
			get => _active;
			internal set
			{
				if (value != _active)
				{
					_active = value;
					onActive?.Invoke(value);
					OnToggle(value);
				}
			}
		}
		private bool _active;
		#endregion

		#region Events
		/// <summary>
		/// Invoked whenever this input layer is (de)activated
		/// </summary>
		public event Action<bool> onPushed;
		/// <summary>
		/// Invoked whenever this layer has been made active
		/// </summary>
		public event Action<bool> onActive;
		#endregion

		#region Virtual
		public override string ToString() => name;
		protected abstract void OnToggle(bool enabled);
		public abstract bool HandleInput(object input);
		#endregion

		#region Constructors
		public InputLayer(string name)
		{
			this.name = name;
		}
		#endregion
	}

	public class BlockingInputLayer : InputLayer
	{
		public BlockingInputLayer(string name) : base(name)
		{
		}

		public override bool HandleInput(object input)
		{
			return true;
		}

		protected override void OnToggle(bool enabled)
		{
		}
	}

	public abstract class InputLayer<TInput> : InputLayer
	{
		protected InputLayer(string name) : base(name)
		{
		}

		public override bool HandleInput(object input)
		{
			return HandleInput((TInput)input);
		}

		public abstract bool HandleInput(TInput input);
	}
	

	public abstract class InputLayer<TInput, TActionMap> : InputLayer<TInput>
		where TActionMap : IActionMapHandler, new()
	{
		public TActionMap map { get; }

		public InputLayer(string name, TActionMap map) : base(name)
		{
			this.map = map;
		}

		public InputLayer() : this(typeof(TActionMap).Name, new TActionMap())
		{
		}

		public InputLayer(string name) : this(name, new TActionMap())
		{
		}


		public override bool HandleInput(object context)
		{
			return map.HandleInput(context);
		}

		protected override void OnToggle(bool enabled)
		{
		}
	}
}
