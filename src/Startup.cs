using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using civica_service.Utils.HealthChecks;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHealthChecks()
                .AddCheck<TestHealthCheck>("TestHealthCheck");
            services.AddAvailability();
            services.AddResilientHttpClients<IGateway, Gateway>(Configuration);
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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "civica_service API");
            });
        }
    }
}
