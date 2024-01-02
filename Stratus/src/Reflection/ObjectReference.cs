using System;

namespace Stratus.Reflection
{
	public interface IObjectReference
	{
		Type type { get; } 
		object value { get; set; }
	}

	public class ObjectReference : IObjectReference
	{
		public Type type { get; private set; }
		public InferredType inferredType { get; private set; }

		public object value
		{
			get => get();
			set => set(value);
		}

		private Func<object> get;
		private Action<object> set;

		public ObjectReference(Type type, Func<object> get, Action<object> set)			
		{
			this.get = get;
			this.set = set;
			this.type = type;
			this.inferredType = type.Infer();
		}

		public static ObjectReference<bool> Boolean(Func<bool> get, Action<bool> set)
		{
			return new ObjectReference<bool>(get, set);
		}

		public static ObjectReference<TEnum> Enum<TEnum>(Func<TEnum> get, Action<TEnum> set)
			where TEnum : Enum
		{
			return new ObjectReference<TEnum>(get, set);
		}
	}

	public class ObjectReference<T> : ObjectReference
	{
		public new T value
		{
			get => (T)base.value;
			set => base.value = value;
		}

		public ObjectReference(Func<T> get, Action<T> set)
			: base(typeof(T), () => get(), v => set((T)v))
		{
		}
	}



	public static class ObjectReferenceExtensions
	{
		public static T GetValueOrDefault<T>(this ObjectReference<T> reference, T defauultValue = default)
		{
			return reference != null ? reference.value : defauultValue;
		}
	}
}