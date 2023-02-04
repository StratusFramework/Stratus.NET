using System.Runtime.Serialization;
using System;

namespace Stratus.Reflection
{
	/// <summary>
	/// An exception that is thrown whenever a field was not found inside of an object when using Reflection.
	/// </summary>
	[Serializable]
	public class FieldNotFoundException : Exception
	{
		public FieldNotFoundException() { }

		public FieldNotFoundException(string message) : base(message) { }

		public FieldNotFoundException(string message, Exception inner) : base(message, inner) { }

		protected FieldNotFoundException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// An exception that is thrown whenever a property was not found inside of an object when using Reflection.
	/// </summary>
	[Serializable]
	public class PropertyNotFoundException : Exception
	{
		public PropertyNotFoundException() { }

		public PropertyNotFoundException(string message) : base(message) { }

		public PropertyNotFoundException(string message, Exception inner) : base(message, inner) { }

		protected PropertyNotFoundException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// An exception that is thrown whenever a field or a property was not found inside of an object when using Reflection.
	/// </summary>
	[Serializable]
	public class PropertyOrFieldNotFoundException : Exception
	{
		public PropertyOrFieldNotFoundException() { }

		public PropertyOrFieldNotFoundException(string message) : base(message) { }

		public PropertyOrFieldNotFoundException(string message, Exception inner) : base(message, inner) { }

		protected PropertyOrFieldNotFoundException(
		  SerializationInfo info,
		  StreamingContext context) : base(info, context) { }
	}
}
