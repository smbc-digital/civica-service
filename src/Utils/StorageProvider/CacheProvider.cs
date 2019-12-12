using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace civica_service.Utils.StorageProvider
{
    public class CacheProvider : ICacheProvider
    {
        private readonly bool _allowCaching;

        private readonly IDistributedCache _cacheProvider;

        public CacheProvider(IDistributedCache cacheProvider, IConfiguration configuration)
        {
            _allowCaching = configuration.GetSection("StorageProvider")["Type"] !=  "None";
            _cacheProvider = cacheProvider;
        }

        public async Task<string> GetStringAsync(string key)
        {
            if (_allowCaching)
            {
                return await _cacheProvider.GetStringAsync(key);
            }

            return null;
        }

        public async Task SetStringAsync(string key, string value)
        {
            if (_allowCaching)
            {
                await _cacheProvider.SetStringAsync(key, value);
            }
        }

        public async Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options)
        {
            if (_allowCaching)
            {
                await _cacheProvider.SetStringAsync(key, value, options);
            }
        }
    }
}
