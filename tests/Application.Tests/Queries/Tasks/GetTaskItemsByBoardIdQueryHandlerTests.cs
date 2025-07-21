using Application.Queries.Tasks;
using Domain.Dtos.Tasks;

namespace Application.Tests.Queries.Tasks;

public class GetTaskItemsByBoardIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetTaskItemsByBoardIdQueryHandler _handler;

    public GetTaskItemsByBoardIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetTaskItemsByBoardIdQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidBoardId_ShouldReturnBoardTasks()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var assignee1Id = Guid.NewGuid();
        var assignee2Id = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new TaskItem
            {
                Id = Guid.NewGuid(),
                BoardId = boardId,
                Title = "Board Task 1",
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
                BoardId = boardId,
                Title = "Board Task 2",
                Description = "Description 2",
                StateId = 2,
                AssigneeId = assignee2Id,
                DueDate = DateTime.UtcNow.AddDays(10),
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetTaskItemsByBoardIdQuery(boardId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        var resultList = result.ToList();
        resultList.Should().AllSatisfy(task => task.BoardId.Should().Be(boardId));

        resultList[0].Title.Should().Be("Board Task 1");
        resultList[0].AssigneeId.Should().Be(assignee1Id);

        resultList[1].Title.Should().Be("Board Task 2");
        resultList[1].AssigneeId.Should().Be(assignee2Id);

        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithBoardHavingNoTasks_ShouldReturnEmptyCollection()
    {
        // Arrange
        var boardId = Guid.NewGuid();
        var tasks = new List<TaskItem>();
        var query = new GetTaskItemsByBoardIdQuery(boardId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificBoardIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var boardId = Guid.Parse(guidString);
        var query = new GetTaskItemsByBoardIdQuery(boardId);

        _taskItemRepositoryMock
            .Setup(x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TaskItem>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.GetTasksByBoardIdAsync(boardId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
