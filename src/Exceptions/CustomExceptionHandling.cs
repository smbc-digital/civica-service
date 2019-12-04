using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Middleware;

namespace civica_service.Exceptions
{
    public class CustomExceptionHandling : ApiExceptionHandling
    {
        public CustomExceptionHandling(
            RequestDelegate next,
            ILogger<ExceptionHandling> logger,
            IConfiguration configuration) : base(next, logger, configuration)
        {
        }

        public override async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CivicaUnavailableException ex)
            {
                await HandleResponse(context, ex, HttpStatusCode.ServiceUnavailable);
            }
            catch (Exception ex)
            {
                await HandleResponse(context, ex);
            }
        }
    }
}
