namespace Application.Abstractions.Messaging;

using System.Threading.Tasks;

public interface IBoardNotifier
{
    Task NotifyBoardUpdatedAsync(string boardId, object payload);
    Task NotifyTaskUpdatedAsync(string taskId, object payload);
}
