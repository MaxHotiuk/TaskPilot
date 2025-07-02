using Application.Queries.Tasks;
using Application.Common.Dtos.Tasks;

namespace Application.Tests.Queries.Tasks;

public class GetTaskItemsByAssigneeIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly GetTaskItemsByAssigneeIdQueryHandler _handler;

    public GetTaskItemsByAssigneeIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _handler = new GetTaskItemsByAssigneeIdQueryHandler(_taskItemRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidAssigneeId_ShouldReturnAssigneeTasks()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var board1Id = Guid.NewGuid();
        var board2Id = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board1Id,
                Title = "Assigned Task 1",
                Description = "Description 1",
                StateId = 1,
                AssigneeId = assigneeId,
                DueDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board2Id,
                Title = "Assigned Task 2",
                Description = "Description 2",
                StateId = 2,
                AssigneeId = assigneeId,
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetTaskItemsByAssigneeIdQuery(assigneeId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(task => task.AssigneeId.Should().Be(assigneeId));

        resultList[0].Title.Should().Be("Assigned Task 1");
        resultList[0].BoardId.Should().Be(board1Id);

        resultList[1].Title.Should().Be("Assigned Task 2");
        resultList[1].BoardId.Should().Be(board2Id);

        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithAssigneeHavingNoTasks_ShouldReturnEmptyCollection()
    {
        // Arrange
        var assigneeId = Guid.NewGuid();
        var tasks = new List<TaskItem>();
        var query = new GetTaskItemsByAssigneeIdQuery(assigneeId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificAssigneeIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var assigneeId = Guid.Parse(guidString);
        var query = new GetTaskItemsByAssigneeIdQuery(assigneeId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByAssigneeIdAsync(assigneeId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
