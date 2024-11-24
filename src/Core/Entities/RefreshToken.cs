using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class RefreshToken : BaseEntity
{
    public string? Token { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpiresAt { get; set; }

    public string? JwtTokenId { get; set; }

    [ForeignKey("Account")] public int AccountId { get; set; }

    public Account? Account { get; set; }
}