using Bogus;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Persistence.Tests.Builders;

namespace Persistence.Tests.Utilities;

public static class TestDataGenerator
{
    private static readonly Faker _faker = new();

    public static class Users
    {
        public static UserBuilder Random() => UserBuilder.Create()
            .WithEmail(_faker.Internet.Email())
            .WithUsername(_faker.Internet.UserName())
            .WithEntraId(_faker.Random.Guid().ToString())
            .WithRole(_faker.PickRandom("User", "Admin", "Moderator"));

        public static IEnumerable<User> Generate(int count) =>
            Enumerable.Range(0, count).Select(_ => Random().Build());

        public static UserBuilder Admin() => Random().WithRole("Admin");
        public static UserBuilder StandardUser() => Random().WithRole("User");
    }

    public static class Boards
    {
        public static BoardBuilder Random() => BoardBuilder.Create()
            .WithName(_faker.Company.CompanyName())
            .WithDescription(_faker.Lorem.Paragraph());

        public static BoardBuilder ForOwner(Guid ownerId) => Random().WithOwnerId(ownerId);

        public static IEnumerable<Board> Generate(int count, Guid ownerId) =>
            Enumerable.Range(0, count).Select(_ => ForOwner(ownerId).Build());
    }

    public static class Tasks
    {
        public static TaskItemBuilder Random() => TaskItemBuilder.Create()
            .WithTitle(_faker.Lorem.Sentence(3))
            .WithDescription(_faker.Lorem.Paragraph())
            .WithDueDate(_faker.Date.Future());

        public static TaskItemBuilder ForBoard(Guid boardId) => Random().WithBoardId(boardId);

        public static TaskItemBuilder Overdue() => Random()
            .WithDueDate(_faker.Date.Past());

        public static TaskItemBuilder WithAssignee(Guid assigneeId) => Random()
            .WithAssigneeId(assigneeId);

        public static IEnumerable<TaskItem> Generate(int count, Guid boardId) =>
            Enumerable.Range(0, count).Select(_ => ForBoard(boardId).Build());
    }

    public static class States
    {
        public static StateBuilder Random() => StateBuilder.Create()
            .WithName(_faker.Lorem.Word())
            .WithOrder(_faker.Random.Int(1, 10));

        public static StateBuilder ForBoard(Guid boardId) => Random().WithBoardId(boardId);

        public static IEnumerable<State> CreateWorkflow(Guid boardId) => new[]
        {
            ForBoard(boardId).WithName("Backlog").WithOrder(1).Build(),
            ForBoard(boardId).WithName("To Do").WithOrder(2).Build(),
            ForBoard(boardId).WithName("In Progress").WithOrder(3).Build(),
            ForBoard(boardId).WithName("Review").WithOrder(4).Build(),
            ForBoard(boardId).WithName("Done").WithOrder(5).Build()
        };
    }

    public static class Comments
    {
        public static CommentBuilder Random() => CommentBuilder.Create()
            .WithContent(_faker.Lorem.Paragraph())
            .WithCreatedAt(_faker.Date.Recent());

        public static CommentBuilder ForTask(Guid taskId) => Random().WithTaskId(taskId);

        public static CommentBuilder WithAuthor(Guid authorId) => Random().WithAuthorId(authorId);

        public static IEnumerable<Comment> Generate(int count, Guid taskId, Guid authorId) =>
            Enumerable.Range(0, count).Select(_ => 
                ForTask(taskId).WithAuthorId(authorId).Build());
    }
}

public static class TestScenarios
{
    /// <summary>
    /// Creates a complete test scenario with a user, board, states, and tasks
    /// </summary>
    public static async Task<TestBoardScenario> CreateBoardWithTasksAsync(ApplicationDbContext context)
    {
        var user = TestDataGenerator.Users.Random().Build();
        
        // Use the first existing board from seeded data instead of creating a new one
        var existingBoard = await context.Boards.FirstAsync();
        
        // Get the states associated with this board
        var existingStates = await context.States.Where(s => s.BoardId == existingBoard.Id).ToListAsync();
        
        var tasks = TestDataGenerator.Tasks.Generate(5, existingBoard.Id).ToList();

        // Assign existing states to tasks (use existing state IDs)
        var random = new Random();
        foreach (var task in tasks)
        {
            if (existingStates.Any())
            {
                var randomState = existingStates[random.Next(existingStates.Count)];
                task.StateId = randomState.Id;
            }
            else
            {
                task.StateId = 1; // Fallback to a default state ID
            }
            task.AssigneeId = user.Id;
        }

        await context.Users.AddAsync(user);
        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();

        return new TestBoardScenario
        {
            User = user,
            Board = existingBoard,
            States = existingStates,
            Tasks = tasks
        };
    }

    /// <summary>
    /// Creates a test scenario with multiple users collaborating on a board
    /// </summary>
    public static async Task<TestCollaborationScenario> CreateCollaborationScenarioAsync(ApplicationDbContext context)
    {
        // Use the existing owner from seeded data instead of creating a new one
        var existingBoard = await context.Boards.Skip(1).Include(b => b.Owner).FirstAsync();
        var owner = existingBoard.Owner;
        
        var members = TestDataGenerator.Users.Generate(3).ToList();
        
        // Get the states associated with boards (since all states are on the first board, we'll use those)
        var existingStates = await context.States.Take(3).ToListAsync();

        var allUsers = new List<User> { owner };
        allUsers.AddRange(members);

        // Create tasks assigned to different users
        var tasks = new List<TaskItem>();
        foreach (var user in allUsers.Take(4)) // Don't assign to all users
        {
            var userTasks = TestDataGenerator.Tasks.Generate(2, existingBoard.Id).ToList();
            foreach (var task in userTasks)
            {
                task.AssigneeId = user.Id;
                if (existingStates.Any())
                {
                    task.StateId = existingStates[new Random().Next(existingStates.Count)].Id;
                }
                else
                {
                    task.StateId = 1; // Fallback to a default state ID
                }
            }
            tasks.AddRange(userTasks);
        }

        await context.Users.AddRangeAsync(members);
        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();

        return new TestCollaborationScenario
        {
            Owner = owner,
            Members = members,
            Board = existingBoard,
            States = existingStates,
            Tasks = tasks
        };
    }
}

public class TestBoardScenario
{
    public User User { get; set; } = null!;
    public Board Board { get; set; } = null!;
    public List<State> States { get; set; } = new();
    public List<TaskItem> Tasks { get; set; } = new();
}

public class TestCollaborationScenario
{
    public User Owner { get; set; } = null!;
    public List<User> Members { get; set; } = new();
    public Board Board { get; set; } = null!;
    public List<State> States { get; set; } = new();
    public List<TaskItem> Tasks { get; set; } = new();
}
