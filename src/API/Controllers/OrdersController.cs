using Application.Features.Order;
using Core.Const;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> GetOrdersByDate([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var res = await _sender.Send(new GetOrdersByDateQuery
        {
            FromDate = fromDate,
            ToDate = toDate
        });
        return Ok(res);
    }

    [HttpGet("revenue")]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> GetOrdersAndRevenue()
    {
        var res = await _sender.Send(new GetOrdersAndRevenueQuery());
        return Ok(res);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var res = await _sender.Send(new GetOrderByIdQuery
        {
            Id = id
        });
        return Ok(res);
    }

    [HttpPost]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> CreateOrder(CreateOrderCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> CreateOrder(int id, UpdateOrderCommand cmd) //emp
    {
        cmd.OrderId = id;
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpPost("pay")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> PayOrdersByGuestId(PayOrdersByGuestIdCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpPost("reject")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> RejectOrdersByGuestId(RejectOrdersByGuestIdCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }
}