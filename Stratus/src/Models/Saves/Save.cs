using Stratus.Extensions;
using Stratus.IO;
using Stratus.Serialization;

using System;
using System.Collections.Generic;

namespace Stratus.Models.Saves
{
	public interface ISave
	{
		string name { get; }
		bool loaded { get; }
		Dictionary<string, string> ComposeDetailedStringMap();
		void Unload();
		StratusOperationResult LoadAsync(Action onLoad);
	}

	public enum SaveType
	{
		/// <summary>
		/// Manual saves triggered by the player
		/// </summary>
		Manual,
		/// <summary>
		/// Automatic saves triggered by the game
		/// </summary>
		Auto,
		/// <summary>
		/// Saves usually triggered by a hotkey
		/// </summary>
		Quick
	}

	/// <summary>
	/// Base class for saves. Classes inherited from this class add which data they wish to be serialized.
	/// </summary>
	public abstract class Save : ISave, IStratusLogger, IDisposable
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
		public SaveFileInfo file { get; protected set; }

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

	public abstract class Save<TData> : Save
		where TData : class, new()
	{
		/// <summary>
		/// The data for this save
		/// </summary>
		public TData data { get; private set; }

		/// <summary>
		/// Whether the data for the save is loaded
		/// </summary>
		public bool dataLoaded => data != null;

		public override bool loaded => dataLoaded;

		/// <summary>
		/// The file path for where the encapsulated data is saved to
		/// </summary>
		public string dataFilePath
		{
			get
			{
				if (_dataFilePath == null)
				{
					_dataFilePath = FileUtility.ChangeExtension(file.path, dataExtension);
				}
				return _dataFilePath;
			}
		}
		private string _dataFilePath;

		/// <summary>
		/// Whether a data file exists for this save
		/// </summary>
		public bool dataFileExists => FileUtility.FileExists(dataFilePath);

		/// <summary>
		/// The data serializer
		/// </summary>
		public static readonly StratusJSONSerializer<TData> dataSerializer = new StratusJSONSerializer<TData>();

		/// <summary>
		/// The extension used for save data
		/// </summary>
		public virtual string dataExtension => ".savedata";

		public Save(TData data)
		{
			this.data = data;
		}

		public Save()
		{
		}

		public void ResetData()
		{
			SetData(new TData());
		}

		public void SetData(TData data)
		{
			this.data = data;
		}

		protected override void OnDelete()
		{
			if (dataFileExists)
			{
				FileUtility.DeleteFile(dataFilePath);
				_dataFilePath = null;
			}
		}

		public override StratusOperationResult Load()
		{
			return LoadData();
		}

		public override StratusOperationResult LoadAsync(Action onLoad)
		{
			return LoadDataAsync(onLoad);
		}

		public virtual StratusOperationResult LoadData()
		{
			if (dataLoaded)
			{
				return new StratusOperationResult(true, "Data already loaded");
			}

			if (!serialized)
			{
				return new StratusOperationResult(false, "Cannot load data before the save has been serialized");
			}

			try
			{
				data = dataSerializer.Deserialize(dataFilePath);
			}
			catch (Exception e)
			{
				return new StratusOperationResult(false, e.ToString());
			}

			if (data == null)
			{
				return new StratusOperationResult(false, $"Failed to deserialize data from {dataFilePath}");
			}

			return new StratusOperationResult(true, $"Loaded data file from {dataFilePath}");
		}

		public virtual StratusOperationResult LoadDataAsync(Action onLoad)
		{
			return LoadData();
		}

		public virtual void UnloadData()
		{
			data = null;
		}

		public virtual bool SaveData()
		{
			if (!serialized)
			{
				this.LogError("Cannot load data before the save has been serialized");
				return false;
			}

			if (data == null)
			{
				this.LogError("No data to serialize! This could mean that this save was created yet no data previously assigned to it");
				return false;
			}

			this.Log($"Saving data to {dataFilePath}");
			dataSerializer.Serialize(data, dataFilePath);
			return true;
		}

		/// <summary>
		/// Invoked whenever this save is serialized/deserialized
		/// </summary>
		/// <param name="filePath"></param>
		public override void OnAnySerialization(string filePath)
		{
			this.file = new SaveFileInfo(filePath);
		}
	}
}