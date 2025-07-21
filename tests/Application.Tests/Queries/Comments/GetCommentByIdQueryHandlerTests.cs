using Application.Queries.Comments;
using Domain.Dtos.Comments;

namespace Application.Tests.Queries.Comments;

public class GetCommentByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetCommentByIdQueryHandler _handler;

    public GetCommentByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _commentRepositoryMock = _fixture.Freeze<Mock<ICommentRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Comments).Returns(_commentRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetCommentByIdQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnCommentDto()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var expectedComment = new Comment
        {
            Id = commentId,
            TaskId = taskId,
            AuthorId = authorId,
            Content = "Test comment content",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetCommentByIdQuery(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedComment);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(commentId);
        result.TaskId.Should().Be(taskId);
        result.AuthorId.Should().Be(authorId);
        result.Content.Should().Be("Test comment content");
        result.CreatedAt.Should().Be(expectedComment.CreatedAt);
        result.UpdatedAt.Should().Be(expectedComment.UpdatedAt);

        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var query = new GetCommentByIdQuery(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var commentId = Guid.Parse(guidString);
        var query = new GetCommentByIdQuery(commentId);

        _commentRepositoryMock
            .Setup(x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(
            x => x.GetByIdAsync(commentId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
