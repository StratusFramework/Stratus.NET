namespace Stratus.Utilities
{
	public static class StratusRandom
	{
		private static System.Random Generate() => new System.Random();

		public static int Range(int min, int max)
		{
			return Generate().Next(min, max);
		}

		public static float Range(float min, float max)
		{
			return Generate().Next((int)min, (int)max);
		}
	}
}