//using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Stratus.Extensions;
using Stratus.Collections;
using Stratus.Serialization;

namespace Stratus
{
	[Serializable]
	public class StratusAssetReference : StratusAsset
	{
		[SerializeField]
		//[StratusDropdown(nameof(availableAssetNames))]
		private string[] _aliases;
		public string alias => GetAlias(_aliases);
		protected virtual string GetAlias(string[] values) => values.Random();

		public StratusAssetReference()
		{
		}

		public StratusAssetReference(string name, params string[] aliases) : base(name)
		{
			_aliases = aliases;
		}

		protected virtual string[] availableAssetNames { get; }
	}

	public abstract class StratusAssetReference<TAsset> : StratusAssetReference,
		IStratusReference<TAsset>
		where TAsset : class
	{
		public StratusAssetToken<TAsset> token => resolver.GetAsset(name);
		protected override string[] availableAssetNames => resolver.GetAssetNames();
		public virtual StratusAssetResolver<TAsset> resolver { get; } = defaultResolver;
		public TAsset value => token.asset;

		private static readonly DefaultStratusAssetResolver<TAsset> defaultResolver =
			new DefaultStratusAssetResolver<TAsset>();
	}

	public class StratusAssetCollection<TAsset> : IStratusAssetSource<TAsset>, IStratusAssetResolver<TAsset>
		where TAsset : class
	{
		[SerializeField]
		private List<TAsset> _assets = new List<TAsset>();

		#region Properties
		public StratusSortedList<string, TAsset> assetsByName
		{
			get
			{
				if (_assetsByName == null)
				{
					_assetsByName = new StratusSortedList<string, TAsset>(GetKey, _assets.Count, StringComparer.InvariantCultureIgnoreCase);
					_assetsByName.AddRange(_assets);
				}
				return _assetsByName;
			}
		}
		private StratusSortedList<string, TAsset> _assetsByName;
		public StratusAssetToken<TAsset> this[string key]
		{
			get => GetAsset(key);
		}
		public TAsset[] assets => _assets.ToArray();
		public string[] assetNames => assetsByName.Keys.ToArray();
		#endregion

		#region Methods
		public bool HasAsset(string name) => name.IsValid() && assetsByName.ContainsKey(name);
		protected virtual string GetKey(TAsset element) => element.ToString();
		public StratusAssetToken<TAsset> GetAsset(string name)
		{
			return new StratusAssetToken<TAsset>(name, () => assetsByName.GetValueOrDefault(name));
		}
		public void Add(TAsset asset)
		{
			_assets.Add(asset);
		}
		public void AddRange(IEnumerable<TAsset> assets)
		{
			_assets.AddRange(assets);
		}
		public string[] GetAssetNames() => assetNames;

		public IEnumerable<StratusAssetToken<TAsset>> Fetch()
		{
			return assets.Select(a => new StratusAssetToken<TAsset>(a));
		}
		#endregion
	}
}