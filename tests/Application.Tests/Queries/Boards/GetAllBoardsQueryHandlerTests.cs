using Application.Queries.Boards;
using Application.Common.Dtos.Boards;

namespace Application.Tests.Queries.Boards;

public class GetAllBoardsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetAllBoardsQueryHandler _handler;

    public GetAllBoardsQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Boards).Returns(_boardRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetAllBoardsQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingBoards_ShouldReturnAllBoards()
    {
        // Arrange
        var boards = new List<Board>
        {
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Board 1",
                Description = "Description 1",
                OwnerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Board 2",
                Description = "Description 2",
                OwnerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllBoardsQuery();

        _boardRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList[0].Id.Should().Be(boards[0].Id);
        resultList[0].Name.Should().Be("Board 1");
        resultList[0].Description.Should().Be("Description 1");
        resultList[0].OwnerId.Should().Be(boards[0].OwnerId);

        resultList[1].Id.Should().Be(boards[1].Id);
        resultList[1].Name.Should().Be("Board 2");
        resultList[1].Description.Should().Be("Description 2");
        resultList[1].OwnerId.Should().Be(boards[1].OwnerId);

        _boardRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoBoards_ShouldReturnEmptyCollection()
    {
        // Arrange
        var boards = new List<Board>();
        var query = new GetAllBoardsQuery();

        _boardRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _boardRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithBoardsHavingNullDescriptions_ShouldHandleCorrectly()
    {
        // Arrange
        var boards = new List<Board>
        {
            new Board
            {
                Id = Guid.NewGuid(),
                Name = "Board with null description",
                Description = null,
                OwnerId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllBoardsQuery();

        _boardRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(boards);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var resultBoard = result.First();
        resultBoard.Description.Should().BeNull();
    }
}
