using System;
using System.Numerics;
using System.Reflection;

namespace Stratus
{
	/// <summary>
	/// Used for interpolating an integer value
	/// </summary>
	public class ActionPropertyInteger : ActionProperty<int>
	{
		float CurrentValue;

		public ActionPropertyInteger(object target, MemberInfo member, int endValue, float duration, StratusEase ease) : base(target, member, endValue, duration, ease)
		{
		}

		public override void ComputeDifference() { this.difference = this.endValue - this.initialValue; }

		public override int ComputeCurrentValue(float easeVal)
		{
			this.CurrentValue = this.initialValue + this.difference * easeVal;
			int result = (int)MathF.Ceiling(this.CurrentValue);
			return result;
		}
	}

	/// <summary>
	/// Used for interpolating a float value
	/// </summary>
	public class ActionPropertyFloat : ActionProperty<float>
	{
		public ActionPropertyFloat(object target, MemberInfo member, float endValue, float duration, StratusEase ease) : base(target, member, endValue, duration, ease)
		{
		}

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
		public ActionPropertyVector2(object target, MemberInfo member, Vector2 endValue, float duration, StratusEase ease) : base(target, member, endValue, duration, ease)
		{
		}

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
		public ActionPropertyVector3(object target, MemberInfo member, Vector3 endValue, float duration, StratusEase ease) : base(target, member, endValue, duration, ease)
		{
		}

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
		public ActionPropertyVector4(object target, MemberInfo member, Vector4 endValue, float duration, StratusEase ease) 
			: base(target, member, endValue, duration, ease)
		{
		}

		public override void ComputeDifference() 
		{
			this.difference = this.endValue - this.initialValue; 
		}

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
		public ActionPropertyBoolean(object target, MemberInfo member, bool endValue, float duration, StratusEase ease) : base(target, member, endValue, duration, ease)
		{
		}

		public override void ComputeDifference() { }
		public override void SetCurrent() { }
		public override bool ComputeCurrentValue(float easeVal) { return false; }
	}
}