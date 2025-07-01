using Domain.Entities;

namespace Persistence.Tests.Builders;

public class TaskItemBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _boardId = Guid.NewGuid();
    private string _title = "Test Task";
    private string? _description = "Test Description";
    private int _stateId = 1;
    private Guid? _assigneeId = null;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private DateTime? _dueDate = null;

    public TaskItemBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public TaskItemBuilder WithBoardId(Guid boardId)
    {
        _boardId = boardId;
        return this;
    }

    public TaskItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public TaskItemBuilder WithStateId(int stateId)
    {
        _stateId = stateId;
        return this;
    }

    public TaskItemBuilder WithAssigneeId(Guid? assigneeId)
    {
        _assigneeId = assigneeId;
        return this;
    }

    public TaskItemBuilder WithDueDate(DateTime? dueDate)
    {
        _dueDate = dueDate;
        return this;
    }

    public TaskItemBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public TaskItemBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public TaskItem Build()
    {
        return new TaskItem
        {
            Id = _id,
            BoardId = _boardId,
            Title = _title,
            Description = _description,
            StateId = _stateId,
            AssigneeId = _assigneeId,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            DueDate = _dueDate
        };
    }

    public static TaskItemBuilder Create() => new();

    public static TaskItemBuilder CreateOverdueTask()
    {
        return new TaskItemBuilder()
            .WithDueDate(DateTime.UtcNow.AddDays(-5))
            .WithTitle("Overdue Task");
    }

    public static TaskItemBuilder CreateTaskForBoard(Guid boardId)
    {
        return new TaskItemBuilder()
            .WithBoardId(boardId);
    }

    public static TaskItemBuilder CreateTaskWithAssignee(Guid assigneeId)
    {
        return new TaskItemBuilder()
            .WithAssigneeId(assigneeId);
    }
}

public class UserBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _entraId = Guid.NewGuid().ToString();
    private string _email = "test@example.com";
    private string _username = "testuser";
    private string _role = "User";
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public UserBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UserBuilder WithEntraId(string entraId)
    {
        _entraId = entraId;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public UserBuilder WithRole(string role)
    {
        _role = role;
        return this;
    }

    public User Build()
    {
        return new User
        {
            Id = _id,
            EntraId = _entraId,
            Email = _email,
            Username = _username,
            Role = _role,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
    }

    public static UserBuilder Create() => new();
}

public class BoardBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Board";
    private string _description = "Test Description";
    private Guid _ownerId = Guid.NewGuid();
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public BoardBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BoardBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BoardBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public BoardBuilder WithOwnerId(Guid ownerId)
    {
        _ownerId = ownerId;
        return this;
    }

    public Board Build()
    {
        return new Board
        {
            Id = _id,
            Name = _name,
            Description = _description,
            OwnerId = _ownerId,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
    }

    public static BoardBuilder Create() => new();
}

public class CommentBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _taskId = Guid.NewGuid();
    private Guid _authorId = Guid.NewGuid();
    private string _content = "Test comment";
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public CommentBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CommentBuilder WithTaskId(Guid taskId)
    {
        _taskId = taskId;
        return this;
    }

    public CommentBuilder WithAuthorId(Guid authorId)
    {
        _authorId = authorId;
        return this;
    }

    public CommentBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public CommentBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public Comment Build()
    {
        return new Comment
        {
            Id = _id,
            TaskId = _taskId,
            AuthorId = _authorId,
            Content = _content,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
    }

    public static CommentBuilder Create() => new();
}

public class StateBuilder
{
    private int _id = 1;
    private Guid _boardId = Guid.NewGuid();
    private string _name = "Test State";
    private int _order = 1;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public StateBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public StateBuilder WithBoardId(Guid boardId)
    {
        _boardId = boardId;
        return this;
    }

    public StateBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public StateBuilder WithOrder(int order)
    {
        _order = order;
        return this;
    }

    public StateBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public StateBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public State Build()
    {
        return new State
        {
            Id = _id,
            BoardId = _boardId,
            Name = _name,
            Order = _order,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt
        };
    }

    public static StateBuilder Create() => new();

    public static StateBuilder CreateForBoard(Guid boardId)
    {
        return new StateBuilder()
            .WithBoardId(boardId);
    }
}
