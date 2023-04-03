using Stratus.Events;
using Stratus.Numerics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratus.Models.Maps
{
	public class Cursor
	{
	}

	public class SetCursorEvent : Event
	{
		public SetCursorEvent(Vector2Int position)
		{
			this.position = position;
		}

		public Vector2Int position { get; }
	}

	public class MoveCursorEvent : Event
	{
		public Vector2Int direction { get; }
		public GridRange range { get; }

		public MoveCursorEvent(Vector2Int direction)
		{
			this.direction = direction;
		}

		public MoveCursorEvent(Vector2Int direction, GridRange range) : this(direction)
		{
			this.range = range;
		}
	}

	public class CursorMovedEvent : Event
	{
		public Vector2Int position { get; }

		public CursorMovedEvent(Vector2Int position)
		{
			this.position = position;
		}
	}

	public class SelectCursorEvent : Event
	{
		public SelectCursorEvent()
		{
		}
	}
}
