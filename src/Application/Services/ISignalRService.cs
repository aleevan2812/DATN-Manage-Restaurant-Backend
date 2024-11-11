namespace Application.Services;

public interface ISignalRService
{
    Task SendMessage(string method, object order);
}