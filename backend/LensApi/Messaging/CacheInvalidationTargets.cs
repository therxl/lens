namespace LensApi.Messaging;

public static class CacheInvalidationTargets
{
    public static IReadOnlyCollection<string> RecommendationKeys { get; } =
        new[] { "recommendations:portrait", "recommendations:landscape", "recommendations:macro", "recommendations:sports" };
}