using Application.Features.Account.Commands;
using Application.Features.Account.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ISender _sender;

    public AccountsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetUserProfile()
    {
        var userId = User.FindFirst("userId")?.Value;
        var res = await _sender.Send(new GetUserProfileQuery(int.Parse(userId)));
        return Ok(res);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateMeCommand cmd)
    {
        var userId = int.Parse(User.FindFirst("userId")?.Value);
        var res = await _sender.Send(new UpdateMeCommand(userId, cmd.Name, cmd.Avatar));
        return Ok(res);
    }
}