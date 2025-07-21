using Application.Abstractions.Messaging;
using Application.Commands.Comments;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Comments;

public class CreateCommentCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly CreateCommentCommandHandler _handler;

    public CreateCommentCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _commentRepositoryMock = _fixture.Freeze<Mock<ICommentRepository>>();
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Comments).Returns(_commentRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _handler = new CreateCommentCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateCommentAndReturnId()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            TaskId: taskId,
            AuthorId: authorId,
            Content: "Test comment content"
        );

        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            BoardId = Guid.NewGuid()
        };

        var existingUser = new User
        {
            Id = authorId,
            Email = "test@example.com",
            Username = "testuser"
        };

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _commentRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Comment>(c =>
                c.TaskId == command.TaskId &&
                c.AuthorId == command.AuthorId &&
                c.Content == command.Content &&
                c.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTask_ShouldThrowValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            TaskId: taskId,
            AuthorId: authorId,
            Content: "Test comment content"
        );

        var existingUser = new User
        {
            Id = authorId,
            Email = "test@example.com",
            Username = "testuser"
        };

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage($"Task with ID {taskId} does not exist");

        _commentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNonExistentAuthor_ShouldThrowValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            TaskId: taskId,
            AuthorId: authorId,
            Content: "Test comment content"
        );

        var existingTask = new TaskItem
        {
            Id = taskId,
            Title = "Test Task",
            BoardId = Guid.NewGuid()
        };

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage($"User with ID {authorId} does not exist");

        _commentRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var command = new CreateCommentCommand(
            TaskId: taskId,
            AuthorId: authorId,
            Content: "Test comment content"
        );

        var existingTask = new TaskItem { Id = taskId, Title = "Test Task", BoardId = Guid.NewGuid() };
        var existingUser = new User { Id = authorId, Email = "test@example.com", Username = "testuser" };

        var beforeExecution = DateTime.UtcNow;

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _commentRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Comment>(c =>
                c.CreatedAt >= beforeExecution &&
                c.CreatedAt <= afterExecution &&
                c.UpdatedAt >= beforeExecution &&
                c.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
