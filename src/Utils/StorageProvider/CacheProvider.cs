using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace civica_service.Utils.StorageProvider
{
    public class CacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cacheProvider;

        public CacheProvider(IDistributedCache cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public async Task<string> GetStringAsync(string key)
        {
            return await _cacheProvider.GetStringAsync(key);
        }

        public async Task SetStringAsync(string key, string value)
        {
            await _cacheProvider.SetStringAsync(key, value);
        }

        public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options)
        {
            await _cacheProvider.SetStringAsync(key, value, options);
        }
    }
}
