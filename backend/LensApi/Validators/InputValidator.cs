using LensApi.Exceptions;

namespace LensApi.Validators;

/// <summary>
/// Утилита для валидации входных данных
/// </summary>
public static class InputValidator
{
    /// <summary>
    /// Проверить, что ID положительный
    /// </summary>
    public static void ValidatePositiveId(int id, string fieldName = "id")
    {
        if (id <= 0)
        {
            throw new ValidationException(fieldName, $"{fieldName} must be a positive number");
        }
    }

    /// <summary>
    /// Проверить, что строка не пуста
    /// </summary>
    public static void ValidateNotEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(fieldName, $"{fieldName} cannot be empty");
        }
    }

    /// <summary>
    /// Проверить длину строки
    /// </summary>
    public static void ValidateLength(string? value, string fieldName, int minLength, int? maxLength = null)
    {
        ValidateNotEmpty(value, fieldName);
        
        if (value!.Length < minLength)
        {
            throw new ValidationException(fieldName, $"{fieldName} must be at least {minLength} characters long");
        }

        if (maxLength.HasValue && value.Length > maxLength)
        {
            throw new ValidationException(fieldName, $"{fieldName} must not exceed {maxLength} characters");
        }
    }

    /// <summary>
    /// Проверить, что значение в диапазоне
    /// </summary>
    public static void ValidateRange(decimal value, decimal min, decimal max, string fieldName)
    {
        if (value < min || value > max)
        {
            throw new ValidationException(fieldName, $"{fieldName} must be between {min} and {max}");
        }
    }

    /// <summary>
    /// Проверить, что значение из допустимого списка
    /// </summary>
    public static void ValidateEnum<T>(string value, string fieldName) where T : struct, Enum
    {
        if (!Enum.TryParse<T>(value, ignoreCase: true, out _))
        {
            var validValues = string.Join(", ", Enum.GetNames(typeof(T)));
            throw new ValidationException(fieldName, $"{fieldName} must be one of: {validValues}");
        }
    }

    /// <summary>
    /// Проверить, что GUID валиден
    /// </summary>
    public static void ValidateGuid(string? value, string fieldName)
    {
        ValidateNotEmpty(value, fieldName);
        
        if (!Guid.TryParse(value, out _))
        {
            throw new ValidationException(fieldName, $"{fieldName} must be a valid GUID");
        }
    }

    /// <summary>
    /// Проверить, что email валиден
    /// </summary>
    public static void ValidateEmail(string? email, string fieldName = "email")
    {
        ValidateNotEmpty(email, fieldName);
        
        try
        {
            var addr = new System.Net.Mail.MailAddress(email!);
            if (addr.Address != email)
            {
                throw new ValidationException(fieldName, $"{fieldName} must be a valid email address");
            }
        }
        catch
        {
            throw new ValidationException(fieldName, $"{fieldName} must be a valid email address");
        }
    }
}
