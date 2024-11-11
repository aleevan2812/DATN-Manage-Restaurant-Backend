namespace Core.Dtos;

public class GuestDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Role { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TableNumber { get; set; }
}