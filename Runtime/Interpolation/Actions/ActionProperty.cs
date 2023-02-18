using Stratus.Interpolation;
using Stratus.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// A type of action that modifies the value of a given property over a specified amount of time, 
	/// using a specified interpolation formula(Ease).
	/// </summary>
	public abstract class ActionProperty : ActionBase
	{
		private static Lazy<Dictionary<Type, Type[]>> implementations
			= new Lazy<Dictionary<Type, Type[]>>(() =>
			StratusTypeUtility.TypeDefinitionParameterMap(typeof(ActionProperty<>)));

		protected StratusEase easeType { get; set; }
		public abstract float Interpolate(float dt);

		public ActionProperty(float duration, StratusEase ease)
		{
			this.duration = duration;
			this.easeType = ease;
		}

		public override float Update(float dt)
		{
			return this.Interpolate(dt);
		}

		public static Type GetImplementation(Type valueType)
		{
			var actionType = implementations.Value.GetValueOrDefault(valueType).First();
			return actionType;
		}

		/// <summary>
		/// Instantiates a property of the given type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="varExpr">An expression that provides a reference to an object. (Example: () => target.value)</param>
		/// <param name="value">The value to be set</param>
		/// <param name="duration">How long to set the value</param>
		/// <param name="ease">The interpolation algorithm to use</param>
		/// <returns></returns>
		public static ActionProperty Instantiate<T>(Expression<Func<T>> varExpr, T value, float duration, StratusEase ease)
		{
			MemberExpression memberExpr = varExpr.Body as MemberExpression;
			Expression inst = memberExpr.Expression;
			string variableName = memberExpr.Member.Name;
			object targetObj = Expression.Lambda<Func<object>>(inst).Compile()();

			// Construct an action then branch depending on whether the member to be
			// interpolated is a property or a field
			ActionProperty action = null;
			Type actionType;

			// Property
			PropertyInfo property = targetObj.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				Type propertyType = property.PropertyType;
				actionType = GetImplementation(propertyType);
			}
			// Field
			else
			{
				FieldInfo field = targetObj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
				Type fieldType = field.FieldType;
				actionType = GetImplementation(fieldType);
			}

			action = (ActionProperty)ObjectUtility.Instantiate(actionType, targetObj, property, value, duration, ease);
			return action;
		}
	}

	public abstract class ActionProperty<T> : ActionProperty
	{
		//----------------------------------------------------------------------/
		// Fields
		//----------------------------------------------------------------------/
		protected T difference;
		protected T initialValue;
		protected T endValue;
		private bool initialized = false;

		protected object target;
		protected PropertyInfo property;
		protected FieldInfo field;

		//----------------------------------------------------------------------/
		// CTOR
		//----------------------------------------------------------------------/
		public ActionProperty(object target, MemberInfo member, T endValue, float duration, StratusEase ease)
			: base(duration, ease)
		{
			this.target = target;
			if (member is FieldInfo field)
			{
				this.field = field;
			}
			else if (member is PropertyInfo property)
			{
				this.property = property;
			}
			this.endValue = endValue;
			base.duration = duration;
			this.easeType = ease;
		}

		/// <summary>
		/// Interpolates the given Property/Field.
		/// </summary>
		/// <param name="dt">The current delta time.</param>
		/// <returns></returns>
		public override float Interpolate(float dt)
		{
			if (!this.initialized)
			{
				this.Initialize();
			}

			this.elapsed += dt;
			float timeLeft = this.duration - this.elapsed;

			// If done updating
			if ((timeLeft + float.Epsilon) < dt)
			{
				this.isFinished = true;
				this.SetLast();
				return dt;
			}

			this.SetCurrent();
			return timeLeft;
		}

		/// <summary>
		/// Gets the initial value for the property. This is done separately
		/// because we want to capture the value at the time this action is beinig
		/// executed, not when it was created!
		/// </summary>
		public void Initialize()
		{
			if (this.property != null)
			{
				this.initialValue = (T)this.property.GetValue(this.target, null);
			}
			else if (this.field != null)
			{
				this.initialValue = (T)this.field.GetValue(this.target);
			}
			else
			{
				throw new System.Exception("Couldn't set initial value!");
			}

			// Now we can compute the difference
			this.ComputeDifference();

			if (logging)
			{
				StratusLog.Info("InitialValue = '" + this.initialValue
								+ "', EndValue = '" + this.endValue + "'"
								+ "' Difference = '" + this.difference + "'");
			}

			this.initialized = true;
		}

		/// <summary>
		/// Sets the current value for the property.
		/// </summary>
		public virtual void SetCurrent()
		{
			float easeVal = this.easeType.Evaluate(this.elapsed / this.duration);
			T currentValue = this.ComputeCurrentValue((easeVal));
			if (logging)
			{
				StratusLog.Info("CurrentValue = '" + currentValue + "'");
			}

			this.Set(currentValue);
		}

		/// <summary>
		/// Sets the last value for the property.
		/// </summary>
		public virtual void SetLast()
		{
			this.Set(this.endValue);
		}

		/// <summary>
		/// Sets the value for the property.
		/// </summary>
		/// <param name="val"></param>
		public void Set(T val)
		{
			if (this.property != null)
			{
				this.property.SetValue(this.target, val, null);
			}
			else
			{
				this.field.SetValue(this.target, val);
			}
		}

		public abstract void ComputeDifference();
		public abstract T ComputeCurrentValue(float easeVal);
	}
}
