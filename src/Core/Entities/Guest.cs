using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Guest : BaseEntity
{
    public string? Name { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    
    public int TableNumber { get; set; }
    [ForeignKey("TableNumber")]
    public Table? Table { get; set; }
}