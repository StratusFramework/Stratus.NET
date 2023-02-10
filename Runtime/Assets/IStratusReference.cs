using Stratus.Extensions;

using System;

//using UnityEngine;

namespace Stratus
{
	public interface IStratusReference<T> where T : class
	{
		T value { get; }
	}

	/// <summary>
	/// Serializes a reference to an instantiable type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class StratusInstanceReference<T> : IStratusNamed, IStratusReference<T>
		where T : class
	{
		private static readonly StratusTypeInstancer<T> instancer
			= new StratusTypeInstancer<T>();

		//[SerializeField]
		private string _name;

		public string name
		{
			get => _name;
			set => _name = value;
		}

		public T value => instancer.Get(name);
		public bool valid => _name.IsValid();

		public static implicit operator T(StratusInstanceReference<T> reference) => reference.value;
		public static implicit operator StratusInstanceReference<T>(T value) => new StratusInstanceReference<T>(value);

		public StratusInstanceReference()
		{
		}

		public StratusInstanceReference(T value)
		{
			Set(value);
		}

		public void Set(T value)
		{
			_name = value.GetType().Name;
		}

		public void Set<U>() where U : T
		{
			_name = typeof(U).Name;
		}
	}

}