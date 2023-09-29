using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using StackExchange.Redis;

namespace civica_service.Utils.StorageProvider
{
    public static class StorageProviderExtension
    {
        public static IServiceCollection AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var storageProviderConfiguration = configuration.GetSection("StorageProvider");

            switch (storageProviderConfiguration["Type"])
            {
                case "Redis":
                    var configurationOptions = new ConfigurationOptions
                    {
                        EndPoints =
                        {
                            { storageProviderConfiguration["Address"] ?? "127.0.0.1",  6379 }
                        },
                        ClientName = storageProviderConfiguration["InstanceName"] ?? Assembly.GetEntryAssembly()?.GetName().Name,
                        SyncTimeout = 30000,
                        AsyncTimeout = 30000,
                    };

                    if (!string.IsNullOrEmpty(storageProviderConfiguration["Password"]))
                        configurationOptions.Password = storageProviderConfiguration["Password"];

                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.ConfigurationOptions = configurationOptions;
                    });

                    break;
                case "None":
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }

            services.AddSingleton<ICacheProvider, CacheProvider>();

            return services;
        }
    }
}