using Application.Features.Guest;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services;

public class NotificationHub : Hub
{
}

public class SignalRService : ISignalRService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(string method, GuestCreateOrderCommandResponse order)
    {
        await _hubContext.Clients.All.SendAsync(method, order);
    }
}