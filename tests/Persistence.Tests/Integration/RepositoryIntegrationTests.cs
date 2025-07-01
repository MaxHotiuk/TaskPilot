using Persistence.Repositories;
using Persistence.Tests.Utilities;

namespace Persistence.Tests.Integration;

public class RepositoryIntegrationTests : BaseRepositoryTest
{
    private readonly TaskItemRepository _taskRepository;
    private readonly UserRepository _userRepository;
    private readonly BoardRepository _boardRepository;

    public RepositoryIntegrationTests()
    {
        _taskRepository = new TaskItemRepository(Context);
        _userRepository = new UserRepository(Context);
        _boardRepository = new BoardRepository(Context);
    }

    [Fact]
    public async Task CompleteWorkflow_CreateBoardWithTasksAndManageThem_ShouldWorkEndToEnd()
    {
        // Arrange - Seed database and create a complete test scenario
        await SeedDatabaseAsync();
        var scenario = await TestScenarios.CreateBoardWithTasksAsync(Context);

        // Act - Perform various operations
        var boardTasks = await _taskRepository.GetTasksByBoardIdAsync(scenario.Board.Id);
        var userTasks = await _taskRepository.GetTasksByAssigneeIdAsync(scenario.User.Id);
        var boardWithTasks = await _boardRepository.GetBoardWithTasksAsync(scenario.Board.Id);

        // Assert
        boardTasks.Should().HaveCount(5);
        userTasks.Should().HaveCount(5);
        boardWithTasks.Should().NotBeNull();
        boardWithTasks!.Tasks.Should().HaveCount(5);

        // Verify all tasks have valid states and assignees
        boardTasks.Should().OnlyContain(t => t.State != null);
        boardTasks.Should().OnlyContain(t => t.Assignee != null);
        boardTasks.Should().OnlyContain(t => t.AssigneeId == scenario.User.Id);
    }

    [Fact]
    public async Task CollaborationScenario_MultipleUsersOnBoard_ShouldHandleCorrectly()
    {
        // Arrange
        await SeedDatabaseAsync();
        var scenario = await TestScenarios.CreateCollaborationScenarioAsync(Context);

        // Act
        var ownerBoards = await _boardRepository.GetBoardsByOwnerIdAsync(scenario.Owner.Id);
        var boardWithMembers = await _boardRepository.GetBoardWithMembersAsync(scenario.Board.Id);
        var allTasks = await _taskRepository.GetTasksByBoardIdAsync(scenario.Board.Id);

        // Assert
        ownerBoards.Should().HaveCount(1);
        ownerBoards.First().Id.Should().Be(scenario.Board.Id);

        boardWithMembers.Should().NotBeNull();
        boardWithMembers!.Owner.Should().NotBeNull();
        boardWithMembers.Owner.Id.Should().Be(scenario.Owner.Id);

        allTasks.Should().HaveCount(8); // 2 tasks per user (4 users)
        
        // Verify tasks are assigned to users from the scenario
        var allUserIds = new List<Guid> { scenario.Owner.Id };
        allUserIds.AddRange(scenario.Members.Select(m => m.Id));
        
        allTasks.Should().OnlyContain(t => t.AssigneeId.HasValue && allUserIds.Contains(t.AssigneeId.Value));
    }

