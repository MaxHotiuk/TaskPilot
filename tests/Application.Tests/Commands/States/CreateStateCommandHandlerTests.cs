using Application.Commands.States;

namespace Application.Tests.Commands.States;

public class CreateStateCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly CreateStateCommandHandler _handler;

    public CreateStateCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.Boards).Returns(_boardRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new CreateStateCommandHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateStateAndReturnId()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateStateCommand(
            BoardId: boardId,
            Name: "To Do",
            Order: 1
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
            .Setup(x => x.GetStateByBoardAndNameAsync(boardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        _stateRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.State>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.State, CancellationToken>((state, _) => state.Id = 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        _stateRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.State>(s =>
                s.BoardId == command.BoardId &&
                s.Name == command.Name &&
                s.Order == command.Order &&
                s.Id > 0
            ), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateStateCommand(
            BoardId: boardId,
            Name: "In Progress",
            Order: 2
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
            .Setup(x => x.GetStateByBoardAndNameAsync(boardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        _stateRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.State>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.State, CancellationToken>((state, _) => state.Id = 2);

        var beforeExecution = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _stateRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.State>(s =>
                s.CreatedAt >= beforeExecution &&
                s.CreatedAt <= afterExecution &&
                s.UpdatedAt >= beforeExecution &&
                s.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Handle_WithDifferentOrders_ShouldCreateStateWithCorrectOrder(int order)
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateStateCommand(
            BoardId: boardId,
            Name: $"State {order}",
            Order: order
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
            .Setup(x => x.GetStateByBoardAndNameAsync(boardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        _stateRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.State>(), It.IsAny<CancellationToken>()))
            .Callback<Domain.Entities.State, CancellationToken>((state, _) => state.Id = order);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);

        _stateRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Domain.Entities.State>(s =>
                s.Order == order
            ), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentBoard_ShouldThrowValidationException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new CreateStateCommand(
            BoardId: boardId,
            Name: "To Do",
            Order: 1
        );

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Application.Common.Exceptions.ValidationException>()
            .WithMessage($"Board with ID {boardId} does not exist");

        _stateRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.State>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
