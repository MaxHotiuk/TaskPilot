using Application.Commands.States;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.States;

public class DeleteStateCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly DeleteStateCommandHandler _handler;

    public DeleteStateCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new DeleteStateCommandHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteState()
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

        var command = new DeleteStateCommand(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);

        _stateRepositoryMock.Verify(
            x => x.Remove(existingState),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentState_ShouldThrowNotFoundException()
    {
        // Arrange
        var stateId = 1;
        var command = new DeleteStateCommand(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"State with ID {stateId} was not found");

        _stateRepositoryMock.Verify(x => x.Remove(It.IsAny<Domain.Entities.State>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Handle_WithSpecificIds_ShouldCallRepositoryWithCorrectId(int stateId)
    {
        // Arrange
        var existingState = new Domain.Entities.State
        {
            Id = stateId,
            BoardId = Guid.NewGuid(),
            Name = "Test State",
            Order = 1
        };

        var command = new DeleteStateCommand(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingState);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
