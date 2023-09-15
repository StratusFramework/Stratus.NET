using Stratus.Collections;
using Stratus.Types;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stratus.Reflection
{
	/// <summary>
	/// Holds a reference to a given variable
	/// </summary>
	public class MemberReference
	{
		#region Properties
		/// <summary>
		/// The name for this mmeber
		/// </summary>
		public string name { get; private set; }
		/// <summary>
		/// The object instance on which this variable resides
		/// </summary>
		public object obj { get; private set; }
		/// <summary>
		/// The type of this member
		/// </summary>
		public Type type { get; private set; }
		/// <summary>
		/// The member information
		/// </summary>
		public MemberInfo member { get; }
		/// <summary>
		/// Information about the variable if it's a field type
		/// </summary>
		public FieldInfo field { get; }
		/// <summary>
		/// Information about the variable if it's a property type
		/// </summary>
		public PropertyInfo property { get; }
		/// <summary>
		/// The type of member, whether a field or property
		/// </summary>
		public MemberTypes memberType { get; private set; }
		/// <summary>
		/// Returns the current value of this member
		/// </summary>
		public object value
		{
			get => get();
			set => set(value);
		}
		private Func<object> get;
		private Action<object> set;
		/// <summary>
		/// Returns true if this member is an <see cref="ICollection"/>
		/// </summary>
		public bool isCollection { get; private set; }
		/// <summary>
		/// What type of <see cref="ICollection"/> this is
		/// </summary>
		public CollectionType collectionType => TypeUtility.Deduce(type);
		#endregion

		public MemberReference(FieldInfo field, object target)
		{
			this.field = field;
			this.member = field;
			this.type = field.FieldType;
			this.obj = target;
			this.name = field.Name;
			memberType = MemberTypes.Field;
			get = () => field.GetValue(target);
			set = value => field.SetValue(target, value);
			Reflect();
		}

		public MemberReference(PropertyInfo property, object target)
		{
			this.property = property;
			this.member = property;
			this.type = property.PropertyType;
			this.obj = target;
			this.name = property.Name;
			memberType = MemberTypes.Property;
			get = () =>
			{
				try
				{
					return property.GetValue(target);
				}
				catch (Exception ex)
				{
					throw new Exception($"Failed to get value from property {name} of object {target}", ex);
				}
			};
			set = value => property.SetValue(target, value);
			Reflect();
		}

		private void Reflect()
		{
			isCollection = TypeUtility.IsCollection(type);
		}

		/// <summary>
		/// Constructs a reference to the given member from a lambda expression capture
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static MemberReference Construct<T>(Expression<Func<T>> expression)
		{
			// Use expressions to find the underlying owner object
			var memberExpr = expression.Body as MemberExpression;
			var inst = memberExpr.Expression;
			var target = Expression.Lambda<Func<object>>(inst).Compile()();
			var variableName = memberExpr.Member.Name;

			// Construct the member reference object
			MemberReference memberReference = null; // new MemberReference();
													//memberReference.name = variableName;
													//memberReference.target = target;

			// Check if it's a property
			var property = target.GetType().GetProperty(variableName);
			if (property != null)
			{
				memberReference = new MemberReference(property, target);
				return memberReference;
			}

			// Check if it's a field
			var field = target.GetType().GetField(variableName);
			if (field != null)
			{
				memberReference = new MemberReference(field, target);
				return memberReference;
			}

			// Invalid
			throw new ArgumentException("The given variable is neither a property or a field!");
		}

		public object Get() => value;
		public T Get<T>() => (T)value;
		public void Set(object value) => this.value = value;
	}


}