using Application.Queries.Boards;
using Application.Common.Dtos.Boards;

namespace Application.Tests.Queries.Boards;

public class GetBoardsByOwnerIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GetBoardsByOwnerIdQueryHandler _handler;

    public GetBoardsByOwnerIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _handler = new GetBoardsByOwnerIdQueryHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidOwnerId_ShouldReturnOwnerBoards()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boards = new List<Board>
        {
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Owned Board 1",
                Description = "Description 1",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Owned Board 2",
                Description = "Description 2",
                OwnerId = ownerId,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetBoardsByOwnerIdQuery(ownerId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(board => board.OwnerId.Should().Be(ownerId));

        resultList[0].Name.Should().Be("Owned Board 1");
        resultList[1].Name.Should().Be("Owned Board 2");

        _boardRepositoryMock.Verify(
            x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentOwnerId_ShouldReturnEmptyCollection()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var boards = new List<Board>();
        var query = new GetBoardsByOwnerIdQuery(ownerId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _boardRepositoryMock.Verify(
            x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificOwnerIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var ownerId = Guid.Parse(guidString);
        var query = new GetBoardsByOwnerIdQuery(ownerId);

        _boardRepositoryMock
            .Setup(x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Board>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _boardRepositoryMock.Verify(
            x => x.GetBoardsByOwnerIdAsync(ownerId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
