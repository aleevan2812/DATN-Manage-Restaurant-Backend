namespace Core.Entities;

public class Account : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }

    public int? OwnerId { get; set; }
}