using Application.Abstractions.Messaging;
using Application.Commands.Tasks;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Tasks;

public class CreateTaskItemCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly CreateTaskItemCommandHandler _handler;
    private readonly Mock<INotificationNotifier> _notificationNotifierMock;

    public CreateTaskItemCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Boards).Returns(_boardRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _notificationNotifierMock = new Mock<INotificationNotifier>();
        _handler = new CreateTaskItemCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object, _notificationNotifierMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateTaskItemAndReturnId()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var stateId = 1;
        var dueDate = DateTime.UtcNow.AddDays(7);

        var command = new CreateTaskItemCommand(
            BoardId: boardId,
            Title: "Test Task",
            Description: "Test Description",
            StateId: stateId,
            TagId: 1,
            Priority: 2,
            AssigneeId: assigneeId,
            DueDate: dueDate
        );

        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            OwnerId = Guid.NewGuid()
        };

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(stateId, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.AddAsync(It.Is<TaskItem>(t =>
                t.BoardId == command.BoardId &&
                t.Title == command.Title &&
                t.Description == command.Description &&
                t.StateId == command.StateId &&
                t.AssigneeId == command.AssigneeId &&
                t.DueDate == command.DueDate &&
                t.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentBoard_ShouldThrowValidationException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateTaskItemCommand(
            BoardId: boardId,
            Title: "Test Task",
            Description: "Test Description",
            TagId: 1,
            Priority: 2,
            StateId: 1,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage($"Board with ID {boardId} does not exist");

        _taskItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidStateForBoard_ShouldThrowValidationException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var stateId = 1;
        var command = new CreateTaskItemCommand(
            BoardId: boardId,
            Title: "Test Task",
            Description: "Test Description",
            StateId: stateId,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            OwnerId = Guid.NewGuid()
        };

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(stateId, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage($"State with ID {stateId} is not valid for board {boardId}");

        _taskItemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullOptionalFields_ShouldCreateTaskWithNullValues()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var stateId = 1;

        var command = new CreateTaskItemCommand(
            BoardId: boardId,
            Title: "Test Task",
            TagId: 1,
            Priority: 2,
            Description: null,
            StateId: stateId,
            AssigneeId: null,
            DueDate: null
        );

        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            OwnerId = Guid.NewGuid()
        };

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(stateId, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.AddAsync(It.Is<TaskItem>(t =>
                t.Description == null &&
                t.AssigneeId == null &&
                t.DueDate == null
            ), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateTaskItemCommand(
            BoardId: boardId,
            Title: "Test Task",
            TagId: 1,
            Priority: 2,
            Description: "Test Description",
            StateId: 1,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        var existingBoard = new Board { Id = boardId, Name = "Test Board", OwnerId = Guid.NewGuid() };

        var beforeExecution = DateTime.UtcNow;

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        _stateRepositoryMock
            .Setup(x => x.IsValidStateForBoardAsync(1, boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.AddAsync(It.Is<TaskItem>(t =>
                t.CreatedAt >= beforeExecution &&
                t.CreatedAt <= afterExecution &&
                t.UpdatedAt >= beforeExecution &&
                t.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
