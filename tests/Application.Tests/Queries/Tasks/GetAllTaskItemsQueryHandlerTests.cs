using Application.Queries.Tasks;
using Application.Common.Dtos.Tasks;

namespace Application.Tests.Queries.Tasks;

public class GetAllTaskItemsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetAllTaskItemsQueryHandler _handler;

    public GetAllTaskItemsQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetAllTaskItemsQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithExistingTasks_ShouldReturnAllTasks()
    {
        // Arrange
        var board1Id = Guid.NewGuid();
        var board2Id = Guid.NewGuid();
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board1Id,
                Title = "Task 1",
                Description = "Description 1",
                StateId = 1,
                AssigneeId = assignee1Id,
                DueDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board2Id,
                Title = "Task 2",
                Description = "Description 2",
                StateId = 2,
                AssigneeId = assignee2Id,
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList[0].Title.Should().Be("Task 1");
        resultList[0].BoardId.Should().Be(board1Id);
        resultList[0].AssigneeId.Should().Be(assignee1Id);

        resultList[1].Title.Should().Be("Task 2");
        resultList[1].BoardId.Should().Be(board2Id);
        resultList[1].AssigneeId.Should().Be(assignee2Id);

        _taskItemRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoTasks_ShouldReturnEmptyCollection()
    {
        // Arrange
        var tasks = new List<TaskItem>();
        var query = new GetAllTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithTasksHavingNullOptionalFields_ShouldHandleCorrectly()
    {
        // Arrange
        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = Guid.NewGuid(),
                Title = "Task with nulls",
                Description = null,
                StateId = 1,
                AssigneeId = null,
                DueDate = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var resultTask = result.First();
        resultTask.Description.Should().BeNull();
        resultTask.AssigneeId.Should().BeNull();
        resultTask.DueDate.Should().BeNull();
    }
}
