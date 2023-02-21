using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Logging;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public class StratusAssetQuery : IStratusLogger
	{
		private Func<string[]> queryAssetNamesFunction;
		private string[] _assetNames;
		
		/// <summary>
		/// Whether assets have been queried
		/// </summary>
		public bool queried { get; private set; }

		/// <summary>
		/// The names of all assets found by the query
		/// </summary>
		public string[] assetNames
		{
			get
			{
				if (_assetNames == null || !updated)
				{
					Update();
				}
				return _assetNames;
			}
		}

		/// <summary>
		/// The count of assets found by the query
		/// </summary>
		public int assetCount => _assetNames.LengthOrZero();

		public bool valid => assetCount > 0;

		/// <summary>
		/// Whether this query is up to date
		/// </summary>
		public bool updated { get; private set; }

		public StratusAssetQuery(Func<string[]> queryFunction)
		{
			this.queryAssetNamesFunction = queryFunction;
			this.updated = false;
		}

		public virtual void Update()
		{
			this._assetNames = GetAssetNames();
			//this.Log($"{_assetNames.Length} assets were found");
			this.updated = true;
		}

		public void Add(string name)
		{
			_assetNames = assetNames.AppendToArray(name);
		}

		protected virtual string[] GetAssetNames()
		{
			return queryAssetNamesFunction();
		}

		/// <summary>
		/// Marks this query as outdated,
		/// meaning it will be updated on the next access
		/// </summary>
		public void SetDirty() => updated = false;
	}

	public class StratusAssetQuery<AssetType> : StratusAssetQuery
		where AssetType : class
	{
		private StratusSortedList<string, AssetType> _assetsByName;
		private Func<IList<AssetType>> getAssetsFunction;
		private Func<AssetType, string> keyFunction;

		private StratusSortedList<string, AssetType> assetsByName
		{
			get
			{
				if (_assetsByName == null || !updated)
				{
					Update();
				}
				return _assetsByName;
			}
		}

		public IList<AssetType> assets => assetsByName.Values;

		public StratusAssetQuery(Func<IList<AssetType>> getAssetsFunction, Func<AssetType, string> keyFunction)
			: base(null)
		{
			this.getAssetsFunction = getAssetsFunction;
			this.keyFunction = keyFunction;
		}

		public bool HasAsset(string name) => assetsByName.ContainsKey(name);

		public AssetType this[string key] => this.assetsByName[key];

		public AssetType GetAsset(string label)
		{
			if (!assetsByName.ContainsKey(label))
			{
				//this.LogError($"Could not find asset named {label}");
				//this.LogError($"Available assets: {assetNames.ToStringJoin()}");
				return null;
			}
			return assetsByName[label];
		}

		public void Add(AssetType asset)
		{
			assetsByName.Add(asset);
			Add(keyFunction(asset));
		}

		protected override string[] GetAssetNames()
		{
			return _assetsByName.Keys.ToArray();
		}

		public override void Update()
		{
			IList<AssetType> values = getAssetsFunction();
			_assetsByName = new StratusSortedList<string, AssetType>(keyFunction, values.Count);
			_assetsByName.AddRange(values);
			base.Update();
		}
	}

}