using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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
                        options.Configuration = storageProviderConfiguration["Address"] ?? "127.0.0.1";
                        options.InstanceName = storageProviderConfiguration["Name"] ?? Assembly.GetEntryAssembly()?.GetName().Name;
                    });
                    break;
                case "None":
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }

            services.AddSingleton<ICacheProvider, CacheProvider>();
        }
    }
}