    [Fact]
    public async Task TaskManagement_WithCommentsAndStateChanges_ShouldMaintainDataIntegrity()
    {
        // Arrange
        await SeedDatabaseAsync();
        var scenario = await TestScenarios.CreateBoardWithTasksAsync(Context);
        var taskToUpdate = scenario.Tasks.First();
        var newState = scenario.States.Last(); // Move to last state (Done)

        // Add comments to the task with ordered dates
        var baseTime = DateTime.UtcNow.AddHours(-3);
        var comments = new[]
        {
            TestDataGenerator.Comments.ForTask(taskToUpdate.Id).WithAuthorId(scenario.User.Id).WithCreatedAt(baseTime).Build(),
            TestDataGenerator.Comments.ForTask(taskToUpdate.Id).WithAuthorId(scenario.User.Id).WithCreatedAt(baseTime.AddHours(1)).Build(),
            TestDataGenerator.Comments.ForTask(taskToUpdate.Id).WithAuthorId(scenario.User.Id).WithCreatedAt(baseTime.AddHours(2)).Build()
        };
        await Context.Comments.AddRangeAsync(comments);
        await Context.SaveChangesAsync();

        // Act - Update task state and get it with comments
        taskToUpdate.StateId = newState.Id;
        _taskRepository.Update(taskToUpdate);
        await Context.SaveChangesAsync();

        var taskWithComments = await _taskRepository.GetTaskWithCommentsAsync(taskToUpdate.Id);

        // Assert
        taskWithComments.Should().NotBeNull();
        taskWithComments!.StateId.Should().Be(newState.Id);
        taskWithComments.State.Name.Should().Be(newState.Name);
        taskWithComments.Comments.Should().HaveCount(3);
        taskWithComments.Comments.Should().OnlyContain(c => c.Author != null);
        
        // Check that comments are ordered by CreatedAt without triggering cyclic reference issues
        var commentDates = taskWithComments.Comments.Select(c => c.CreatedAt).ToList();
        commentDates.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task UserRepositoryMethods_WithGeneratedData_ShouldWorkCorrectly()
    {
        // Arrange
        var users = TestDataGenerator.Users.Generate(10).ToList();
        var adminUser = TestDataGenerator.Users.Admin().Build();
        users.Add(adminUser);

        await Context.Users.AddRangeAsync(users);
        await Context.SaveChangesAsync();

        // Act
        var userByEmail = await _userRepository.GetByEmailAsync(adminUser.Email);
        var userByEntraId = await _userRepository.GetByEntraIdAsync(adminUser.EntraId);
        var emailExists = await _userRepository.ExistsByEmailAsync(adminUser.Email);
        var entraIdExists = await _userRepository.ExistsByEntraIdAsync(adminUser.EntraId);

        // Assert
        userByEmail.Should().NotBeNull();
        userByEmail!.Id.Should().Be(adminUser.Id);

        userByEntraId.Should().NotBeNull();
        userByEntraId!.Id.Should().Be(adminUser.Id);

        emailExists.Should().BeTrue();
        entraIdExists.Should().BeTrue();
    }

    [Fact]
    public async Task OverdueTasks_WithMixedDueDates_ShouldReturnOnlyOverdue()
    {
        // Arrange
        await SeedDatabaseAsync();
        var scenario = await TestScenarios.CreateBoardWithTasksAsync(Context);
        
        // Create specific overdue tasks
        var overdueTasks = new[]
        {
            TestDataGenerator.Tasks.Overdue()
                .WithBoardId(scenario.Board.Id)
                .WithAssigneeId(scenario.User.Id)
                .WithStateId(scenario.States.First().Id)
                .WithDueDate(DateTime.UtcNow.AddDays(-5))
                .Build(),
            TestDataGenerator.Tasks.Overdue()
                .WithBoardId(scenario.Board.Id)
                .WithAssigneeId(scenario.User.Id)
                .WithStateId(scenario.States.First().Id)
                .WithDueDate(DateTime.UtcNow.AddDays(-1))
                .Build()
        };

        await Context.Tasks.AddRangeAsync(overdueTasks);
        await Context.SaveChangesAsync();

        // Act
        var overdueResults = await _taskRepository.GetOverdueTasksAsync();

        // Assert
        overdueResults.Should().HaveCount(2);
        overdueResults.Should().OnlyContain(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow);
        overdueResults.Should().OnlyContain(t => t.Assignee != null);
        overdueResults.Should().OnlyContain(t => t.Board != null);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task PerformanceTest_WithMultipleBoards_ShouldHandleLoad(int numberOfBoards)
    {
        // Arrange
        var owner = TestDataGenerator.Users.Random().Build();
        await Context.Users.AddAsync(owner);
        await Context.SaveChangesAsync();

        var boards = TestDataGenerator.Boards.Generate(numberOfBoards, owner.Id).ToList();
        await Context.Boards.AddRangeAsync(boards);
        await Context.SaveChangesAsync();

        // Act
        var startTime = DateTime.UtcNow;
        var ownerBoards = await _boardRepository.GetBoardsByOwnerIdAsync(owner.Id);
        var endTime = DateTime.UtcNow;

        // Assert
        ownerBoards.Should().HaveCount(numberOfBoards);
        var executionTime = endTime - startTime;
        executionTime.Should().BeLessThan(TimeSpan.FromSeconds(5)); // Performance assertion
    }
}
