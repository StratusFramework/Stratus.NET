using System;

namespace Stratus
{
	public interface IStratusAssetToken<T> : IStratusAsset<T>
	{
		StratusAssetSourceType assetSourceType { get; }
	}

	public class StratusAssetToken<T> : IStratusAssetToken<T>
		where T : class
	{
		public string name { get; private set; }
		public StratusAssetSourceType assetSourceType { get; private set; }
		public T asset
		{
			get
			{
				if (!queried || IsNull(_asset))
				{
					switch (assetSourceType)
					{
						case StratusAssetSourceType.Invalid:
							break;
						case StratusAssetSourceType.Reference:
							_asset = assetFunction();
							break;
						case StratusAssetSourceType.Alias:
							_asset = aliasToAssetFunction(name);
							break;
					}
					queried = true;
				}
				return _asset;
			}
		}
		private T _asset = null;

		public bool queried { get; private set; }

		private Func<T> assetFunction;
		private Func<string, T> aliasToAssetFunction;

		public StratusAssetToken(string name, Func<T> assetFunction)
		{
			this.name = name;
			this.assetSourceType = StratusAssetSourceType.Reference;
			this.assetFunction = assetFunction;
		}

		public StratusAssetToken(IStratusNamed named, Func<T> assetFunction)
			: this(named.name, assetFunction)
		{
		}

		public StratusAssetToken(object obj)
			: this(obj.ToString(), () => (T)obj)
		{
		}

		public StratusAssetToken(string name, Func<string, T> aliasToAssetFunction)
		{
			this.name = name;
			this.assetSourceType = StratusAssetSourceType.Alias;
			this.aliasToAssetFunction = aliasToAssetFunction;
		}

		protected virtual bool IsNull(T asset) => asset == null;
		public override string ToString() => name;
	}
}
