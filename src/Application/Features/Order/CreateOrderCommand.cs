namespace Application.Features.Order;

public class CreateOrderCommand
{
    public int GuestId { get; set; }
    public List<OrderInformationRequest>? Orders { get; set; }
}

public class OrderInformationRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
}