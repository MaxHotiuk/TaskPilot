using Application.Commands.States;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.States;

public class UpdateStateCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly UpdateStateCommandHandler _handler;

    public UpdateStateCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new UpdateStateCommandHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateState()
    {
        // Arrange
        var stateId = 1;
        var existingState = new Domain.Entities.State
        {
            Id = stateId,
            BoardId = Guid.NewGuid(),
            Name = "Old Name",
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateStateCommand(
            Id: stateId,
            Name: "Updated Name",
            Order: 2
        );

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState);

        _stateRepositoryMock
            .Setup(x => x.GetStateByBoardAndNameAsync(existingState.BoardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingState.Name.Should().Be(command.Name);
        existingState.Order.Should().Be(command.Order);
        existingState.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentState_ShouldThrowNotFoundException()
    {
        // Arrange
        var stateId = 1;
        var command = new UpdateStateCommand(
            Id: stateId,
            Name: "Updated Name",
            Order: 2
        );

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"State with ID {stateId} was not found");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotChangeCreatedAtTimestamp()
    {
        // Arrange
        var stateId = 1;
        var originalCreatedAt = DateTime.UtcNow.AddDays(-5);
        var existingState = new Domain.Entities.State
        {
            Id = stateId,
            BoardId = Guid.NewGuid(),
            Name = "Old Name",
            Order = 1,
            CreatedAt = originalCreatedAt,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateStateCommand(
            Id: stateId,
            Name: "Updated Name",
            Order: 3
        );

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState);

        _stateRepositoryMock
            .Setup(x => x.GetStateByBoardAndNameAsync(existingState.BoardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingState.CreatedAt.Should().Be(originalCreatedAt);
        existingState.UpdatedAt.Should().NotBe(originalCreatedAt);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Handle_WithDifferentOrders_ShouldUpdateStateWithCorrectOrder(int newOrder)
    {
        // Arrange
        var stateId = 1;
        var existingState = new Domain.Entities.State
        {
            Id = stateId,
            BoardId = Guid.NewGuid(),
            Name = "Test State",
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateStateCommand(
            Id: stateId,
            Name: "Test State",
            Order: newOrder
        );

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState);

        _stateRepositoryMock
            .Setup(x => x.GetStateByBoardAndNameAsync(existingState.BoardId, command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState); // Return the same state since it's the same ID

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingState.Order.Should().Be(newOrder);
    }
}
