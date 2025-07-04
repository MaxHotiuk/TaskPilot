using Application.Queries.Comments;
using Application.Common.Dtos.Comments;

namespace Application.Tests.Queries.Comments;

public class GetCommentsByTaskIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetCommentsByTaskIdQueryHandler _handler;

    public GetCommentsByTaskIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _commentRepositoryMock = _fixture.Freeze<Mock<ICommentRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Comments).Returns(_commentRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetCommentsByTaskIdQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidTaskId_ShouldReturnComments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var author1Id = Guid.NewGuid();
        var author2Id = Guid.NewGuid();
        var comments = new List<Comment>
        {
            new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                AuthorId = author1Id,
                Content = "First comment",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Comment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                AuthorId = author2Id,
                Content = "Second comment",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var query = new GetCommentsByTaskIdQuery(taskId);

        _commentRepositoryMock
            .Setup(x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(comment => comment.TaskId.Should().Be(taskId));

        resultList[0].Content.Should().Be("First comment");
        resultList[0].AuthorId.Should().Be(author1Id);

        resultList[1].Content.Should().Be("Second comment");
        resultList[1].AuthorId.Should().Be(author2Id);

        _commentRepositoryMock.Verify(
            x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithTaskHavingNoComments_ShouldReturnEmptyCollection()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var comments = new List<Comment>();
        var query = new GetCommentsByTaskIdQuery(taskId);

        _commentRepositoryMock
            .Setup(x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _commentRepositoryMock.Verify(
            x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificTaskIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var taskId = Guid.Parse(guidString);
        var query = new GetCommentsByTaskIdQuery(taskId);

        _commentRepositoryMock
            .Setup(x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Comment>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(
            x => x.GetCommentsByTaskIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
