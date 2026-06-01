namespace RecommendationsApi.Messaging;

public sealed record CacheInvalidationMessage(
    string Reason,
    IReadOnlyCollection<string> CacheKeys,
    string? UserId,
    int? LensId,
    DateTimeOffset OccurredAtUtc);