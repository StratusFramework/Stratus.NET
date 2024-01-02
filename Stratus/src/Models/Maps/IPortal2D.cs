using Stratus.Numerics;
using Stratus.Reflection;

using System;

namespace Stratus.Models.Maps
{
	/// <summary>
	/// A portal could be a door, gate, etc...
	/// </summary>
	public interface IPortal2D : IObject2D
	{
		bool open { get; set; }
		void Toggle();
		event Action<bool> onToggle;
	}

	public enum PortalState
	{
		Open,
		Closed
	}

	public class Portal2D : Object2D, IPortal2D
	{
		public bool open
		{
			get => _open.value;
			set
			{
				_open.value = value;
				onToggle?.Invoke(value);
			}

		}
		private ObjectReference<bool> _open;

		public event Action<bool> onToggle;

		public PortalState state => open ? PortalState.Open : PortalState.Closed;

		public Portal2D(string name, Enumerated layer, Vector2Int cellPosition,
			ObjectReference<bool> open)
			: base(name, layer, cellPosition)
		{
			_open = open;
		}

		public void Toggle()
		{
			open = !open;
		}

	}
}