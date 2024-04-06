using Stratus.Events;
using Stratus.Interpolation;

using System;

namespace Stratus.Models.UI
{
	/// <summary>
	/// An event that handles a screen transition
	/// </summary>
	public abstract record ScreenTransitionEvent(float duration, Action onFinished) : Event
	{
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
	/// Fade in from black.
	/// </summary>
	public record FadeInEvent : FadeEvent
	{
		public FadeInEvent(float duration, Action onFinished = null) : base(duration, onFinished)
		{
		}
	}

	/// <summary>
	/// Fade out to black.
	/// </summary>
	public record FadeOutEvent : FadeEvent
	{
		public FadeOutEvent(float duration, Action onFinished = null) : base(duration, onFinished)
		{
		}
	}

	/// <summary>
	/// Performs a <see cref="FadeOutEvent"/>, executes a given action followed by a <see cref="FadeInEvent"/>
	/// </summary>
	public record FadeOutInEvent(float fadeOutDuration, Action transition, float fadeInDuration) : Event
	{
	}

	public interface IFadeEventHandler
	{
		void FadeIn(FadeInEvent e);
		void FadeOut(FadeOutEvent e);
		void FadeOutIn(FadeOutInEvent e);
	}
}
