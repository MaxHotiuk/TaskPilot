using Application.Commands.Comments;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Comments;

public class UpdateCommentCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _commentRepositoryMock = _fixture.Freeze<Mock<ICommentRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _handler = new UpdateCommentCommandHandler(_commentRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var existingComment = new Comment
        {
            Id = commentId,
            TaskId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Old content",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateCommentCommand(
            Id: commentId,
            Content: "Updated content"
        );

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingComment);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingComment.Content.Should().Be(command.Content);
        existingComment.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentComment_ShouldThrowNotFoundException()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var command = new UpdateCommentCommand(
            Id: commentId,
            Content: "Updated content"
        );

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Comment with ID {commentId} was not found");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldNotChangeCreatedAtTimestamp()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var originalCreatedAt = DateTime.UtcNow.AddDays(-5);
        var existingComment = new Comment
        {
            Id = commentId,
            TaskId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Old content",
            CreatedAt = originalCreatedAt,
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateCommentCommand(
            Id: commentId,
            Content: "Updated content"
        );

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingComment);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingComment.CreatedAt.Should().Be(originalCreatedAt);
        existingComment.UpdatedAt.Should().NotBe(originalCreatedAt);
    }
}
