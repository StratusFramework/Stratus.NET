using System.Collections.Generic;
//using UnityEngine;
using System;
using Stratus.Extensions;
using Stratus.IO;

namespace Stratus
{
	public interface IStratusSave
	{
		string name { get; }
		bool loaded { get; }
		Dictionary<string, string> ComposeDetailedStringMap();
		void Unload();
		StratusOperationResult LoadAsync(Action onLoad);
	}

	/// <summary>
	/// Base class for saves. Classes inherited from this class add which data they wish to be serialized.
	/// </summary>
	public abstract class StratusSave : IStratusSave, IStratusLogger, IDisposable
	{
		#region Fields

		/// <summary>
		/// The date at which this save was made (possibly overwritten)
		/// </summary>
		public string date;

		/// <summary>
		/// The current total time of this save, in minutes.
		/// </summary>
		public int playtime;

		/// <summary>
		/// An optional description for the state of the game at this save
		/// </summary>
		public string description;
		#endregion

		#region Properties

		/// <summary>
		/// The user-given name for this save
		/// </summary>
		public string name { get; set; }

		/// <summary>
		/// File information about this save
		/// </summary>
		public StratusSaveFileInfo file { get; protected set; }

		/// <summary>
		/// The extension used for save data
		/// </summary>
		public virtual string extension => defaultExtension;

		/// <summary>
		/// The default save extension
		/// </summary>
		public const string defaultExtension = ".save";

		/// <summary>
		/// Whether this data  has been saved to disk
		/// </summary>
		public bool serialized => file != null && file.valid;

		/// <summary>
		/// Whether this save has been fully loaded
		/// </summary>
		public virtual bool loaded => true;
		#endregion

		#region Virtual
		public abstract void OnAfterDeserialize();
		public abstract void OnBeforeSerialize();
		protected abstract void OnDelete();
		#endregion

		#region Overrides
		public override string ToString()
		{
			return $"{name}{(serialized ? $" ({file})" : string.Empty)}";
		}
		#endregion

		#region Messages
		public virtual Dictionary<string, string> ComposeDetailedStringMap()
		{
			var details = new Dictionary<string, string>();
			details.Add(nameof(name), name);
			if (date.IsValid())
			{
				details.Add(nameof(date), date);
			}
			if (description.IsValid())
			{
				details.Add(nameof(description), description);
			}
			details.Add(nameof(playtime), $"{playtime}m"); // 'm' for minutes
			return details;
		}

		/// <summary>
		/// Call this function after the save has been serialized externally
		/// </summary>
		public virtual void OnAfterSerialize()
		{
			if (!serialized)
			{
				//this.Log("Not yet serialized");
				return;
			}
		}

		/// <summary>
		/// Deletes this save's files, if serialized previously
		/// </summary>
		/// <returns></returns>
		public virtual bool DeleteSerialization()
		{
			if (!serialized)
			{
				return false;
			}

			if (!file.Delete())
			{
				//this.LogError($"Failed to delete save file at {file}");
				return false;
			}

			OnDelete();

			file = null;

			return true;
		}
		public abstract void OnAnySerialization(string filePath);
		#endregion

		#region Interface
		/// <summary>
		/// Loads this save, invoking the given action afterwards.
		/// </summary>
		/// <param name="onLoad"></param>
		/// <returns></returns>
		public virtual StratusOperationResult Load()
		{
			return true;
		}

		/// <summary>
		/// Loads this save asynchronously, invoking the given action afterwards.
		/// </summary>
		/// <param name="onLoad"></param>
		public virtual StratusOperationResult LoadAsync(Action onLoad)
		{
			onLoad?.Invoke();
			return true;
		}

		/// <summary>
		/// Unloads all relevant data in memory for this save
		/// </summary>
		public virtual void Unload()
		{
		} 
		#endregion

		#region IDisposable Support
		public bool disposed { get; private set; }
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					Unload();
				}

				disposed = true;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}