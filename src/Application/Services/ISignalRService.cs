using Application.Features.Guest;

namespace Application.Services;

public interface ISignalRService
{
    Task SendMessage(string method, GuestCreateOrderCommandResponse order);
}