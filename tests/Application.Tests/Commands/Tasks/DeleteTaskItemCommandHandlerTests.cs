using Application.Commands.Tasks;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Tasks;

public class DeleteTaskItemCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly DeleteTaskItemCommandHandler _handler;

    public DeleteTaskItemCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _taskItemRepositoryMock = _fixture.Freeze<Mock<ITaskItemRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Tasks).Returns(_taskItemRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new DeleteTaskItemCommandHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldDeleteTaskItem()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test Description",
            StateId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new DeleteTaskItemCommand(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);

        _taskItemRepositoryMock.Verify(
            x => x.Remove(existingTask),
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentTask_ShouldThrowNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command = new DeleteTaskItemCommand(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Task with ID {taskId} was not found");

        _taskItemRepositoryMock.Verify(x => x.Remove(It.IsAny<TaskItem>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    public async Task Handle_WithSpecificGuids_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var taskId = Guid.Parse(guidString);
        var existingTask = new TaskItem
        {
            Id = taskId,
            BoardId = Guid.NewGuid(),
            Title = "Test Task",
            StateId = 1
        };

        var command = new DeleteTaskItemCommand(taskId);

        _taskItemRepositoryMock
            .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _taskItemRepositoryMock.Verify(
            x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
