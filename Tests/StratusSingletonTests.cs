using Stratus.Utilities;

namespace Stratus.Tests
{
	public class StratusSingletonTests
	{
		internal abstract class MockClass
		{
			public int a;
			protected abstract int GetA();
		}

		internal class MockClassSingleton : StratusSingleton<MockClass>
		{
			protected override void OnInitialize()
			{
			}
		}
	}
}