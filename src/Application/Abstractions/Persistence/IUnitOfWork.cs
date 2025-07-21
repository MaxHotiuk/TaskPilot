namespace Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBoardRepository Boards { get; }
    ITaskItemRepository Tasks { get; }
    IStateRepository States { get; }
    IBoardMemberRepository BoardMembers { get; }
    ICommentRepository Comments { get; }
    INotificationRepository Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    bool HasActiveTransaction { get; }
}
