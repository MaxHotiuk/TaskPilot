using Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using Persistence.Repositories;
using Database;

namespace Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IDbContextTransaction _transaction;
    private bool _disposed;

    private IUserRepository? _users;
    private IBoardRepository? _boards;
    private ITaskItemRepository? _tasks;
    private ITagRepository? _tags;
    private IStateRepository? _states;
    private IBoardMemberRepository? _boardMembers;
    private ICommentRepository? _comments;
    private INotificationRepository? _notifications;
    private IBacklogRepository? _backlogs;
    private IMeetingMemberRepository? _meetingMembers;
    private IMeetingRepository? _meetings;
    private IOrganizationRepository? _organizations;
    private IOrganizationMemberRepository? _organizationMembers;
    private IChatRepository? _chats;
    private IChatMemberRepository? _chatMembers;
    private IChatMessageRepository? _chatMessages;
    private IBoardInvitationRepository? _boardInvitations;
    private IOrganizationInvitationRepository? _organizationInvitations;

    public UnitOfWork(ApplicationDbContext context, IDbContextTransaction transaction)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IBoardRepository Boards => _boards ??= new BoardRepository(_context);
    public ITaskItemRepository Tasks => _tasks ??= new TaskItemRepository(_context);
    public ITagRepository Tags => _tags ??= new TagRepository(_context);
    public IStateRepository States => _states ??= new StateRepository(_context);
    public IBoardMemberRepository BoardMembers => _boardMembers ??= new BoardMemberRepository(_context);
    public ICommentRepository Comments => _comments ??= new CommentRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
    public IBacklogRepository Backlogs => _backlogs ??= new BacklogRepository(_context);
    public IMeetingMemberRepository MeetingMembers => _meetingMembers ??= new MeetingMemberRepository(_context);
    public IMeetingRepository Meetings => _meetings ??= new MeetingRepository(_context);
    public IOrganizationRepository Organizations => _organizations ??= new OrganizationRepository(_context);
    public IOrganizationMemberRepository OrganizationMembers => _organizationMembers ??= new OrganizationMemberRepository(_context);
    public IChatRepository Chats => _chats ??= new ChatRepository(_context);
    public IChatMemberRepository ChatMembers => _chatMembers ??= new ChatMemberRepository(_context);
    public IChatMessageRepository ChatMessages => _chatMessages ??= new ChatMessageRepository(_context);
    public IBoardInvitationRepository BoardInvitations => _boardInvitations ??= new BoardInvitationRepository(_context);
    public IOrganizationInvitationRepository OrganizationInvitations => _organizationInvitations ??= new OrganizationInvitationRepository(_context);

    public bool HasActiveTransaction => _transaction != null && !_disposed;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null && !_disposed)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null && !_disposed)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
