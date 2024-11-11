using Application.Features.Guest;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private readonly ISender _sender;

    public GuestController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("auth/login")]
    public async Task<IActionResult> LoginGuest(LoginGuestCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpPost("auth/refresh-token")]
    public async Task<IActionResult> LoginGuest(RefreshTokenCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrderByGuest(List<GuestCreateOrderRequest>? orders) //emp
    {
        var res = await _sender.Send(new GuestCreateOrderCommand
        {
            Orders = orders
        });
        return Ok(res);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetGuestOrders() //emp
    {
        var res = await _sender.Send(new GetGuestOrderQuery());
        return Ok(res);
    }
}