using Application.Queries.Tasks;
using Application.Common.Dtos.Tasks;

namespace Application.Tests.Queries.Tasks;

public class GetTaskItemByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly GetTaskItemByIdQueryHandler _handler;

    public GetTaskItemByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _handler = new GetTaskItemByIdQueryHandler(_taskItemRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnTaskItemDto()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var boardId = Guid.NewGuid();
        var assigneeId = Guid.NewGuid();
        var dueDate = DateTime.UtcNow.AddDays(7);

        var expectedTask = new TaskItem
        {
            Id = taskId,
            BoardId = boardId,
            Title = "Test Task",
            Description = "Test Description",
            StateId = 1,
            AssigneeId = assigneeId,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskItemByIdQuery(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.BoardId.Should().Be(boardId);
        result.Title.Should().Be("Test Task");
        result.Description.Should().Be("Test Description");
        result.StateId.Should().Be(1);
        result.AssigneeId.Should().Be(assigneeId);
        result.DueDate.Should().Be(dueDate);
        result.CreatedAt.Should().Be(expectedTask.CreatedAt);
        result.UpdatedAt.Should().Be(expectedTask.UpdatedAt);

        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var query = new GetTaskItemByIdQuery(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var taskId = Guid.Parse(guidString);
        var query = new GetTaskItemByIdQuery(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithTaskHavingNullOptionalFields_ShouldReturnDtoWithNullValues()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedTask = new TaskItem
        {
            Id = taskId,
            BoardId = Guid.NewGuid(),
            Title = "Test Task",
            Description = null,
            StateId = 1,
            AssigneeId = null,
            DueDate = null,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetTaskItemByIdQuery(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().BeNull();
        result.AssigneeId.Should().BeNull();
        result.DueDate.Should().BeNull();
    }
}
