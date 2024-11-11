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
}