using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace civica_service.Utils.HealthChecks
{
    public static class HealthCheckConfig
    {
        private static readonly AssemblyName Assembly = System.Reflection.Assembly.GetEntryAssembly()?.GetName();

        public static HealthCheckOptions Options => new HealthCheckOptions
        {
            ResponseWriter = async (c, r) =>
            {
                c.Response.ContentType = MediaTypeNames.Application.Json;

                switch (r.Status)
                {
                    case HealthStatus.Healthy:
                        await c.Response.WriteAsync(ProcessHealthy(r));
                        break;
                    case HealthStatus.Unhealthy:
                        await c.Response.WriteAsync(ProcessUnhealthy(r));
                        break;
                    case HealthStatus.Degraded:
                        await c.Response.WriteAsync(ProcessUnhealthy(r));
                        break;
                }
            }
        };

        private static string ProcessUnhealthy(HealthReport report)
        {
            return JsonSerializer.Serialize(new
                {
                    application = new {
                        name = Assembly.Name,
                        version = Assembly.Version.ToString(),
                        status = report.Status.ToString(),
                    },
                    checks = report.Entries.Select(e =>
                        new {
                            description = e.Key,
                            status = e.Value.Status.ToString(),
                            exception = e.Value.Exception?.Message,
                            data = e.Value.Data.Select(_ => $"{_.Key}: {_.Value}"),
                            responseTime = e.Value.Duration.TotalMilliseconds
                        }),
                    totalResponseTime = report.TotalDuration.TotalMilliseconds
                });
        }

        private static string ProcessHealthy(HealthReport report)
        {
            return JsonSerializer.Serialize(new
                {
                    application = new {
                        name = Assembly.Name,
                        version = Assembly.Version.ToString(),
                        status = report.Status.ToString(),
                    },
                    checks = report.Entries.Select(e =>
                        new {
                            description = e.Key,
                            status = e.Value.Status.ToString(),
                            responseTime = e.Value.Duration.TotalMilliseconds
                        }),
                    totalResponseTime = report.TotalDuration.TotalMilliseconds
                });
        }
    }
}