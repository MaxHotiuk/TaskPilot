using Application.Commands.Boards;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Boards;

public class UpdateBoardCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateBoardCommandHandler _handler;

    public UpdateBoardCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _handler = new UpdateBoardCommandHandler(_boardRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateBoard()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Old Name",
            Description = "Old Description",
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateBoardCommand(
            Id: boardId,
            Name: "Updated Name",
            Description: "Updated Description"
        );

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingBoard.Name.Should().Be(command.Name);
        existingBoard.Description.Should().Be(command.Description);
        existingBoard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _boardRepositoryMock.Verify(
            x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentBoard_ShouldThrowNotFoundException()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var command = new UpdateBoardCommand(
            Id: boardId,
            Name: "Updated Name",
            Description: "Updated Description"
        );

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Board?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Board with id {boardId} was not found");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ShouldUpdateBoardWithNullDescription()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var existingBoard = new Board
        {
            Id = boardId,
            Name = "Old Name",
            Description = "Old Description",
            OwnerId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateBoardCommand(
            Id: boardId,
            Name: "Updated Name",
            Description: null
        );

        _boardRepositoryMock
            .Setup(x => x.GetByIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBoard);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingBoard.Name.Should().Be(command.Name);
        existingBoard.Description.Should().BeNull();
        existingBoard.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
