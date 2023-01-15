using Stratus.IO;

using System;

namespace Stratus
{
	public interface IStratusSaveSystem
	{
		void RefreshSaveFiles();
		void ClearSaveFiles();
		void LoadAllSaves(bool force = false);
	}

	public interface IStratusSaveSystem<SaveType> : IStratusSaveSystem
		where SaveType : IStratusSave, new()
	{
		SaveType[] saves { get; }
		SaveType CreateSave(Action<SaveType> onCreated = null);
		StratusOperationResult SaveAs(SaveType save, string fileName);
		StratusOperationResult Save(SaveType save);
		StratusOperationResult SaveAsync(SaveType save, Action onFinished);
		SaveType Load(StratusSaveFileInfo file);
		SaveType GetSaveAtIndex(int index);
	}

	public enum StratusSaveType
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
	/// Configurable attributes for a save system
	/// </summary>
	public class StratusSaveSystemConfiguration
	{
		/// <summary>
		/// Whether the save data system is being debugged
		/// </summary>
		public bool debug { get; set; }

		/// <summary>
		/// If assigned, will store saves within this folder rather than the root
		/// of <see cref="StratusSaveSystem.rootSaveDirectoryPath"/>
		/// </summary>
		public string folder { get; set; }

		/// <summary>
		/// The save format
		/// </summary>
		public StratusSaveFormat format
		{
			get => _format;
			set
			{
				_format = value;
				onChanged?.Invoke();
			}
		}
		private StratusSaveFormat _format;
		/// <summary>
		/// What naming convention to use for a save file of this type
		/// </summary>
		public StratusFileNamingConvention namingConvention { get; set; }
		/// <summary>
		/// The maximum amount of saves allowed. If 0, the saves are unlimited.
		/// </summary>
		public int saveLimit = 1000;

		/// <summary>
		/// The default save extension
		/// </summary>
		public const string defaultSaveExtension = ".save";

		public event Action onChanged;

		public StratusSaveSystemConfiguration(StratusSaveFormat format, StratusFileNamingConvention namingConvention)
		{
			this.format = format;
			this.namingConvention = namingConvention;
		}

		public string GenerateSaveFilePath(string path, StratusSaveFileQuery files)
		{
			return format.GenerateSaveFilePath(path, namingConvention.GenerateFileName(files));
		}
	}

}