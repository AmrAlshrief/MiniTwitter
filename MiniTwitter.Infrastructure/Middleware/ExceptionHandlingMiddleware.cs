using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using MiniTwitter.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiniTwitter.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerService _logger;
        private readonly IHostEnvironment _environment;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public ExceptionHandlingMiddleware(
            RequestDelegate next, 
            ILoggerService logger,
            IHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = GetOrGenerateCorrelationId(context);
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Add correlation ID to response headers
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(CorrelationIdHeader, correlationId);
                    return Task.CompletedTask;
                });

                await _next(context);
                stopwatch.Stop();
                
                // Log error responses (4xx and 5xx)
                if (context.Response.StatusCode >= 400)
                {
                    LogRequestError(context, null, stopwatch.ElapsedMilliseconds, correlationId);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await HandleExceptionAsync(context, ex, correlationId, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context, 
            Exception exception, 
            string correlationId,
            long durationMs)
        {
            // Log the error with detailed context
            LogRequestError(context, exception, durationMs, correlationId);

            // Prepare error response
            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path,
                Extensions = new Dictionary<string, object>
                {
                    ["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier,
                    ["correlationId"] = correlationId,
                    ["timestamp"] = DateTime.UtcNow.ToString("O")
                }
            };

            // Customize based on exception type
            switch (exception)
            {
                case UnauthorizedAccessException _:
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Access Denied";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                    break;
                    
                case KeyNotFoundException _:
                case FileNotFoundException _:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Resource not found";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                    break;
                    
                case ArgumentException _:
                case InvalidOperationException _:
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid request";
                    problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                    break;
            }

            // Include error details in development
            if (_environment.IsDevelopment())
            {
                problemDetails.Detail = exception?.ToString();
                problemDetails.Extensions["stackTrace"] = exception?.StackTrace;
                problemDetails.Extensions["innerException"] = exception?.InnerException?.Message;
            }
            else
            {
                problemDetails.Detail = exception?.Message;
            }

            // Write the response
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";
            
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }

        private void LogRequestError(
            HttpContext context, 
            Exception exception, 
            long durationMs,
            string correlationId)
        {
            var request = context.Request;
            var user = context.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = user.FindFirst(ClaimTypes.Name)?.Value;
            
            var logMessage = new StringBuilder();
            logMessage.AppendLine($"Request Error [{correlationId}] [{request.Method} {request.Path}{request.QueryString}]");
            logMessage.AppendLine($"Status: {context.Response?.StatusCode}");
            logMessage.AppendLine($"Duration: {durationMs}ms");
            
            if (!string.IsNullOrEmpty(userId))
            {
                logMessage.AppendLine($"User: {userName} (ID: {userId})");
            }
            
            logMessage.AppendLine($"Remote IP: {context.Connection?.RemoteIpAddress}");
            
            if (exception != null)
            {
                logMessage.AppendLine($"Exception: {exception.GetType().Name}");
                logMessage.AppendLine($"Message: {exception.Message}");
                
                if (_environment.IsDevelopment())
                {
                    logMessage.AppendLine($"Stack Trace: {exception.StackTrace}");
                    
                    if (exception.InnerException != null)
                    {
                        logMessage.AppendLine($"Inner Exception: {exception.InnerException.GetType().Name}");
                        logMessage.AppendLine($"Inner Message: {exception.InnerException.Message}");
                    }
                }
            }
            
            // Log request headers (sanitized)
            logMessage.AppendLine("Headers:");
            foreach (var header in request.Headers)
            {
                // Skip sensitive headers
                if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase) ||
                    header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    logMessage.AppendLine($"  {header.Key}: [REDACTED]");
                    continue;
                }
                
                logMessage.AppendLine($"  {header.Key}: {string.Join(", ", header.Value.ToArray())}");
            }
            
            // Log query string if present
            if (request.QueryString.HasValue)
            {
                logMessage.AppendLine($"Query: {request.QueryString}");
            }
            
            // Log the error
            if (exception != null)
            {
                _logger.LogError(exception, logMessage.ToString());
            }
            else
            {
                _logger.LogError(new Exception("Request failed"), logMessage.ToString());
            }
        }
        
        private static string GetOrGenerateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId) && 
                !string.IsNullOrEmpty(correlationId))
            {
                return correlationId.ToString();
            }
            
            return Guid.NewGuid().ToString();
        }

        public class ProblemDetails
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }
            
            [JsonPropertyName("title")]
            public string Title { get; set; }
            
            [JsonPropertyName("status")]
            public int? Status { get; set; }
            
            [JsonPropertyName("detail")]
            public string Detail { get; set; }
            
            [JsonPropertyName("instance")]
            public string Instance { get; set; }
            
            [JsonExtensionData]
            public IDictionary<string, object> Extensions { get; set; } = new Dictionary<string, object>();
        }
    }
}
