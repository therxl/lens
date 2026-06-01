using RecommendationsApi.Exceptions;

namespace RecommendationsApi.Validators;

/// <summary>
/// Утилита для валидации входных данных
/// </summary>
public static class InputValidator
{
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
}
