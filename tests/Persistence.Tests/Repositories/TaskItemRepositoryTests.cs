using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories;
using Persistence.Tests.Builders;
using Persistence.Tests.Infrastructure;

namespace Persistence.Tests.Repositories;

public class TaskItemRepositoryTests : BaseRepositoryTest
{
    private readonly TaskItemRepository _taskItemRepository;

    public TaskItemRepositoryTests()
    {
        _taskItemRepository = new TaskItemRepository(Context);
    }

    [Fact]
    public async Task GetTasksByBoardIdAsync_WithValidBoardId_ShouldReturnTasksForBoard()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var tasks = new[]
        {
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Task 1")
                .WithStateId(1)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Task 2")
                .WithStateId(2)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"))
                .WithTitle("Other Board Task")
                .WithStateId(1)
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetTasksByBoardIdAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.BoardId == boardId);
        result.Select(t => t.Title).Should().Contain(new[] { "Task 1", "Task 2" });
        
        // Verify includes
        result.Should().OnlyContain(t => t.State != null);
        result.Should().OnlyContain(t => t.Assignee != null);
    }

    [Fact]
    public async Task GetTasksByBoardIdAsync_WithNonExistentBoardId_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentBoardId = Guid.NewGuid();

        // Act
        var result = await _taskItemRepository.GetTasksByBoardIdAsync(nonExistentBoardId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasksByStateIdAsync_WithValidStateId_ShouldReturnTasksInState()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var tasks = new[]
        {
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Todo Task 1")
                .WithStateId(1)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Todo Task 2")
                .WithStateId(1)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("In Progress Task")
                .WithStateId(2)
                .WithAssigneeId(assigneeId)
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetTasksByStateIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.StateId == 1);
        result.Select(t => t.Title).Should().Contain(new[] { "Todo Task 1", "Todo Task 2" });
        
        // Verify includes
        result.Should().OnlyContain(t => t.Assignee != null);
    }

    [Fact]
    public async Task GetTasksByAssigneeIdAsync_WithValidAssigneeId_ShouldReturnAssignedTasks()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var otherAssigneeId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var tasks = new[]
        {
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Assigned Task 1")
                .WithStateId(1)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Assigned Task 2")
                .WithStateId(2)
                .WithAssigneeId(assigneeId)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Other User Task")
                .WithStateId(1)
                .WithAssigneeId(otherAssigneeId)
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetTasksByAssigneeIdAsync(assigneeId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.AssigneeId == assigneeId);
        result.Select(t => t.Title).Should().Contain(new[] { "Assigned Task 1", "Assigned Task 2" });
        
        // Verify includes
        result.Should().OnlyContain(t => t.State != null);
        result.Should().OnlyContain(t => t.Board != null);
    }

    [Fact]
    public async Task GetTaskWithCommentsAsync_WithValidTaskId_ShouldReturnTaskWithComments()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var authorId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var task = TaskItemBuilder.CreateTaskForBoard(boardId)
            .WithTitle("Task with Comments")
            .WithStateId(1)
            .WithAssigneeId(assigneeId)
            .Build();

        await Context.Tasks.AddAsync(task);
        await Context.SaveChangesAsync();

        var comments = new[]
        {
            CommentBuilder.Create()
                .WithTaskId(task.Id)
                .WithAuthorId(authorId)
                .WithContent("First comment")
                .WithCreatedAt(DateTime.UtcNow.AddHours(-2))
                .Build(),
            CommentBuilder.Create()
                .WithTaskId(task.Id)
                .WithAuthorId(assigneeId)
                .WithContent("Second comment")
                .WithCreatedAt(DateTime.UtcNow.AddHours(-1))
                .Build()
        };

        await Context.Comments.AddRangeAsync(comments);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetTaskWithCommentsAsync(task.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(task.Id);
        result.Title.Should().Be("Task with Comments");
        
        // Verify includes
        result.State.Should().NotBeNull();
        result.Assignee.Should().NotBeNull();
        result.Board.Should().NotBeNull();
        
        // Verify comments are loaded and ordered
        result.Comments.Should().HaveCount(2);
        result.Comments.Should().BeInAscendingOrder(c => c.CreatedAt);
        result.Comments.First().Content.Should().Be("First comment");
        result.Comments.Last().Content.Should().Be("Second comment");
        
        // Verify comment authors are loaded
        result.Comments.Should().OnlyContain(c => c.Author != null);
    }

    [Fact]
    public async Task GetTaskWithCommentsAsync_WithNonExistentTaskId_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentTaskId = Guid.NewGuid();

        // Act
        var result = await _taskItemRepository.GetTaskWithCommentsAsync(nonExistentTaskId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetOverdueTasksAsync_WithOverdueTasks_ShouldReturnOnlyOverdueTasks()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var tasks = new[]
        {
            TaskItemBuilder.CreateOverdueTask()
                .WithBoardId(boardId)
                .WithAssigneeId(assigneeId)
                .WithTitle("Overdue Task 1")
                .WithDueDate(DateTime.UtcNow.AddDays(-5))
                .Build(),
            TaskItemBuilder.CreateOverdueTask()
                .WithBoardId(boardId)
                .WithAssigneeId(assigneeId)
                .WithTitle("Overdue Task 2")
                .WithDueDate(DateTime.UtcNow.AddDays(-2))
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Future Task")
                .WithAssigneeId(assigneeId)
                .WithDueDate(DateTime.UtcNow.AddDays(5))
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("No Due Date Task")
                .WithAssigneeId(assigneeId)
                .WithDueDate(null)
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetOverdueTasksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow);
        result.Select(t => t.Title).Should().Contain(new[] { "Overdue Task 1", "Overdue Task 2" });
        
        // Verify includes
        result.Should().OnlyContain(t => t.Assignee != null);
        result.Should().OnlyContain(t => t.Board != null);
    }

    [Fact]
    public async Task GetOverdueTasksAsync_WithNoOverdueTasks_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var assigneeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var tasks = new[]
        {
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Future Task")
                .WithAssigneeId(assigneeId)
                .WithDueDate(DateTime.UtcNow.AddDays(5))
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("No Due Date Task")
                .WithAssigneeId(assigneeId)
                .WithDueDate(null)
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _taskItemRepository.GetOverdueTasksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Repository_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _taskItemRepository.GetTasksByBoardIdAsync(boardId, cts.Token));
    }

    [Fact]
    public async Task Repository_ShouldInheritBaseRepositoryFunctionality()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var task = TaskItemBuilder.CreateTaskForBoard(boardId)
            .WithTitle("Base Repository Test")
            .WithStateId(1)
            .Build();

        // Act - Test inherited AddAsync functionality
        await _taskItemRepository.AddAsync(task);
        await Context.SaveChangesAsync();

        // Assert
        var addedTask = await _taskItemRepository.GetByIdAsync(task.Id);
        addedTask.Should().NotBeNull();
        addedTask!.Title.Should().Be("Base Repository Test");

        // Act - Test inherited Update functionality
        addedTask.Title = "Updated Title";
        _taskItemRepository.Update(addedTask);
        await Context.SaveChangesAsync();

        // Assert
        var updatedTask = await _taskItemRepository.GetByIdAsync(task.Id);
        updatedTask!.Title.Should().Be("Updated Title");
    }
}
