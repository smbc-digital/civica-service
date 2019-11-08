using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Helpers.SessionProvider.Models;
using civica_service.Utils.HealthChecks;
using civica_service.Utils.StorageProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockportGovUK.AspNetCore.Middleware;
using StockportGovUK.AspNetCore.Availability;
using StockportGovUK.AspNetCore.Availability.Middleware;
using StockportGovUK.AspNetCore.Gateways;
using Swashbuckle.AspNetCore.Swagger;
using civica_service.Services;

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
            services.Configure<SessionConfiguration>(Configuration.GetSection("SessionConfiguration"));
            services.AddSingleton<ICivicaService, CivicaService>();
            services.AddSingleton<ISessionProvider, SessionProvider>();
            services.AddTransient<IQueryBuilder, QueryBuilder>();


            services.AddStorageProvider(Configuration);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAvailability();
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "civica_service API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseMiddleware<Availability>();
            app.UseMiddleware<ExceptionHandling>();
            app.UseHttpsRedirection();
            app.UseHealthChecks("/healthcheck", HealthCheckConfig.Options);
            app.UseMvc();
            app.UseSwagger();
            var swaggerPrefix = env.IsDevelopment() ? string.Empty : "/civicaservice";
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"{swaggerPrefix}/swagger/v1/swagger.json", "civica_service API");
            });
        }
    }
}
