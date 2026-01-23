namespace LensBackend.Models;

public class Lens
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string FocalLength { get; set; } = string.Empty;
    public int MinFocal { get; set; }
    public int MaxFocal { get; set; }
    public string Aperture { get; set; } = string.Empty;
    public string Compatibility { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool? IsPopular { get; set; }
}