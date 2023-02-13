using System;
using System.Numerics;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// Used for interpolating a float value
	/// </summary>
	public class ActionPropertyFloat : ActionProperty<float>
	{
		public ActionPropertyFloat(object target, PropertyInfo property, float endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyFloat(object target, FieldInfo field, float endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }
		public override float ComputeCurrentValue(float easeVal)
		{
			var currentVal = this.initialValue + this.difference * easeVal;
			return currentVal;
		}
	}

	/// <summary>
	/// Used for interpolating a Vector2 value
	/// </summary>
	public class ActionPropertyVector2 : ActionProperty<Vector2>
	{
		public ActionPropertyVector2(object target, PropertyInfo property, Vector2 endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyVector2(object target, FieldInfo field, Vector2 endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

		public override Vector2 ComputeCurrentValue(float easeVal)
		{
			return this.initialValue + this.difference * easeVal;
		}

	}

	/// <summary>
	/// Used for interpolating a Vector3 value
	/// </summary>
	public class ActionPropertyVector3 : ActionProperty<Vector3>
	{
		public ActionPropertyVector3(object target, PropertyInfo property, Vector3 endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyVector3(object target, FieldInfo field, Vector3 endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

		public override Vector3 ComputeCurrentValue(float easeVal)
		{
			return this.initialValue + this.difference * easeVal;
		}
	}

	/// <summary>
	/// Used for interpolating a Vector4 value
	/// </summary>
	public class ActionPropertyVector4 : ActionProperty<Vector4>
	{
		public ActionPropertyVector4(object target, PropertyInfo property, Vector4 endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyVector4(object target, FieldInfo field, Vector4 endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

		public override Vector4 ComputeCurrentValue(float easeVal)
		{
			return this.initialValue + this.difference * easeVal;
		}
	}

	/// <summary>
	/// Used for interpolating a boolean value
	/// </summary>
	public class ActionPropertyBoolean : ActionProperty<bool>
	{
		public ActionPropertyBoolean(object target, PropertyInfo property, bool endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyBoolean(object target, FieldInfo field, bool endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { }
		public override void SetCurrent() { }
		public override bool ComputeCurrentValue(float easeVal) { return false; }
	}

	/// <summary>
	/// Used for interpolating an integer value
	/// </summary>
	public class ActionPropertyInteger : ActionProperty<int>
	{
		float CurrentValue;

		public ActionPropertyInteger(object target, PropertyInfo property, int endValue, float duration, StratusEase ease)
		  : base(target, property, endValue, duration, ease) { }

		public ActionPropertyInteger(object target, FieldInfo field, int endValue, float duration, StratusEase ease)
		  : base(target, field, endValue, duration, ease) { }

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

		public override int ComputeCurrentValue(float easeVal)
		{
			this.CurrentValue = this.initialValue + this.difference * easeVal;
			return (int)MathF.Ceiling(this.CurrentValue);
		}

	}
}