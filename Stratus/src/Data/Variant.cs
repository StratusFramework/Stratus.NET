using System;
using System.Collections.Generic;
using System.Numerics;

namespace Stratus.Data
{
	public enum VariantType
	{
		Integer,
		Boolean,
		Float,
		String,
		Vector3
	}

	/// <summary>
	/// A Variant is a dynamic value type which represent a variety of types.
	/// It can be used in situtations where you need a common interface
	/// for your types to represent a variety of data.
	/// </summary>
	[Serializable]
	public struct Variant
	{
		#region Fields
		private VariantType _type;
		private int integerValue;
		private float floatValue;
		private bool booleanValue;
		private Vector3 vector3Value;
		private string stringValue;
		#endregion

		#region Properties
		public VariantType type => _type;
		#endregion

		#region Constructors
		public Variant(int value) : this()
		{
			SetInteger(value);
		}

		public Variant(float value) : this()
		{
			SetFloat(value);
		}

		public Variant(bool value) : this()
		{
			SetBoolean(value);
		}

		public Variant(string value) : this()
		{
			SetString(value);
		}

		public Variant(Vector3 value) : this()
		{
			SetVector3(value);
		}

		public Variant(Variant variant) : this()
		{
			_type = variant._type;
			Set(variant.Get());
		}
		#endregion

		#region Virtual
		public override string ToString()
		{
			switch (_type)
			{
				case VariantType.Integer:
					return integerValue.ToString();
				case VariantType.Float:
					return floatValue.ToString();
				case VariantType.Boolean:
					return booleanValue.ToString();
				case VariantType.String:
					return stringValue;
				case VariantType.Vector3:
					return vector3Value.ToString();
			}

			throw new NotSupportedException($"Unsupported type {type} used");
		} 
		#endregion

		#region Generic Accessors
		/// <summary>
		/// Gets the current value of this variant
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>()
		{
			var givenType = typeof(T);

			if (!Match(givenType))
			{
				throw new ArgumentException($"The provided type '{givenType.Name} is not the correct type for this value ({this._type.ToString()})");
			}

			object value = null;
			switch (this._type)
			{
				case VariantType.Integer:
					value = integerValue;
					break;
				case VariantType.Boolean:
					value = booleanValue;
					break;
				case VariantType.Float:
					value = floatValue;
					break;
				case VariantType.String:
					value = stringValue;
					break;
				case VariantType.Vector3:
					value = vector3Value;
					break;
			}

			return (T)Convert.ChangeType(value, typeof(T));
		}

		/// <summary>
		/// Gets the current value of this variant
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public object Get()
		{
			object value = null;
			switch (_type)
			{
				case VariantType.Integer:
					value = integerValue;
					break;
				case VariantType.Boolean:
					value = booleanValue;
					break;
				case VariantType.Float:
					value = floatValue;
					break;
				case VariantType.String:
					value = stringValue;
					break;
				case VariantType.Vector3:
					value = vector3Value;
					break;
			}
			return value;
		}

		/// <summary>
		/// Sets the current value of this variant, by deducing the given value type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public void Set(object value, bool strict = false)
		{
			var valueType = value.GetType();
			var variantType = VariantUtility.Convert(valueType);

			if (strict && variantType != _type)
			{
				throw new Exception($"The given type {variantType} does not match the current ({type}).");
			}

			switch (variantType)
			{
				case VariantType.Integer:
					SetInteger((int)value);
					break;
				case VariantType.Boolean:
					SetBoolean((bool)value);
					break;
				case VariantType.Float:
					SetFloat((float)value);
					break;
				case VariantType.String:
					SetString(value as string);
					break;
				case VariantType.Vector3:
					SetVector3((Vector3)value);
					break;
			}
		}
		#endregion

		#region Specific Accessors
		public void SetInteger(int value)
		{
			integerValue = value;
			_type = VariantType.Integer;
		}

