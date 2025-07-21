namespace Application.Abstractions.Messaging;

using System.Threading.Tasks;
using Domain.Entities;

public interface INotificationNotifier
{
    Task NotifyUserAsync(Guid userId, Notification notification);
}
