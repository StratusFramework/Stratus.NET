using Stratus.Events;

using System;

namespace Stratus.Models.UI
{
	/// <summary>
	/// An event that handles a screen transition
	/// </summary>
	public abstract record ScreenTransitionEvent : Event
	{
		public float duration { get; }
		public Action onFinished { get; }

		public ScreenTransitionEvent(float duration, Action onFinished)
		{
			this.duration = duration;
			this.onFinished = onFinished;
		}
	}

	/// <summary>
	/// A fade is a subtype of dissolve transition that gradually moves to or from an image to or from black.
	/// </summary>
	/// <remarks>Fades are often used at the beginning/end of scenes.</remarks>
	public record FadeEvent : ScreenTransitionEvent
	{
		public FadeEvent(float duration, Action onFinished) : base(duration, onFinished)
		{
		}
	}

	/// <summary>
	/// Fade in FROM black.
	/// </summary>
	public record FadeInEvent : FadeEvent
	{
		public FadeInEvent(float duration, Action onFinished) : base(duration, onFinished)
		{
		}
	}

	/// <summary>
	/// Fade out TO black.
	/// </summary>
	public record FadeOutEvent : FadeEvent
	{
		public FadeOutEvent(float duration, Action onFinished) : base(duration, onFinished)
		{
		}
	}
}