		public int GetInteger()
		{
			if (_type != VariantType.Integer)
			{
				throw new ArgumentException("This variant has not been set as an integer type");
			}
			return integerValue;
		}

		public void SetFloat(float value)
		{
			floatValue = value;
			_type = VariantType.Float;
		}

		public float GetFloat()
		{
			if (_type != VariantType.Float)
			{
				throw new ArgumentException("This variant has not been set as a float type");
			}
			return floatValue;
		}

		public void SetString(string value)
		{
			stringValue = value;
			_type = VariantType.String;
		}

		public string GetString()
		{
			if (_type != VariantType.String)
			{
				throw new ArgumentException("This variant has not been set as a string type");
			}
			return stringValue;
		}

		public void SetBoolean(bool value)
		{
			booleanValue = value;
			_type = VariantType.Boolean;
		}

		public bool GetBool()
		{
			if (_type != VariantType.Boolean)
			{
				throw new ArgumentException("This variant has not been set as a boolean type");
			}
			return booleanValue;
		}

		public void SetVector3(Vector3 value)
		{
			if (_type != VariantType.Vector3)
			{
				throw new ArgumentException("This variant has not been set as a Vector3 type");
			}

			vector3Value = value;
			_type = VariantType.Vector3;
		}

		public Vector3 GetVector3()
		{
			if (_type != VariantType.Vector3)
			{
				throw new ArgumentException("This variant has not been set as a Vector3 type");
			}
			return vector3Value;
		}
		#endregion

		#region Comparison
		public bool Compare(Variant other)
		{
			if (this._type != other._type)
			{
				throw new Exception("Mismatching variants are being compared!");
			}

			switch (other._type)
			{
				case VariantType.Boolean:
					return this.booleanValue == other.booleanValue;
				case VariantType.Integer:
					return this.integerValue == other.integerValue;
				case VariantType.Float:
					return this.floatValue == other.floatValue;
				case VariantType.String:
					return this.stringValue == other.stringValue;
				case VariantType.Vector3:
					return this.vector3Value == other.vector3Value;
			}

			throw new NotSupportedException($"Unsupported type {other._type} used");
		}

		private bool Match(Type type)
		{
			VariantType givenVariantType = VariantUtility.Convert(type);
			return givenVariantType == this._type;
		}
		#endregion

		#region Static
		/// <summary>
		/// Constructs a variant based from a given value. It only accepts supported types,
		/// which are found in the Types enum.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Variant Make<T>(T value)
		{
			var type = typeof(T);

			if (type == typeof(int))
				return new Variant((int)(object)value);
			else if (type == typeof(float))
				return new Variant((float)(object)value);
			else if (type == typeof(bool))
				return new Variant((bool)(object)value);
			else if (type == typeof(string))
				return new Variant((string)(object)value);
			else if (type == typeof(Vector3))
				return new Variant((Vector3)(object)value);

			throw new Exception("Unsupported type being used (" + type.Name + ")");
		}
		#endregion
	}

	public static class VariantUtility
	{
		private static Dictionary<Type, VariantType> systemTypeToVariantType { get; } = new Dictionary<Type, VariantType>()
		{
			{typeof(int), VariantType.Integer},
			{typeof(bool), VariantType.Boolean},
			{typeof(float), VariantType.Float},
			{typeof(string), VariantType.String},
			{typeof(Vector3), VariantType.Vector3},
		};

		private static Dictionary<VariantType, Type> variantTypeToSystemType { get; } = new Dictionary<VariantType, Type>()
		{
			{VariantType.Integer, typeof(int)},
			{VariantType.Boolean, typeof(bool)},
			{VariantType.Float, typeof(float)},
			{VariantType.String, typeof(string)},
			{VariantType.Vector3, typeof(Vector3)},
		};

		public static Type Convert(this VariantType type) => variantTypeToSystemType[type];
		public static VariantType Convert(Type type) => systemTypeToVariantType[type];

	}
}