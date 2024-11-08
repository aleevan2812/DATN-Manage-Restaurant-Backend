using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Order : BaseEntity
{
    public int Quantity { get; set; }
    public string? Status { get; set; }

    public int GuestId { get; set; }
    [ForeignKey("GuestId")]
    public Guest? Guest { get; set; }

    public int TableNumber { get; set; }
    [ForeignKey("TableNumber")]
    public Table? Table { get; set; }

    public int DishSnapshotId { get; set; }
    [ForeignKey("DishSnapshotId")]
    public DishSnapshot? DishSnapshot { get; set; }

    public int OrderHandlerId { get; set; }
    [ForeignKey("OrderHandlerId")]
    public Account? Account { get; set; }
}