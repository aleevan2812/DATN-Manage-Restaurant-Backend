using Application.Features.Table.Commands;
using Application.Features.Table.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class TablesController : ControllerBase
{
    private readonly ISender _sender;

    public TablesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTable(CreateTableCommand cmd)
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpGet]
    public async Task<IActionResult> GetTables()
    {
        var res = await _sender.Send(new GetTablesQuery());
        return Ok(res);
    }

    [HttpGet("{number}")]
    public async Task<IActionResult> GetTableByNumber(int number)
    {
        var res = await _sender.Send(new GetTableByNumberQuery(number));
        return Ok(res);
    }

    [HttpPut("{number}")]
    [Authorize]
    public async Task<IActionResult> UpdateTableByNumber(int number, UpdateTableCommand cmd)
    {
        cmd.Number = number;
        var res = await _sender.Send(cmd);
        return Ok(res);
    }

    [HttpDelete("{number}")]
    [Authorize]
    public async Task<IActionResult> DeleteTableByNumber(int number)
    {
        var res = await _sender.Send(new DeleteTableByNumberCommand(number));
        return Ok(res);
    }
}