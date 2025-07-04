using Application.Queries.Tasks;
using Application.Common.Dtos.Tasks;

namespace Application.Tests.Queries.Tasks;

public class GetOverdueTaskItemsQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetOverdueTaskItemsQueryHandler _handler;

    public GetOverdueTaskItemsQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetOverdueTaskItemsQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithOverdueTasks_ShouldReturnOverdueTasks()
    {
        // Arrange
        var board1Id = Guid.NewGuid();
        var board2Id = Guid.NewGuid();
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();

        var overdueTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board1Id,
                Title = "Overdue Task 1",
                Description = "Description 1",
                StateId = 1,
                AssigneeId = assignee1Id,
                DueDate = DateTime.UtcNow.AddDays(-2), // Overdue
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = board2Id,
                Title = "Overdue Task 2",
                Description = "Description 2",
                StateId = 2,
                AssigneeId = assignee2Id,
                DueDate = DateTime.UtcNow.AddDays(-1), // Overdue
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetOverdueTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetOverdueTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(overdueTasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(task => task.DueDate.Should().BeBefore(DateTime.UtcNow));

        resultList[0].Title.Should().Be("Overdue Task 1");
        resultList[0].BoardId.Should().Be(board1Id);
        resultList[0].AssigneeId.Should().Be(assignee1Id);

        resultList[1].Title.Should().Be("Overdue Task 2");
        resultList[1].BoardId.Should().Be(board2Id);
        resultList[1].AssigneeId.Should().Be(assignee2Id);

        _taskItemRepositoryMock.Verify(
            x => x.GetOverdueTasksAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNoOverdueTasks_ShouldReturnEmptyCollection()
    {
        // Arrange
        var overdueTasks = new List<TaskItem>();
        var query = new GetOverdueTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetOverdueTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(overdueTasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.GetOverdueTasksAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldOnlyReturnTasksWithPastDueDates()
    {
        // Arrange
        var overdueTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = Guid.NewGuid(),
                Title = "Very Overdue Task",
                StateId = 1,
                DueDate = DateTime.UtcNow.AddDays(-10), // Very overdue
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var query = new GetOverdueTaskItemsQuery();

        _taskItemRepositoryMock
            .Setup(x => x.GetOverdueTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(overdueTasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        var overdueTask = result.First();
        overdueTask.DueDate.Should().BeBefore(DateTime.UtcNow);
        overdueTask.Title.Should().Be("Very Overdue Task");
    }
}
