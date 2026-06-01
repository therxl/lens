namespace LensApi.Exceptions;

/// <summary>
/// Исключение для ошибок валидации
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string fieldName, string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { fieldName, new[] { message } }
        };
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed")
    {
        Errors = errors;
    }
}

/// <summary>
/// Исключение для не найденных ресурсов
/// </summary>
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceName, object id)
        : base($"{resourceName} with id {id} not found")
    {
    }

    public ResourceNotFoundException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Исключение для дублирующихся ресурсов
/// </summary>
public class DuplicateResourceException : Exception
{
    public DuplicateResourceException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Исключение для ошибок авторизации
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Unauthorized")
        : base(message)
    {
    }
}

/// <summary>
/// Исключение для ошибок доступа
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Forbidden")
        : base(message)
    {
    }
}
