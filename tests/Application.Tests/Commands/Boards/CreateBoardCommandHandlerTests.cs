using Application.Commands.Boards;

namespace Application.Tests.Commands.Boards;

public class CreateBoardCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBoardRepository> _boardRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateBoardCommandHandler _handler;

    public CreateBoardCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _boardRepositoryMock = _fixture.Freeze<Mock<IBoardRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _handler = new CreateBoardCommandHandler(_boardRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateBoardAndReturnId()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.NewGuid()
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.Name == command.Name &&
                b.Description == command.Description &&
                b.OwnerId == command.OwnerId &&
                b.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ShouldCreateBoardWithNullDescription()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: null,
            OwnerId: Guid.NewGuid()
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.Name == command.Name &&
                b.Description == null &&
                b.OwnerId == command.OwnerId
            ), It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.NewGuid()
        );

        var beforeExecution = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _boardRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Board>(b => 
                b.CreatedAt >= beforeExecution &&
                b.CreatedAt <= afterExecution &&
                b.UpdatedAt >= beforeExecution &&
                b.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
