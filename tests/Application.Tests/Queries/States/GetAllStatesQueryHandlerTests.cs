using Application.Queries.States;
using Application.Common.Dtos.States;

namespace Application.Tests.Queries.States;

public class GetAllStatesQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetAllStatesQueryHandler _handler;

    public GetAllStatesQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.States).Returns(_stateRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetAllStatesQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingStates_ShouldReturnAllStates()
    {
        // Arrange
        var board1Id = Guid.NewGuid();
        var board2Id = Guid.NewGuid();
        var states = new List<Domain.Entities.State>
        {
            new Domain.Entities.State
            {
                Id = 1,
                BoardId = board1Id,
                Name = "To Do",
                Order = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Domain.Entities.State
            {
                Id = 2,
                BoardId = board1Id,
                Name = "Done",
                Order = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            },
            new Domain.Entities.State
            {
                Id = 3,
                BoardId = board2Id,
                Name = "In Progress",
                Order = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllStatesQuery();

        _stateRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(states);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        var resultList = result.ToList();
        resultList.Should().Contain(s => s.Name == "To Do" && s.BoardId == board1Id);
        resultList.Should().Contain(s => s.Name == "Done" && s.BoardId == board1Id);
        resultList.Should().Contain(s => s.Name == "In Progress" && s.BoardId == board2Id);

        _stateRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoStates_ShouldReturnEmptyCollection()
    {
        // Arrange
        var states = new List<Domain.Entities.State>();
        var query = new GetAllStatesQuery();

        _stateRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(states);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _stateRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
