using Application.Features.Account.Commands;
using Application.Features.Account.Queries;
using Core.Const;
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
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
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

    [HttpPost]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeCommand cmd) //emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpGet]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> GetEmployees() // emp
    {
        var res = await _sender.Send(new GetEmployeesQuery());
        return Ok(res);
    }

    [HttpGet("detail/{id}")]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> GetDetailAccount(int id) // emp
    {
        var res = await _sender.Send(new GetDetailAccountQuery(id));
        return Ok(res);
    }

    [HttpPut("detail/{id}")]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> UpdateDetailAccount(int id, UpdateEmployeeDetailCommand command) // emp
    {
        command.Id = id;
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpDelete("detail/{id}")]
    [Authorize(Roles = Role.Owner)]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var res = await _sender.Send(new DeleteEmployeeCommand(id));
        return Ok(res);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpGet("guests")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult>
        GetGuestsByDate([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate) // emp
    {
        var res = await _sender.Send(new GetGuestsByDateQuery { FromDate = fromDate, ToDate = toDate });
        return Ok(res);
    }

    [HttpPost("guests")]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> CreateGuest(CreateGuestCommand cmd) // emp
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }
}