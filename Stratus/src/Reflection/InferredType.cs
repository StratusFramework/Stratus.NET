using Stratus.Collections;

using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;

namespace Stratus.Reflection
{
	/// <summary>
	/// The <see cref="Type"/> that was inferred and supported by the system
	/// </summary>
	/// <remarks>This provides a framework-supported inferral of <see cref="System"/> types that can be used in many places</remarks>
	public enum InferredType
	{
		/// <summary>
		/// Another type that isn't supported
		/// </summary>
		Unsupported,

		/// <summary>
		/// <see cref="System.Boolean"/>
		/// </summary>
		Boolean,
		/// <summary>
		/// <see cref="System.Int32"/>
		/// </summary>
		Integer,
		/// <summary>
		/// <see cref="float"/>
		/// </summary>
		Float,
		/// <summary>
		/// <see cref="System.String"/>
		/// </summary>
		String,
		/// <summary>
		/// <see cref="System.Enum"/>
		/// </summary>
		Enum,
		/// <summary>
		/// <see cref="System.Numerics.Vector2"/>
		/// </summary>
		Vector2,
		/// <summary>
		/// <see cref="System.Numerics.Vector3"/>
		/// </summary>
		Vector3,
		/// <summary>
		/// <see cref="System.Numerics.Vector4"/>
		/// </summary>
		Vector4,
		/// <summary>
		/// <see cref="System.Drawing.Color"/>
		/// </summary>
		Color
	}

	public static class InferredTypeExtensions
	{
		private static Bictionary<Type, InferredType> typeMap { get; set; }
			= new Bictionary<Type, InferredType>()
			{
				{ typeof(bool), InferredType.Boolean },
				{ typeof(int), InferredType.Integer},
				{ typeof(float), InferredType.Float },
				{ typeof(string), InferredType.String },
				{ typeof(Enum), InferredType.Enum},
				{ typeof(Vector2), InferredType.Vector2},
				{ typeof(Vector3), InferredType.Vector3},
				{ typeof(Vector4), InferredType.Vector4},
				{ typeof(Color), InferredType.Color},
			};

		public static Type ToType(this InferredType inferredType)
		{
			return inferredType switch
			{
				InferredType.Unsupported => throw new NotImplementedException(),
				_ => typeMap.reverse[inferredType]
			};
		}

		public static InferredType Infer(this Type type)
		{
			if (typeMap.forward.Contains(type))
			{
				return typeMap.forward[type];
			}
			else if (type.IsEnum)
			{
				return InferredType.Enum;
			}
			return InferredType.Unsupported;
		}

		public static InferredType Infer(FieldInfo field)
		{
			return field.FieldType.Infer();
		}
	}
}