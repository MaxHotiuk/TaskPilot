using Application.Abstractions.Messaging;
using Application.Commands.Boards;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Boards;

public class DeleteBoardCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly DeleteBoardCommandHandler _handler;

    public DeleteBoardCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Boards).Returns(_boardRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _handler = new DeleteBoardCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteBoard()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            Description = "Test Description",
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new DeleteBoardCommand(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _boardRepositoryMock.Verify(
            x => x.Remove(existingBoard), 
            Times.Once);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentBoard_ShouldThrowNotFoundException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new DeleteBoardCommand(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Board with id {boardId} was not found");

        _boardRepositoryMock.Verify(x => x.Remove(It.IsAny<Board>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var boardId = Guid.Parse(guidString);
        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Test Board",
            OwnerId = Guid.NewGuid()
        };

        var command = new DeleteBoardCommand(boardId);

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
