using Stratus.Extensions;

using System;
using System.Collections.Generic;

namespace Stratus.Inputs
{
	/// <summary>
	/// Manages pushing/popping input layers in a customized manner for the system
	/// </summary>
	/// <typeparam name="TLayer"></typeparam>
	public class InputStack<TLayer>
		where TLayer : InputLayer
	{
		private Stack<TLayer> _layers = new Stack<TLayer>();
		private Queue<TLayer> _queuedLayers = new Queue<TLayer>();

		public TLayer activeLayer => _layers.Count > 0 ? _layers.Peek() : null;
		public bool canPop => activeLayer != null && !activeLayer.pushed;
		public int layerCount => _layers.Count;
		public bool hasActiveLayers => layerCount > 0;
		public bool hasQueuedLayers => _queuedLayers.IsValid();

		public event Action<TLayer> onInputLayerChanged;

		public Result Push(TLayer layer)
		{
			return Push(layer, true);
		}

		private Result Push(TLayer layer, bool update)
		{
			if (hasActiveLayers && !layer.ignoreBlocking)
			{
				// If the current layer is blocking, queue this layer for later
				if (activeLayer.blocking)
				{
					_queuedLayers.Enqueue(layer);
					return new Result(false, $"Active layer {activeLayer.name} is blocking. Queuing...");
				}
				activeLayer.active = false;
			}

			layer.pushed = true;
			if (update)
			{
				ActivateInputLayer(layer);
			}
			_layers.Push(layer);
			return true;
		}

		public TLayer Pop()
		{
			TLayer layer = null;


			// If there's layers remaining, remove the topmost
			if (hasActiveLayers)
			{
				layer = _layers.Pop();
				layer.active = false;
				layer.pushed = false;
			}

			bool queue = hasQueuedLayers &&
				(!hasActiveLayers || hasActiveLayers && !activeLayer.blocking);

			// If there's still layers left
			if (queue)
			{
				// If there are queud layers
				// and the topmost is not blocking
				if (hasQueuedLayers)
				{
					while (_queuedLayers.IsValid())
					{
						layer = _queuedLayers.Dequeue();
						bool blocking = layer.blocking;
						Push(layer, !blocking);
						if (blocking)
						{
							break;
						}
					}
				}
			}

			// Update the current layer if its active
			if (hasActiveLayers)
			{
				if (activeLayer.pushed)
				{
					ActivateInputLayer(activeLayer);
				}
			}

			return layer;
		}

		private void ActivateInputLayer(TLayer inputLayer)
		{
			inputLayer.active = true;
			onInputLayerChanged?.Invoke(inputLayer);
		}

	}

}