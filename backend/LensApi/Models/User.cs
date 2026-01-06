namespace LensApi.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string Mode { get; set; } = "user";
    public DateTime CreatedAt { get; set; }
}