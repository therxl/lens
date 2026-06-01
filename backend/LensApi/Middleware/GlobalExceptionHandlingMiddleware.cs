using System.Text.Json;
using LensApi.Dtos;
using LensApi.Exceptions;

namespace LensApi.Middleware;

/// <summary>
/// Middleware для глобальной обработки исключений
/// </summary>
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
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResponse response = exception switch
        {
            ValidationException ve =>
                GetValidationErrorResponse(ve),
            
            ResourceNotFoundException rnfe =>
                GetNotFoundResponse(rnfe),
            
            DuplicateResourceException dre =>
                GetConflictResponse(dre),
            
            UnauthorizedException ue =>
                GetUnauthorizedResponse(context, ue),
            
            ForbiddenException fe =>
                GetForbiddenResponse(context, fe),
            
            _ =>
                GetInternalServerErrorResponse(context, exception)
        };

        context.Response.StatusCode = GetStatusCode(exception);
        return context.Response.WriteAsJsonAsync(response);
    }

    private static ApiResponse GetValidationErrorResponse(ValidationException ex)
    {
        return ApiResponse.ValidationErrorResponse(ex.Errors);
    }

    private static ApiResponse GetNotFoundResponse(ResourceNotFoundException ex)
    {
        return ApiResponse.ErrorResponse(ex.Message);
    }

    private static ApiResponse GetConflictResponse(DuplicateResourceException ex)
    {
        return ApiResponse.ErrorResponse(ex.Message);
    }

    private static ApiResponse GetUnauthorizedResponse(HttpContext context, UnauthorizedException ex)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return ApiResponse.ErrorResponse(ex.Message);
    }

    private static ApiResponse GetForbiddenResponse(HttpContext context, ForbiddenException ex)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return ApiResponse.ErrorResponse(ex.Message);
    }

    private static ApiResponse GetInternalServerErrorResponse(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return ApiResponse.ErrorResponse("An internal server error occurred. Please try again later.");
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            ResourceNotFoundException => StatusCodes.Status404NotFound,
            DuplicateResourceException => StatusCodes.Status409Conflict,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
}
