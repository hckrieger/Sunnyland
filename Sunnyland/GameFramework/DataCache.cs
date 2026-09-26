using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sunnyland.GameFramework
{
	public class DataCache<TAsset>
	{

		private Dictionary<string, TAsset> assets;
		private Func<string, TAsset> methodOfAccess;

		public DataCache(Func<string, TAsset> methodOfAccess)
		{
			assets = new Dictionary<string, TAsset>();
			this.methodOfAccess = methodOfAccess;
		}


		public void Add(string key, string path)
		{
			if (!assets.ContainsKey(key))
			{
				
				assets[key] = methodOfAccess(path);

			}
		}


		public TAsset GetData(string key)
		{
			if (assets.TryGetValue(key, out var asset))
			{
				return asset;
			}
			else
			{
				throw new KeyNotFoundException($"Data with key '{key}' not found.");
			}
		}


		public void Remove(string key)
		{
			if (assets.TryGetValue(key,out var asset))
			{
				assets.Remove(key);
			}
		}



	}
}
