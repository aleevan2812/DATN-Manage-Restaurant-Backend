namespace Core.Entities;

public class Table : BaseEntity
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
}