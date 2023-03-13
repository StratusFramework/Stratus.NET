using Stratus;
using Stratus.Events;
using Stratus.Extensions;
using Stratus.Interpolation;
using Stratus.Logging;
using Stratus.Reflection;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
		/// Whether this input layer is currently pushed. 
		/// If this layer has been made the topmost: if it's enabled it will be made active,
		/// otherwise inactive and will be removed.
		/// </summary>
		public bool pushed { get; set; }
		/// <summary>
		/// Whether this input layer has been made active
		/// </summary>
		public bool active
		{
			get => _active;
			set
			{
				if (value != _active)
				{
					_active = value;
					this.Log($"{this} has been made {(value ? "active" : "inactive")}");
					onActive?.Invoke(value);
					OnActive(value);
				}
			}
		}
		private bool _active;
		/// <summary>
		/// The input action map this layer is for
		/// </summary>
		public abstract string map { get; }
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
		public override string ToString() => $"{name} ({map})";
		protected abstract void OnActive(bool enabled);
		public abstract bool HandleInput(object input);
		#endregion

		#region Constructors
		public InputLayer(string name)
		{
			this.name = name;
		}
		#endregion

		#region Interface
		/// <summary>
		/// Activates this input layer by sending a scene-level event that will be received by 
		/// any applicable Stratus Player Input behaviour
		/// </summary>
		public void PushByEvent()
		{
			if (pushed)
			{
				this.LogWarning($"Layer {this} already enabled");
				return;
			}
			pushed = true;
			onPushed?.Invoke(true);
			PushEvent e = new PushEvent(this);
		}

		/// <summary>
		/// Deactivates this input layer by sending a scene-level event that will be received by 
		/// any applicable Stratus Player Input behaviour.
		/// If this input layer is not at the top, it will be popped from the input stack
		/// when the ones above it have been.
		/// </summary>
		public void PopByEvent()
		{
			if (!pushed)
			{
				StratusLog.Error($"Cannot pop disabled layer {name}");
				return;
			}
			pushed = false;
			onPushed?.Invoke(false);
			PopEvent e = new PopEvent();
		}

		/// <summary>
		/// Activates/deactivates the input layer
		/// </summary>
		/// <param name="toggle"></param>
		public void ToggleByEvent(bool toggle)
		{
			if (toggle)
			{
				PushByEvent();
			}
			else
			{
				PopByEvent();
			}
		}
		#endregion
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
		public TActionMap actions { get; } = new TActionMap();
		public override string map => actions.name;

		public InputLayer() : this(typeof(TActionMap).Name, new TActionMap())
		{
		}

		public InputLayer(string label) : this(label, new TActionMap())
		{
		}

		public InputLayer(string label, TActionMap actions) : base(label)
		{
			this.actions = actions;
		}

		public override bool HandleInput(object context)
		{
			return actions.HandleInput(context);
		}

		protected override void OnActive(bool enabled)
		{
		}
	}
}
