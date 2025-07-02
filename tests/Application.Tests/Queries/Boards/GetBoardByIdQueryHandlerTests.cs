using Application.Queries.Boards;
using Application.Common.Dtos.Boards;

namespace Application.Tests.Queries.Boards;

public class GetBoardByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly GetBoardByIdQueryHandler _handler;

    public GetBoardByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _handler = new GetBoardByIdQueryHandler(_boardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnBoardDto()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var expectedBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            Description = "Test Description",
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetBoardByIdQuery(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBoard);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(boardId);
        result.Name.Should().Be("Test Board");
        result.Description.Should().Be("Test Description");
        result.OwnerId.Should().Be(expectedBoard.OwnerId);
        result.CreatedAt.Should().Be(expectedBoard.CreatedAt);
        result.UpdatedAt.Should().Be(expectedBoard.UpdatedAt);

        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var query = new GetBoardByIdQuery(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var boardId = Guid.Parse(guidString);
        var query = new GetBoardByIdQuery(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithBoardHavingNullDescription_ShouldReturnDtoWithNullDescription()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var expectedBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            Description = null,
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetBoardByIdQuery(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBoard);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().BeNull();
    }
}
