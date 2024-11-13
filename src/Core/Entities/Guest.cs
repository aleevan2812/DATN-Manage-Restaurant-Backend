namespace Core.Entities;

public class Guest : BaseEntity
{
    public string? Name { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }

    public int? TableNumber { get; set; }
}