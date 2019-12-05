using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace civica_service.Utils.StorageProvider
{
    public static class StorageProviderExtension
    {
        public static void AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var storageProviderConfiguration = configuration.GetSection("StorageProvider");

            switch (storageProviderConfiguration["Type"])
            {
                case "Redis":
                    services.AddStackExchangeRedisCache(options => 
                    {
                        options.Configuration = string.IsNullOrEmpty(storageProviderConfiguration["Address"]) ? "http://127.0.0.1" : storageProviderConfiguration["Address"];
                        options.InstanceName = string.IsNullOrEmpty(storageProviderConfiguration["Name"]) ? "master" : storageProviderConfiguration["Name"];
                    });
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }

            services.AddSingleton<ICacheProvider, CacheProvider>();
        }
    }
}