using NUnit.Framework;

using Stratus.Models;
using Stratus.Search;

using System.Collections.Generic;

namespace Stratus.Editor.Tests
{
    public class StratusSearchTests
    {
        [Test]
        public void GetsNeighbors()
        {
			StratusVector3Int[] squareNeighbors(StratusVector3Int element)
            {
                List<StratusVector3Int> result = new List<StratusVector3Int>();
                result.Add(new StratusVector3Int(element.x + 1, element.y, element.z));
                result.Add(new StratusVector3Int(element.x - 1, element.y, element.z));
                result.Add(new StratusVector3Int(element.x, element.y + 1, element.z));
                result.Add(new StratusVector3Int(element.x, element.y - 1, element.z));
                return result.ToArray();
            }

			StratusVector3Int startElement = new StratusVector3Int(0, 0, 0);

            StratusSearch<StratusVector3Int>.RangeSearch search
                = new StratusSearch<StratusVector3Int>.RangeSearch()
                {
                    debug = true,
                    distanceFunction = StratusVector3Int.Distance,
                    neighborFunction = squareNeighbors,
                    range = 1,
                    startElement = startElement
                };

			StratusVector3Int[] actual = search.Search();
            Assert.AreEqual(4, actual.Length);
        }
    }
}