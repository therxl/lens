namespace LensApi.Controllers;

public record FavoriteResponse(int LensId, string LensName, decimal Price, DateTime AddedAt);
public record AddFavoriteRequest(int LensId);
public record ErrorResponse(string Message, string? ErrorCode = null);
