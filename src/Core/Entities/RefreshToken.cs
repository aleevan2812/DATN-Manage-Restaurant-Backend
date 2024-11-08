namespace Core.Entities;

public class RefreshToken : BaseEntity
{
    public int AccountId { get; set; }
    public string Token { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpiresAt { get; set; }
}