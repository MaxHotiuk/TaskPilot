using Application.Queries.States;
using Application.Common.Dtos.States;

namespace Application.Tests.Queries.States;

public class GetStatesByBoardIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IStateRepository> _stateRepositoryMock;
    private readonly GetStatesByBoardIdQueryHandler _handler;

    public GetStatesByBoardIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _stateRepositoryMock = _fixture.Freeze<Mock<IStateRepository>>();
        _handler = new GetStatesByBoardIdQueryHandler(_stateRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidBoardId_ShouldReturnStatesOrderedByOrder()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var states = new List<Domain.Entities.State>
        {
            new Domain.Entities.State
            {
                Id = 1,
                BoardId = boardId,
                Name = "To Do",
                Order = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Domain.Entities.State
            {
                Id = 2,
                BoardId = boardId,
                Name = "In Progress",
                Order = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Domain.Entities.State
            {
                Id = 3,
                BoardId = boardId,
                Name = "Done",
                Order = 3,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetStatesByBoardIdQuery(boardId);

        _stateRepositoryMock
            .Setup(x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(states);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(state => state.BoardId.Should().Be(boardId));

        // Verify order is maintained
        resultList[0].Name.Should().Be("To Do");
        resultList[0].Order.Should().Be(1);

        resultList[1].Name.Should().Be("In Progress");
        resultList[1].Order.Should().Be(2);

        resultList[2].Name.Should().Be("Done");
        resultList[2].Order.Should().Be(3);

        _stateRepositoryMock.Verify(
            x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithBoardHavingNoStates_ShouldReturnEmptyCollection()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var states = new List<Domain.Entities.State>();
        var query = new GetStatesByBoardIdQuery(boardId);

        _stateRepositoryMock
            .Setup(x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(states);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _stateRepositoryMock.Verify(
            x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificBoardIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var boardId = Guid.Parse(guidString);
        var query = new GetStatesByBoardIdQuery(boardId);

        _stateRepositoryMock
            .Setup(x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Domain.Entities.State>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _stateRepositoryMock.Verify(
            x => x.GetStatesByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
