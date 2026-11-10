using System.Net;
using System.Text.Json;
using AgriStress.Application.Exceptions;

namespace AgriStress.API.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İşlem sırasında yakalanmamış bir hata oluştu: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Sunucu tarafında beklenmeyen bir hata oluştu.";

        if (exception is NotFoundException notFoundEx)
        {
            statusCode = HttpStatusCode.NotFound;
            message = notFoundEx.Message;
        }
        else if (exception is BusinessException businessEx)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = businessEx.Message;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = (int)statusCode,
            Message = message,
            Detailed = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
