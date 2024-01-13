using Stratus.Types;

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stratus.Interpolation
{
	/// <summary>
	/// A type of action that modifies the value of a given property over a specified amount of time, 
	/// using a specified interpolation formula(Ease).
	/// </summary>
	public abstract class ActionProperty : ActionBase
	{
		public static ImplementationTypeInstancer<ActionProperty> implementations = new ImplementationTypeInstancer<ActionProperty>(typeof(ActionProperty<>));

		protected Ease easeType { get; set; }
		public abstract float Interpolate(float dt);

		public ActionProperty(float duration, Ease ease)
		{
			this.duration = duration;
			this.easeType = ease;
		}

		public override float Update(float dt)
		{
			return this.Interpolate(dt);
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
		public static ActionProperty Construct<T>(Expression<Func<T>> varExpr, T value, float duration, Ease ease)
		{
			MemberExpression memberExpr = varExpr.Body as MemberExpression;
			Expression inst = memberExpr.Expression;
			string variableName = memberExpr.Member.Name;
			object targetObj = Expression.Lambda<Func<object>>(inst).Compile()();

			// Construct an action then branch depending on whether the member to be interpolated is a property or a field
			ActionProperty action = null;
			Type actionType;

			// Property
			PropertyInfo property = targetObj.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
			if (property != null)
			{
				Type propertyType = property.PropertyType;
				actionType = implementations.Resolve(propertyType);
			}
			// Field
			else
			{
				FieldInfo field = targetObj.GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
				Type fieldType = field.FieldType;
				actionType = implementations.Resolve(fieldType);
			}

			action = implementations.Instantiate(actionType, targetObj, property, value, duration, ease);
			if (action == null)
			{
				throw new NotImplementedException($"No implementation of {nameof(ActionProperty<T>)} was found");
			}
			return action;
		}
	}

	public static class ActionPropertyExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="set"></param>
		/// <param name="varExpr"></param>
		/// <param name="value"></param>
		/// <param name="duration"></param>
		/// <param name="ease"></param>
		/// <returns></returns>
		/// <remarks>An implementation of <see cref="ActionProperty{T}"/> will be needed!</remarks>
		public static ActionSet Property<T>(this ActionSet set, Expression<Func<T>> varExpr, T value, float duration, Ease ease)
		{
			ActionProperty action = ActionProperty.Construct<T>(varExpr, value, duration, ease);
			set.Add(action);
			return set;
		}
	}

	public abstract class ActionProperty<T> : ActionProperty
	{
		#region Fields
		protected T difference;
		protected T initialValue;
		protected T endValue;
		private bool initialized = false;

		protected object target;
		protected PropertyInfo property;
		protected FieldInfo field;
		#endregion

		#region Constructors
		public ActionProperty(object target, MemberInfo member, T endValue, float duration, Ease ease)
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
		#endregion

		#region Virtual
		public abstract void ComputeDifference();
		public abstract T ComputeCurrentValue(float easeVal); 
		#endregion

		#region Interface
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
			if (timeLeft + float.Epsilon < dt)
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
				throw new Exception("Couldn't set initial value!");
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
			T currentValue = this.ComputeCurrentValue(easeVal);
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
		#endregion
	}
}
