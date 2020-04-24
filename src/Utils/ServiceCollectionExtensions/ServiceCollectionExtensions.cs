using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using civica_service.Helpers.QueryBuilder;
using civica_service.Helpers.SessionProvider;
using civica_service.Services;
using civica_service.Utils.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace civica_service.Utils.ServiceCollectionExtensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static void RegisterHelpers(this IServiceCollection services)
        {
            services.AddSingleton<ISessionProvider, SessionProvider>();
            services.AddTransient<IQueryBuilder, QueryBuilder>();
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ICivicaService, CivicaService>();
        }

        public static void RegisterUtils(this IServiceCollection services)
        {
            services.AddSingleton<IXmlParser, XmlParser>();
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Civica service API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
