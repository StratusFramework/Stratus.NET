using Stratus.Extensions;
using Stratus.Logging;
using Stratus.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stratus.Reflection
{
	/// <summary>
	/// Edits System.Object types in a completely generic way
	/// </summary>
	public class SerializedObject : IStratusLogger
	{
		#region Properties
		public Type type { get; private set; }
		public object target { get; private set; }
		public SerializedField[] fields { get; private set; }
		public Dictionary<string, SerializedField> fieldsByname { get; private set; } = new Dictionary<string, SerializedField>();
		public bool debug { get; set; }
		#endregion

		#region Constructors
		public SerializedObject(object target)
		{
			this.target = target;
			this.type = target.GetType();
			this.GenerateFields();
		} 
		#endregion

		public override string ToString()
		{
			return $"({type.GetNiceName()}) {target}";
		}

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		public string Serialize() => JsonSerializationUtility.Serialize(this.target);

		public void Deserialize(string data)
		{
			target = JsonSerializationUtility.Deserialize(data, type);
		}

		public string PrintHierarchy()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(ToString()); foreach (var field in fields)
			{
				sb.Append(field.PrintHierarchy(1));
			}
			return sb.ToString();
		}

		//------------------------------------------------------------------------/
		// Procedures
		//------------------------------------------------------------------------/
		private void GenerateFields()
		{
			FieldInfo[] fields = this.type.GetSerializedFields();
			List<SerializedField> serializedFields = new List<SerializedField>();

			// Backwards since we want the top-most declared classes first
			foreach (FieldInfo field in fields.Reverse())
			{
				SerializedField property = new SerializedField(field, this.target);
				this.fieldsByname.Add(property.name, property);
				serializedFields.Add(property);
			}

			this.fields = serializedFields.ToArray();
		}

		public static bool IsList(object o)
		{
			if (o == null)
			{
				return false;
			}

			return o is IList &&
			 o.GetType().IsGenericType &&
			 o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}

		public static bool IsList(Type type)
		{
			return type.IsGenericType &&
				   type.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}

		public static bool IsArray(Type type)
		{
			return typeof(IList).IsAssignableFrom(type);
		}

		public static bool IsDictionary(object o)
		{
			if (o == null)
			{
				return false;
			}

			return o is IDictionary &&
			 o.GetType().IsGenericType &&
			 o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
		}
	}

}