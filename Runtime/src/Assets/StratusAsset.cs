namespace Stratus
{
	public enum StratusAssetSourceType
	{
		Invalid,
		Reference,
		Alias
	}

	public interface IStratusAsset
	{
		/// <summary>
		/// The name of the asset
		/// </summary>
		string name { get; }
	}

	public interface IStratusAsset<T> : IStratusAsset
	{
		T asset { get; }
	}

	public abstract class StratusAsset
	{
		private string _name;
		public string name => _name;

		public StratusAsset()
		{
		}

		public StratusAsset(string name)
		{
			this._name = name;
		}

		public void Set(string name)
		{
			_name = name;
		}

		public override string ToString()
		{
			return _name;
		}
	}


}