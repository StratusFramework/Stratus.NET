using Stratus.Collections;
using Stratus.Extensions;
using Stratus.Types;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratus
{
	public interface IStratusAssetResolver<TAsset>
		where TAsset : class
	{
		bool HasAsset(string name);
		StratusAssetToken<TAsset> GetAsset(string name);
		string[] GetAssetNames();
	}

	public interface IStratusAssetSource<TAsset>
		where TAsset : class
	{
		IEnumerable<StratusAssetToken<TAsset>> Fetch();
	}

	public abstract class StratusAssetSource<TAsset>
		: IStratusAssetSource<TAsset>
		where TAsset : class
	{
		public abstract IEnumerable<StratusAssetToken<TAsset>> Fetch();
	}

	public abstract class StratusAssetResolver<TAsset> : IStratusAssetResolver<TAsset>
		where TAsset : class
	{
		public AutoSortedList<string, StratusAssetToken<TAsset>> assetsByName
		{
			get
			{
				if (_assetsByName == null)
				{
					Resolve();
				}
				return _assetsByName;
			}
		}
		private AutoSortedList<string, StratusAssetToken<TAsset>> _assetsByName;

		public abstract StratusAssetSource<TAsset>[] sources { get; }
		protected virtual string GetKey(StratusAssetToken<TAsset> element) => element.ToString();
		private static readonly string typeName = typeof(TAsset).Name;

		public void Resolve(bool force = false)
		{
			if (_assetsByName == null || force)
			{
				_assetsByName = new AutoSortedList<string, StratusAssetToken<TAsset>>(
					a => a.name,
					0, 
					StringComparer.InvariantCultureIgnoreCase);

				foreach (var source in sources)
				{
					try
					{
						var assets = source.Fetch();
						_assetsByName.AddRange(assets);
					}
					catch (Exception ex)
					{
						//StratusDebug.LogException(ex);
					}
				}

				if (_assetsByName.IsNullOrEmpty())
				{
					//StratusDebug.LogError($"Found no assets among sources ({sources.Length}) for {typeof(TAsset)}");
				}
			}
		}

		public bool HasAsset(string name)
		{
			return assetsByName.ContainsKey(name);
		}

		public StratusAssetToken<TAsset> GetAsset(string name)
		{
			var asset = assetsByName.GetValueOrDefault(name);
			if (asset == null)
			{
				//StratusDebug.LogError($"Did not find {typeName} named {name}. ({assetsByName.Count})");
			}
			return asset;
		}

		public string[] GetAssetNames()
		{
			return _assetsByName.Keys.ToArray();
		}
	}

	public class DefaultStratusAssetResolver<TAsset> : StratusAssetResolver<TAsset>
		where TAsset : class
	{
		private static readonly Lazy<Dictionary<Type, Type[]>> sourceTypesByAsset
			= new Lazy<Dictionary<Type, Type[]>>(() => TypeUtility.TypeDefinitionParameterMap(typeof(StratusAssetSource<>)));

		public override StratusAssetSource<TAsset>[] sources
		{
			get
			{
				if (_sources == null)
				{
					Type assetType = typeof(TAsset);
					var impl = TypeUtility.ImplementationsOf(typeof(StratusAssetSource<>), assetType);
					if (impl.IsValid())
					{
						_sources = impl.Select(t => t.Instantiate<StratusAssetSource<TAsset>>()).ToArray();
						//StratusDebug.Log($"Found sources ({_sources.Length}) for {assetType}");
					}
					else
					{
						//StratusDebug.LogError($"Found no sources for {assetType}. Sources -> {impl.ToStringJoin()}");
					}
				}
				return _sources;
			}
		}
		private StratusAssetSource<TAsset>[] _sources;
	}

	public abstract class CustomStratusAssetSource<TAsset> : StratusAssetSource<TAsset>
		where TAsset : class
	{
		public IEnumerable<TAsset> assets
		{
			get
			{
				if (_assets == null)
				{
					_assets = Generate();
				}
				return _assets;
			}
		}

		private IEnumerable<TAsset> _assets;
		protected abstract string Name(TAsset asset);
		protected abstract IEnumerable<TAsset> Generate();
		public override IEnumerable<StratusAssetToken<TAsset>> Fetch()
		{
			return assets.Select(a => new StratusAssetToken<TAsset>(Name(a), () => a));
		}
	}

	public abstract class ResourcesStratusAssetSource<TAsset>
	{
	}

	public static class StratusAssetDatabase
	{
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class StratusAssetSourceAttribute : Attribute
	{
		public Type sourceTypes { get; set; }
	}

}