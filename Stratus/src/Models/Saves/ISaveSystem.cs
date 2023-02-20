using Stratus.IO;

using System;

namespace Stratus.Models.Saves
{
	public interface ISaveSystem
	{
		void RefreshSaveFiles();
		void ClearSaveFiles();
		void LoadAllSaves(bool force = false);
	}

	public interface ISaveSystem<SaveType> : ISaveSystem
		where SaveType : ISave, new()
	{
		SaveType[] saves { get; }
		SaveType CreateSave(Action<SaveType> onCreated = null);
		Result SaveAs(SaveType save, string fileName);
		Result Save(SaveType save);
		SaveType Load(SaveFileInfo file);
		SaveType GetSaveAtIndex(int index);
	}

	/// <summary>
	/// Configurable attributes for a save system
	/// </summary>
	public class SaveSystemConfiguration
	{
		/// <summary>
		/// Whether the save data system is being debugged
		/// </summary>
		public bool debug { get; set; }

		/// <summary>
		/// If assigned, will store saves within this folder rather than the root
		/// of <see cref="SaveSystem.rootSaveDirectoryPath"/>
		/// </summary>
		public string folder { get; set; }

		/// <summary>
		/// The save format
		/// </summary>
		public SaveFormat format
		{
			get => _format;
			set
			{
				_format = value;
				onChanged?.Invoke();
			}
		}
		private SaveFormat _format;
		/// <summary>
		/// What naming convention to use for a save file of this type
		/// </summary>
		public FileNamingConvention namingConvention { get; set; }
		/// <summary>
		/// The maximum amount of saves allowed. If 0, the saves are unlimited.
		/// </summary>
		public int saveLimit = 1000;

		/// <summary>
		/// The default save extension
		/// </summary>
		public const string defaultSaveExtension = ".save";

		public event Action onChanged;

		public SaveSystemConfiguration(SaveFormat format, FileNamingConvention namingConvention)
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