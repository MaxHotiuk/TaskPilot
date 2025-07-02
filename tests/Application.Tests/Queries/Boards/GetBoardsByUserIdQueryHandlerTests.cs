using Application.Queries.Boards;
using Application.Common.Dtos.Boards;

namespace Application.Tests.Queries.Boards;

public class GetBoardsByUserIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GetBoardsByUserIdQueryHandler _handler;

    public GetBoardsByUserIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _handler = new GetBoardsByUserIdQueryHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ShouldReturnAccessibleBoards()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var boards = new List<Board>
        {
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Accessible Board 1",
                Description = "Description 1",
                OwnerId = userId, // User owns this board
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Accessible Board 2",
                Description = "Description 2",
                OwnerId = Guid.NewGuid(), // User is member of this board
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetBoardsByUserIdQuery(userId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList[0].Name.Should().Be("Accessible Board 1");
        resultList[1].Name.Should().Be("Accessible Board 2");

        _boardRepositoryMock.Verify(
            x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithUserHavingNoBoards_ShouldReturnEmptyCollection()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var boards = new List<Board>();
        var query = new GetBoardsByUserIdQuery(userId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _boardRepositoryMock.Verify(
            x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificUserIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var userId = Guid.Parse(guidString);
        var query = new GetBoardsByUserIdQuery(userId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Board>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _boardRepositoryMock.Verify(
            x => x.GetBoardsByUserIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
