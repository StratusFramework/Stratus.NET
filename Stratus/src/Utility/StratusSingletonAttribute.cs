using System;

namespace Stratus.Utilities
{
	/// <summary>
	/// An optional attribute for Stratus singletons, offering more control over its initial setup.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class StratusSingletonAttribute : Attribute
	{
		/// <summary>
		/// Whether the class should only be instantiated during playmode
		/// </summary>
		public bool isPlayerOnly { get; set; } = true;
		/// <summary>
		/// If instantiated, the name of the GameObject that will contain the singleton
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Whether to instantiate an instance of the class if one is not found present at runtime
		/// </summary>
		public bool instantiate { get; set; }
		/// <summary>
		/// Whether the instance is persistent across scene loading
		/// </summary>
		public bool persistent { get; set; }

		/// <param name="name">The name of the GameObject where the singleton will be placed</param>
		/// <param name="persistent">Whether the instance is persistent across scene loading</param>
		/// <param name="instantiate">Whether to instantiate an instance of the class if one is not found present at runtime</param>
		public StratusSingletonAttribute(string name, bool persistent = true, bool instantiate = true)
		{
			this.name = name;
			this.persistent = persistent;
			this.instantiate = instantiate;
		}

		public StratusSingletonAttribute()
		{
			this.instantiate = true;
			this.persistent = true;
		}
	}
}