using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Services.MiddleWare
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError($"Unhandled Exception: {exception}");

            httpContext.Response.ContentType = "application/json";

            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,  // 401
                ArgumentException => (int)HttpStatusCode.BadRequest,  // 400
                KeyNotFoundException => (int)HttpStatusCode.NotFound,  // 404
                _ => (int)HttpStatusCode.InternalServerError  // 500
            };

            httpContext.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                statusCode,
                message = exception.Message,
                errorType = exception.GetType().Name
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse);

            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true; // Exception handled
        }
    }
}
