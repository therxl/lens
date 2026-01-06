namespace LensApi.Models;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int LensId { get; set; }
    public DateTime AddedAt { get; set; }
}