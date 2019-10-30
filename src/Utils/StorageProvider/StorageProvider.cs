using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace civica_service.Utils.StorageProvider
{
    public static class StorageProvider
    {
        public static void AddStorageProvider(this IServiceCollection services, IConfiguration configuration)
        {
            var type = configuration.GetSection("StorageProvider")?["Type"] ?? string.Empty;

            switch (type)
            {
                case "Redis":
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = "localhost";
                        options.InstanceName = "SampleInstance";
                    });
                    break;
                default:
                    services.AddDistributedMemoryCache();
                    break;
            }
        }
    }
}