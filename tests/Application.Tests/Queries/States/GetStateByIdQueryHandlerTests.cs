using Application.Queries.States;
using Domain.Dtos.States;

namespace Application.Tests.Queries.States;

public class GetStateByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetStateByIdQueryHandler _handler;

    public GetStateByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetStateByIdQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnStateDto()
    {
        // Arrange
        var stateId = 1;
        var boardId = Guid.NewGuid();
        var expectedState = new Domain.Entities.State
        {
            Id = stateId,
            BoardId = boardId,
            Name = "Test State",
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetStateByIdQuery(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedState);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(stateId);
        result.BoardId.Should().Be(boardId);
        result.Name.Should().Be("Test State");
        result.Order.Should().Be(1);
        result.CreatedAt.Should().Be(expectedState.CreatedAt);
        result.UpdatedAt.Should().Be(expectedState.UpdatedAt);

        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var stateId = 1;
        var query = new GetStateByIdQuery(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Handle_WithSpecificIds_ShouldCallRepositoryWithCorrectId(int stateId)
    {
        // Arrange
        var query = new GetStateByIdQuery(stateId);

        _stateRepositoryMock
            .Setup(x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.State?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _stateRepositoryMock.Verify(
            x => x.GetByIdAsync(stateId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
