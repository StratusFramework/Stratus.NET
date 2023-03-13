using NUnit.Framework;

using Stratus.Inputs;

using System;
using System.Numerics;

namespace Stratus.Tests.Editor
{
	public class ActionMapHandlerTests
	{
		public enum MockInputActions
		{
			Console,
			Pan,
			Pause
		}

		public class MockInput
		{
		}

		public class MockInputActionMap : ActionMapHandler<MockInput>
		{
			public Action console;
			public Action<Vector2> pan;
			public Action pause;

			public override string name { get; }

			public override bool HandleInput(MockInput input)
			{
				throw new NotImplementedException();
			}

			public override bool TryBind(string name, object deleg)
			{
				if (deleg is Action action)
				{
					Bind(name, action);
					return true;
				}
				else if (deleg is Action<Vector2> vec2Action)
				{
					Bind(name, input => vec2Action(Vector2.Zero));
					return true;
				}
				return false;
			}
		}

		[Test]
		public void BindsActionsFromEnum()
		{
			var map = new MockInputActionMap();
			map.console = () => { };
			map.pan = (value) => { };
			map.pause = () => { };
			map.TryBindAll<MockInputActions>();

			Assert.AreEqual(3, map.count);
			Assert.True(map.Contains(nameof(MockInputActionMap.console)));
			Assert.True(map.Contains(nameof(MockInputActionMap.pan)));
			Assert.True(map.Contains(nameof(MockInputActionMap.pause)));
		}
	}
}