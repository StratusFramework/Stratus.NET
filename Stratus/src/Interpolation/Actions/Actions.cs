using System;
using System.Collections;
using System.Drawing;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace Stratus.Interpolation
{
	public static class Actions
    {
		/// <summary>
		/// Adds a property change to the action set.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set">A reference to the set.</param>
		/// <param name="varExpr">A lambda expression encapsulating a reference to the property which will be modified</param>
		/// <param name="value">The new value for the property</param>
		/// <param name="duration">Over how long should the property be changed</param>
		/// <param name="ease">What interpolation algorithm to use</param>
		public static void Property<T>(ActionSet set, Expression<Func<T>> varExpr, T value, float duration, Ease ease)
		{
			MemberExpression memberExpr = varExpr.Body as MemberExpression;
			Expression inst = memberExpr.Expression;
			string variableName = memberExpr.Member.Name;
			object targetObj = Expression.Lambda<Func<object>>(inst).Compile()();

			// Construct an action then branch depending on whether the member to be interpolated is a property or a field
			ActionBase action = null;

			// Property
			PropertyInfo property = targetObj.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				Type propertyType = property.PropertyType;

				if (propertyType == typeof(float))
				{
					action = new ActionPropertyFloat(targetObj, property, Convert.ToSingle(value), duration, ease);
				}
				else if (propertyType == typeof(int))
				{
					action = new ActionPropertyInteger(targetObj, property, Convert.ToInt32(value), duration, ease);
				}
				else if (propertyType == typeof(bool))
				{
					action = new ActionPropertyBoolean(targetObj, property, Convert.ToBoolean(value), duration, ease);
				}
				else if (propertyType == typeof(Vector2))
				{
					action = new ActionPropertyVector2(targetObj, property, (System.Numerics.Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
				}
				else if (propertyType == typeof(Vector3))
				{
					action = new ActionPropertyVector3(targetObj, property, (System.Numerics.Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
				}
				else if (propertyType == typeof(Vector4))
				{
					action = new ActionPropertyVector4(targetObj, property, (System.Numerics.Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
				}
				else
				{
					StratusLog.Info("Couldn't find the property!");
				}
			}
			// Field
			else
			{
				FieldInfo field = targetObj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
				Type fieldType = field.FieldType;

				if (fieldType == typeof(float))
				{
					action = new ActionPropertyFloat(targetObj, field, Convert.ToSingle(value), duration, ease);
				}
				else if (fieldType == typeof(int))
				{
					action = new ActionPropertyInteger(targetObj, field, Convert.ToInt32(value), duration, ease);
				}
				else if (fieldType == typeof(bool))
				{
					action = new ActionPropertyBoolean(targetObj, field, Convert.ToBoolean(value), duration, ease);
				}
				else if (fieldType == typeof(Vector2))
				{
					action = new ActionPropertyVector2(targetObj, field, (System.Numerics.Vector2)Convert.ChangeType(value, typeof(Vector2)), duration, ease);
				}
				else if (fieldType == typeof(Vector3))
				{
					action = new ActionPropertyVector3(targetObj, field, (System.Numerics.Vector3)Convert.ChangeType(value, typeof(Vector3)), duration, ease);
				}
				else if (fieldType == typeof(Vector4))
				{
					action = new ActionPropertyVector4(targetObj, field, (System.Numerics.Vector4)Convert.ChangeType(value, typeof(Vector4)), duration, ease);
				}
				else
				{
					StratusLog.Info("Couldn't find the field!");
				}
			}
			// Now add it!
			set.Add(action);
		}
	}
}
