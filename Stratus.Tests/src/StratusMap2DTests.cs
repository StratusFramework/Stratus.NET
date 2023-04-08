using NUnit.Framework;

using Stratus.Editor.Tests;
using Stratus.Extensions;
using Stratus.Models.Maps;
using Stratus.Numerics;

using System;
using System.Linq;

namespace Stratus.Models.Tests
{
	public class StratusMap2DTests : StratusTest
	{
		public abstract class MockObject : IObject2D
		{
			public MockObject(string name)
			{
				this.name = name;
			}

			public string name { get; }
			public Vector2Int cellPosition { get; }

			public override string ToString()
			{
				return name;
			}
		}

		public class MockTerrain : MockObject
		{
			public int traversalCost { get; set; }
			public int blocking { get; set; }

			public MockTerrain(string name, int cost = 5) : base(name)
			{
				this.traversalCost = cost;
			}
		}

		public class MockActor : MockObject
		{
			public int speed = 5;

			public MockActor(string name) : base(name)
			{
			}
		}

		public enum MockLayer
		{
			Terrain,
			Actor
		}

		public class MockMap : Grid2D<MockObject, MockLayer>
		{
			public MockMap(Vector2Int size) : base(new SquareGrid(size), CellLayout.Rectangle)
			{
			}

			//public override float GetTraversalCost(Vector2Int position)
			//{
			//	var terrain = Get<MockTerrain>(MockLayer.Terrain, position);
			//	if (terrain == null)
			//	{
			//		throw new Exception($"Terrain not set at {position}");
			//	}
			//	return terrain.traversalCost;
			//}
		}


		[Test]
		public void ConstructsMap()
		{
			// This creates a 3x3 map, starting from (0,0) and expanding outwards on X+, Y+
			int length = 3;
			Vector2Int size = new Vector2Int(length, length);
			MockMap map = new MockMap(size);

			for (int x = 0; x < length; x++)
			{
				for (int y = 0; y < length; y++)
				{
					AssertSuccess(map.ContainsCell(new Vector2Int(x, y)));
					if (x != 0 && y != 0)
					{
						AssertFailure(map.ContainsCell(new Vector2Int(-x, -y)));
						AssertFailure(map.ContainsCell(new Vector2Int(-x, y)));
						AssertFailure(map.ContainsCell(new Vector2Int(x, -y)));
					}
				}
			}
		}

