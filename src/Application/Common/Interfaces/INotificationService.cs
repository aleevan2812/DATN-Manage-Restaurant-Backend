using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface INotificationService
{
    Task SendMessage(string method, object order);
}