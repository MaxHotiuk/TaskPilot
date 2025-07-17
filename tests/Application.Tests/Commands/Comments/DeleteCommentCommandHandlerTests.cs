using Application.Abstractions.Messaging;
using Application.Commands.Comments;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Comments;

public class DeleteCommentCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IBoardNotifier> _boardNotifierMock;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _commentRepositoryMock = _fixture.Freeze<Mock<ICommentRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Comments).Returns(_commentRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _boardNotifierMock = _fixture.Freeze<Mock<IBoardNotifier>>();
        _handler = new DeleteCommentCommandHandler(_unitOfWorkFactoryMock.Object, _boardNotifierMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var existingComment = new Comment
        {
            Id = commentId,
            TaskId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Test comment",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new DeleteCommentCommand(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingComment);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);

        _commentRepositoryMock.Verify(
            x => x.Remove(existingComment),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentComment_ShouldThrowNotFoundException()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var command = new DeleteCommentCommand(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Comment with ID {commentId} was not found");

        _commentRepositoryMock.Verify(x => x.Remove(It.IsAny<Comment>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var commentId = Guid.Parse(guidString);
        var existingComment = new Comment
        {
            Id = commentId,
            TaskId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Test comment"
        };

        var command = new DeleteCommentCommand(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingComment);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
