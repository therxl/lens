namespace RecommendationsApi.Models;

/// <summary>
/// История выбора пользователем типов съемки
/// </summary>
public class SelectionHistory
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public string ShootingType { get; set; } = null!; // portrait, landscape, macro, sports
    public int? SelectedLensId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Тип съемки с описанием оптимальных характеристик объектива
/// </summary>
public class ShootingTypeProfile
{
    public string Type { get; set; } = null!; // portrait, landscape, macro, sports
    public string Description { get; set; } = null!;
    public List<string> PreferredBrands { get; set; } = [];
    public int? PreferredApertureMin { get; set; } // f/1.4 = 1.4, f/2.8 = 2.8
    public int? PreferredFocalMin { get; set; }
    public int? PreferredFocalMax { get; set; }
}

/// <summary>
/// Ответ с рекомендациями
/// </summary>
public class RecommendationResponse
{
    public string ShootingType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<RecommendedLens> Recommendations { get; set; } = [];
}

/// <summary>
/// Рекомендуемый объектив
/// </summary>
public class RecommendedLens
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string FocalLength { get; set; } = null!;
    public string Aperture { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string MatchReason { get; set; } = null!; // Почему рекомендуется
}

public class ErrorResponse
{
    public ErrorResponse() { }
    public ErrorResponse(string message, string code)
    {
        Message = message;
        Code = code;
    }

    public string Message { get; set; } = null!;
    public string Code { get; set; } = null!;
}
