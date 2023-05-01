using Stratus.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Stratus.Inputs
{
	/// <summary>
	/// Manages pushing/popping input layers in a customized manner for the system
	/// </summary>
	/// <typeparam name="TLayer"></typeparam>
	public class InputStack<TLayer> : IEnumerable<TLayer>
		where TLayer : InputLayer
	{
		private Stack<TLayer> _layers = new Stack<TLayer>();
		private Queue<TLayer> _layersToPush = new Queue<TLayer>();
		private HashSet<TLayer> _layersToPop = new HashSet<TLayer>();

		public TLayer? current => _layers.Count > 0 ? _layers.Peek() : null;
		public int count => _layers.Count;
		public bool hasLayers => count > 0;
		public bool hasQueuedLayers => _layersToPush.IsValid();

		public event Action<TLayer> onLayerToggled;
		public event Action<TLayer> onQueue;
		public event Action<TLayer> onPush;
		public event Action<TLayer> onPop;

		public override string ToString()
		{
			return $"{count} : {_layers.ToStringJoin().Enclose(StratusStringEnclosure.SquareBracket)}";
		}

		public Result Push(TLayer layer)
		{
			return Push(layer, true);
		}

		private Result Push(TLayer layer, bool update)
		{
			if (hasLayers && !layer.ignoreBlocking)
			{
				// If the current layer is blocking, queue this layer for later
				if (current.blocking)
				{
					_layersToPush.Enqueue(layer);
					onQueue?.Invoke(layer);
					return new Result(false, $"Active layer {current.name} is blocking. Queuing...");
				}
				current.active = false;
			}

			if (update)
			{
				ActivateInputLayer(layer);
			}

			_layers.Push(layer);
			onPush?.Invoke(layer);

			return new Result(true, $"Pushed layer {layer}");
		}

		public TLayer? TryPop(TLayer layer)
		{
			if (current == layer)
			{
				return Pop();
			}

			_layersToPop.Add(layer);
			return current;
		}

		public TLayer Pop()
		{
			TLayer layer = null;

			// If there's layers remaining, remove the topmost
			if (hasLayers)
			{
				layer = _layers.Pop();
				onPop?.Invoke(layer);
				layer.active = false;
			}

			bool queue = hasQueuedLayers &&
				(!hasLayers || hasLayers && !current.blocking);

			// If there's still layers left
			if (queue)
			{
				// If there are queued layers
				// and the topmost is not blocking
				if (hasQueuedLayers)
				{
					while (_layersToPush.IsValid())
					{
						layer = _layersToPush.Dequeue();
						bool blocking = layer.blocking;
						Push(layer, !blocking);
						if (blocking)
						{
							break;
						}
					}
				}
			}

			// Update the current layer if its not active
			if (current != null && !current.active)
			{
				ActivateInputLayer(current);
			}

			if (_layersToPop.Contains(current))
			{
				_layersToPop.Remove(current);
				layer = Pop();
			}

			return layer;
		}

		private void ActivateInputLayer(TLayer inputLayer)
		{
			inputLayer.active = true;
			onLayerToggled?.Invoke(inputLayer);
		}

		public IEnumerator<TLayer> GetEnumerator()
		{
			return ((IEnumerable<TLayer>)this._layers).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this._layers).GetEnumerator();
		}
	}

}