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
                        options.Configuration = storageProviderConfiguration["Address"];
                        options.InstanceName = storageProviderConfiguration["InstanceName"];
                    });
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
        }
    }
}