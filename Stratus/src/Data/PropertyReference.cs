using System;

namespace Stratus.Data
{
	public class PropertyReference<T>
	{
		public T value
		{
			get =>  get();
			set => set(value);
		}

		private Func<T> get;
		private Action<T> set;

		public PropertyReference(Func<T> get, Action<T> set)
		{
			this.get = get;
			this.set = set;
		}
	}

	public static class PropertyReferenceExtension
	{
		public static T GetValueOrDefault<T>(this PropertyReference<T> reference, T defauultValue = default)
		{
			return reference != null ? reference.value : defauultValue;
		}
	}
}