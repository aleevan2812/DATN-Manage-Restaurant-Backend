using Application.Features.Dish;
using Core.Const;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class DishesController : ControllerBase
{
    private readonly ISender _sender;

    public DishesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Authorize(Roles = Role.Owner + "," + Role.Employee)]
    public async Task<IActionResult> CreateDish(CreateDishCommand cmd)
    {
        var res = await _sender.Send(cmd);
        return Ok(res);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetDishes()
    {
        var res = await _sender.Send(new GetDishesQuery());
        return Ok(res);
    }
}