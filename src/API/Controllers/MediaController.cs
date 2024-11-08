using Application.Features.Media.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class MediaController : ControllerBase
{
    private readonly ISender _sender;

    public MediaController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("upload")]
    [AllowAnonymous]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var res = await _sender.Send(new UploadCommand(file));
        return Ok(res);
    }
}