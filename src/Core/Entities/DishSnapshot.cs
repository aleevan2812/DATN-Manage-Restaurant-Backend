namespace Core.Entities;

public class DishSnapshot : BaseEntity
{
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
    public int DishId { get; set; }
}