		[Test]
		public void AddsObjectsToLayer()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));

			MockActor a = new MockActor("a");
			MockActor b = new MockActor("b");

			Vector2Int aStart = new Vector2Int(1, 1);
			Vector2Int bStart = new Vector2Int(2, 2);

			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));
			Assert.True(map.Contains(MockLayer.Actor, a));
			Assert.True(map.Contains(MockLayer.Actor, aStart));
			Assert.AreEqual(a, map.Get(MockLayer.Actor, aStart));

			AssertFailure(map.Set(MockLayer.Actor, b, aStart));
			AssertSuccess(map.Set(MockLayer.Actor, b, bStart));
			Assert.NotNull(map.Get(MockLayer.Actor, bStart));
			Assert.AreEqual(b, map.Get(MockLayer.Actor, bStart));
		}

		[Test]
		public void RemovesObjectFromLayer()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));
			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(1, 1);
			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));
			AssertSuccess(map.Remove(MockLayer.Actor, a));
			AssertFailure(map.Contains(MockLayer.Actor, a));
		}

		[Test]
		public void MovesObjectFromPreviousPosition()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));
			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(1, 1);
			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));
			Vector2Int aEnd = new Vector2Int(2, 2);
			AssertSuccess(map.Set(MockLayer.Actor, a, aEnd));
			Assert.False(map.Contains(MockLayer.Actor, aStart));
			Assert.True(map.Contains(MockLayer.Actor, aEnd));
		}

		[Test]
		public void GetCellsInRange()
		{
			MockMap map = DefaultMap();
			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(1, 1);

			// With the 3x3 grid and the agent at (1,1) with a speed of 1,
			// the available cells should be 5, its start position and its 4 neighbors:
			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));
			var aRange = map.GetRange(MockLayer.Actor, a, a.speed);
			Assert.AreEqual(5, aRange.Count);
			AssertContains(aStart, aRange);
			AssertContains(new Vector2Int(0, 1), aRange);
			AssertContains(new Vector2Int(2, 1), aRange);
			AssertContains(new Vector2Int(1, 2), aRange);
			AssertContains(new Vector2Int(1, 1), aRange);

			// If we place B at (2,2), it should only have 3 cells in range
			// since its in the top right corner of the map
			MockActor b = new MockActor("b");
			Vector2Int bStart = new Vector2Int(2, 2);
			AssertSuccess(map.Set(MockLayer.Actor, b, bStart));
			var bRange = map.GetRange(MockLayer.Actor, b, b.speed);
			Assert.AreEqual(3, bRange.Count);
			AssertContainsExactly(bRange, bStart, new Vector2Int(1, 2), new Vector2Int(2, 1));
		}

		[Test]
		public void FillsCells()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));
			map.Fill(MockLayer.Terrain, () => new MockTerrain("Grass"));
			Assert.AreEqual(9, map.Count(MockLayer.Terrain));
			AssertSuccess(map.ForEach(MockLayer.Terrain, (MockTerrain t) => t.name == "Grass"));
		}

		[Test]
		public void DoesNotAddObjectOfWrongType()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));
			map.Associate<MockTerrain>(MockLayer.Terrain);
			MockActor a = new MockActor("a");
			AssertFailure(map.Set(MockLayer.Terrain, a, new Vector2Int(0, 0)));
		}

		[Test]
		public void RangeExcludesCellsWithObjectsInCompetingLayers()
		{
			MockMap map = DefaultMap();

			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(1, 1);
			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));

			// Place B right next to A
			MockActor b = new MockActor("b");
			Vector2Int bStart = new Vector2Int(2, 1);
			AssertSuccess(map.Set(MockLayer.Actor, b, bStart));

			var aRange = map.GetRange(MockLayer.Actor, a, a.speed);
			AssertContainsExactly(aRange, 
				aStart, 
				new Vector2Int(0, 1), 
				new Vector2Int(1, 2), 
				new Vector2Int(1, 0));
		}

		[Test]
		public void GetsCellInRangeWithVariantCosts()
		{
			MockMap map = DefaultMap();

			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(1, 1);
			AssertSuccess(map.Set(MockLayer.Actor, a, aStart));
			Vector2Int badTerrainPos = new Vector2Int(2, 1);

			// Initial range should be 1 + 4 surrounding tiles
			var aRange = map.GetRange(MockLayer.Actor, a, a.speed);
			AssertContainsExactly(aRange,
				aStart,
				new Vector2Int(0, 1),
				badTerrainPos,
				new Vector2Int(1, 2),
				new Vector2Int(1, 0));

			// If we make (2,1) cost 6, it should be unavailable
			var terrain = map.Get<MockTerrain>(badTerrainPos);
			terrain.traversalCost = 10;
			aRange = map.GetRange(MockLayer.Actor, a, a.speed);
			AssertContainsExactly(aRange,
				aStart,
				new Vector2Int(0, 1),
				new Vector2Int(1, 2),
				new Vector2Int(1, 0));

			// If we move the agent to 0,0 and give them 15 speed,
			// they should cover all cells except (2,2)
			map.Set(MockLayer.Actor, a, new Vector2Int(0, 0));
			a.speed = 15;
			terrain.traversalCost = 5;
			aRange = map.GetRange(MockLayer.Actor, a, a.speed);
			Assert.AreEqual(8, aRange.Count, aRange.ToStringJoin());
			Assert.False(aRange.ContainsKey(new Vector2Int(2, 2)));
		}

		[Test]
		public void GetsObjectsInRange()
		{
			MockMap map = DefaultMap();

			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(0, 0);
			map.Set(MockLayer.Actor, a, aStart);

			MockActor b = new MockActor("b");
			Vector2Int bStart = new Vector2Int(1, 1);
			map.Set(MockLayer.Actor, b, bStart);

			// First, itself
			MockObject[] objectsInRange = map.GetObjectsInRange(a, new GridSearchRangeArguments(a.speed));
			AssertLength(1, objectsInRange);
			// Move B nex to A
			map.Set(b, new Vector2Int(1, 0));
			objectsInRange = map.GetObjectsInRange(a, new GridSearchRangeArguments(a.speed));
			AssertLength(2, objectsInRange);
		}

		[Test]
		public void FindsPathToPosition()
		{
			MockMap map = DefaultMap();

			MockActor a = new MockActor("a");
			Vector2Int aStart = new Vector2Int(0, 0);
			map.Set(MockLayer.Actor, a, aStart);

			Vector2Int aEnd = new Vector2Int(2, 0);
			var path = map.SearchPath(aStart, aEnd).ToHashSet();
			Assert.AreEqual(3, path.Count);
			AssertContains(path, aStart);
			AssertContains(path, new Vector2Int(1, 0));
			AssertContains(path, new Vector2Int(2, 0));

		}

		MockMap DefaultMap()
		{
			MockMap map = new MockMap(new Vector2Int(3, 3));
			map.Fill(MockLayer.Terrain, () => new MockTerrain("Grass", 5));
			map.Associate<MockTerrain>(MockLayer.Terrain);
			map.Associate<MockActor>(MockLayer.Actor);
			return map;
		}

	}
}
