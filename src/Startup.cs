using System.Diagnostics.CodeAnalysis;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.HealthChecks;
using civica_service.Utils.ServiceCollectionExtensions;
using civica_service.Utils.StorageProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.NetStandard.Gateways;

namespace civica_service
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddStorageProvider(Configuration);
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.AddAvailability();
            services.AddSwagger();
            services.AddHealthChecks()
                    .AddCheck<TestHealthCheck>("TestHealthCheck");
            
            services.Configure<SessionConfiguration>(Configuration.GetSection("SessionConfiguration"));

            services.RegisterHelpers();
            services.RegisterServices();
            services.RegisterUtils();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("local"))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseMiddleware<Availability>();
            app.UseMiddleware<ApiExceptionHandling>();
            
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{(env.IsEnvironment("local") ? string.Empty : "/civicaservice")}/swagger/v1/swagger.json", "Civica service API");
            });
        }
    }
}
