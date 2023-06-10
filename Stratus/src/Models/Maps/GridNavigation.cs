using Stratus.Events;
using Stratus.Numerics;

using System.Numerics;

namespace Stratus.Models.Maps
{
	public class NavigateGridEvent : Event
	{
		public Vector2Int direction { get; }

		public NavigateGridEvent(Vector2Int direction)
		{
			this.direction = direction;
		}

		public NavigateGridEvent(Vector2 direction)
		{
			this.direction = direction;
		}
	}
}
