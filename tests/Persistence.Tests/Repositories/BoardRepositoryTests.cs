using Domain.Entities;
using FluentAssertions;
using Persistence.Repositories;
using Persistence.Tests.Builders;
using Persistence.Tests.Infrastructure;

namespace Persistence.Tests.Repositories;

public class BoardRepositoryTests : BaseRepositoryTest
{
    private readonly BoardRepository _boardRepository;

    public BoardRepositoryTests()
    {
        _boardRepository = new BoardRepository(Context);
    }

    [Fact]
    public async Task GetBoardsByOwnerIdAsync_WithValidOwnerId_ShouldReturnOwnedBoards()
    {
        // Arrange
        await SeedDatabaseAsync();
        var ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Add additional boards for testing
        var additionalBoards = new[]
        {
            BoardBuilder.Create()
                .WithName("Additional Board 1")
                .WithOwnerId(ownerId)
                .Build(),
            BoardBuilder.Create()
                .WithName("Additional Board 2")
                .WithOwnerId(ownerId)
                .Build()
        };

        await Context.Boards.AddRangeAsync(additionalBoards);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardsByOwnerIdAsync(ownerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // 1 from seed + 2 additional
        result.Should().OnlyContain(b => b.OwnerId == ownerId);
        result.Select(b => b.Name).Should().Contain(new[] { "Test Board 1", "Additional Board 1", "Additional Board 2" });
    }

    [Fact]
    public async Task GetBoardsByOwnerIdAsync_WithNonExistentOwnerId_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentOwnerId = Guid.NewGuid();

        // Act
        var result = await _boardRepository.GetBoardsByOwnerIdAsync(nonExistentOwnerId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBoardsByUserIdAsync_WithOwnerUserId_ShouldReturnOwnedBoards()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var result = await _boardRepository.GetBoardsByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().OnlyContain(b => b.OwnerId == userId);
        result.First().Name.Should().Be("Test Board 1");
    }

    [Fact]
    public async Task GetBoardsByUserIdAsync_WithMemberUserId_ShouldReturnAccessibleBoards()
    {
        // Arrange
        await SeedDatabaseAsync();
        var ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var memberId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        
        // Create a board where memberId is a member but not owner
        var memberBoard = BoardBuilder.Create()
            .WithName("Member Board")
            .WithOwnerId(ownerId)
            .Build();

        await Context.Boards.AddAsync(memberBoard);
        await Context.SaveChangesAsync();

        // Add board membership
        var boardMember = new BoardMember
        {
            BoardId = memberBoard.Id,
            UserId = memberId,
            Role = "Member",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await Context.BoardMembers.AddAsync(boardMember);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardsByUserIdAsync(memberId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // 1 owned + 1 as member
        result.Select(b => b.Name).Should().Contain(new[] { "Test Board 2", "Member Board" });
    }

    [Fact]
    public async Task GetBoardWithStatesAsync_WithValidBoardId_ShouldReturnBoardWithOrderedStates()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Add states to the board (they should already be seeded, but let's verify the relationship)
        var boardStates = new[]
        {
            new State { Id = 4, Name = "Review", Order = 4 },
            new State { Id = 5, Name = "Testing", Order = 5 }
        };

        await Context.States.AddRangeAsync(boardStates);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardWithStatesAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(boardId);
        result.Name.Should().Be("Test Board 1");
        
        // Verify states are loaded and ordered
        result.States.Should().NotBeNull();
        result.States.Should().BeInAscendingOrder(s => s.Order);
    }

    [Fact]
    public async Task GetBoardWithStatesAsync_WithNonExistentBoardId_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentBoardId = Guid.NewGuid();

        // Act
        var result = await _boardRepository.GetBoardWithStatesAsync(nonExistentBoardId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBoardWithTasksAsync_WithValidBoardId_ShouldReturnBoardWithTasksAndRelations()
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
                .Build()
        };

        await Context.Tasks.AddRangeAsync(tasks);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardWithTasksAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(boardId);
        result.Name.Should().Be("Test Board 1");
        
        // Verify tasks are loaded
        result.Tasks.Should().NotBeNull();
        result.Tasks.Should().HaveCount(2);
        result.Tasks.Select(t => t.Title).Should().Contain(new[] { "Task 1", "Task 2" });
        
        // Verify task relations are loaded
        result.Tasks.Should().OnlyContain(t => t.State != null);
        result.Tasks.Should().OnlyContain(t => t.Assignee != null);
    }

    [Fact]
    public async Task GetBoardWithMembersAsync_WithValidBoardId_ShouldReturnBoardWithMembersAndOwner()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var memberId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Add board member
        var boardMember = new BoardMember
        {
            BoardId = boardId,
            UserId = memberId,
            Role = "Member",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await Context.BoardMembers.AddAsync(boardMember);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardWithMembersAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(boardId);
        result.Name.Should().Be("Test Board 1");
        
        // Verify owner is loaded
        result.Owner.Should().NotBeNull();
        result.Owner.Email.Should().Be("user1@test.com");
        
        // Verify members are loaded
        result.Members.Should().NotBeNull();
        result.Members.Should().HaveCount(1);
        result.Members.First().UserId.Should().Be(memberId);
        result.Members.First().Role.Should().Be("Member");
        
        // Verify member user is loaded
        result.Members.First().User.Should().NotBeNull();
        result.Members.First().User.Email.Should().Be("user2@test.com");
    }

    [Fact]
    public async Task GetBoardWithMembersAsync_WithBoardHavingNoMembers_ShouldReturnBoardWithOwnerOnly()
    {
        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        var result = await _boardRepository.GetBoardWithMembersAsync(boardId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(boardId);
        
        // Verify owner is loaded
        result.Owner.Should().NotBeNull();
        result.Owner.Email.Should().Be("user1@test.com");
        
        // Verify no members
        result.Members.Should().NotBeNull();
        result.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task Repository_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _boardRepository.GetBoardsByOwnerIdAsync(userId, cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _boardRepository.GetBoardsByUserIdAsync(userId, cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _boardRepository.GetBoardWithStatesAsync(boardId, cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _boardRepository.GetBoardWithTasksAsync(boardId, cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _boardRepository.GetBoardWithMembersAsync(boardId, cts.Token));
    }

    [Fact]
    public async Task Repository_ShouldInheritBaseRepositoryFunctionality()
    {
        // Arrange
        var ownerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var board = BoardBuilder.Create()
            .WithName("Inheritance Test Board")
            .WithDescription("Testing inheritance")
            .WithOwnerId(ownerId)
            .Build();

        // Act - Test inherited AddAsync functionality
        await _boardRepository.AddAsync(board);
        await Context.SaveChangesAsync();

        // Assert
        var addedBoard = await _boardRepository.GetByIdAsync(board.Id);
        addedBoard.Should().NotBeNull();
        addedBoard!.Name.Should().Be("Inheritance Test Board");

        // Act - Test specific BoardRepository functionality
        var boardsByOwner = await _boardRepository.GetBoardsByOwnerIdAsync(ownerId);

        // Assert
        boardsByOwner.Should().Contain(b => b.Id == board.Id);
    }

    [Fact]
    public async Task GetBoardsByUserIdAsync_WithUserAsOwnerAndMember_ShouldNotReturnDuplicates()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Add the user as a member of their own board (edge case)
        var boardMember = new BoardMember
        {
            BoardId = boardId,
            UserId = userId,
            Role = "Owner",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await Context.BoardMembers.AddAsync(boardMember);
        await Context.SaveChangesAsync();

        // Act
        var result = await _boardRepository.GetBoardsByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1); // Should not have duplicates
        result.First().Id.Should().Be(boardId);
    }
}
