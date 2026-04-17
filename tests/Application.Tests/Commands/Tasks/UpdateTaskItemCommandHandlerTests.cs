using Application.Abstractions.Messaging;
using Application.Commands.Tasks;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Tasks;

public class UpdateTaskItemCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly UpdateTaskItemCommandHandler _handler;
    private readonly Mock<INotificationNotifier> _notificationNotifierMock;
    private readonly Mock<IAiSyncEnqueuer> _aiSyncEnqueuerMock;

    public UpdateTaskItemCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _notificationNotifierMock = new Mock<INotificationNotifier>();
        _aiSyncEnqueuerMock = new Mock<IAiSyncEnqueuer>();
        _handler = new UpdateTaskItemCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object, _notificationNotifierMock.Object, _aiSyncEnqueuerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateTaskItem()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var newStateId = 2;
        var newAssigneeId = Guid.NewGuid();
        var newDueDate = DateTime.UtcNow.AddDays(10);

        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = boardId,
            Title = "Old Title",
            Description = "Old Description",
            StateId = 1,
            AssigneeId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(5),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskItemCommand(
            Id: taskId,
            Title: "Updated Title",
            TagId: 1,
            Priority: 2,
            Description: "Updated Description",
            StateId: newStateId,
            AssigneeId: newAssigneeId,
            DueDate: newDueDate
        );

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(newStateId, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.Title.Should().Be(command.Title);
        existingTask.Description.Should().Be(command.Description);
        existingTask.StateId.Should().Be(command.StateId);
        existingTask.AssigneeId.Should().Be(command.AssigneeId);
        existingTask.DueDate.Should().Be(command.DueDate);
        existingTask.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTask_ShouldThrowNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new UpdateTaskItemCommand(
            Id: taskId,
            Title: "Updated Title",
            TagId: 1,
            Priority: 2,
            Description: "Updated Description",
            StateId: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(10)
        );

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with ID {taskId} was not found");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidStateForBoard_ShouldThrowValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var invalidStateId = 999;

        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = boardId,
            Title = "Old Title",
            StateId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskItemCommand(
            Id: taskId,
            Title: "Updated Title",
            TagId: 1,
            Priority: 2,
            Description: "Updated Description",
            StateId: invalidStateId,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(10)
        );

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(invalidStateId, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage($"State with ID {invalidStateId} is not valid for board {boardId}");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullOptionalFields_ShouldUpdateTaskWithNullValues()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var boardId = Guid.NewGuid();

        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = boardId,
            Title = "Old Title",
            Description = "Old Description",
            StateId = 1,
            AssigneeId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(5),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskItemCommand(
            Id: taskId,
            Title: "Updated Title",
            TagId: 1,
            Priority: 2,
            Description: null,
            StateId: 1,
            AssigneeId: null,
            DueDate: null
        );

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(1, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.Description.Should().BeNull();
        existingTask.AssigneeId.Should().BeNull();
        existingTask.DueDate.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldNotChangeCreatedAtTimestamp()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var originalCreatedAt = DateTime.UtcNow.AddDays(-5);

        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = boardId,
            Title = "Old Title",
            StateId = 1,
            CreatedAt = originalCreatedAt,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateTaskItemCommand(
            Id: taskId,
            Title: "Updated Title",
            Description: "Updated Description",
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(10)
        );

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(1, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingTask.CreatedAt.Should().Be(originalCreatedAt);
        existingTask.UpdatedAt.Should().NotBe(originalCreatedAt);
    }
}
