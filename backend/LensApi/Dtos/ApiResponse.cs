using System.Text.Json.Serialization;

namespace LensApi.Dtos;

/// <summary>
/// Стандартный ответ API для ошибок и сообщений
/// </summary>
public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Успешный ответ с данными
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Ошибка с сообщением
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Ошибка валидации с деталями
    /// </summary>
    public static ApiResponse<T> ValidationErrorResponse(Dictionary<string, string[]> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Стандартный ответ API для empty responses
/// </summary>
public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse SuccessResponse(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse ErrorResponse(string message)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse ValidationErrorResponse(Dictionary<string, string[]> errors)
    {
        return new ApiResponse
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors,
            Timestamp = DateTime.UtcNow
        };
    }
}
