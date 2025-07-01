using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;

namespace Persistence.Tests.Integration;

public class UnitOfWorkTests : BaseRepositoryTest
{
    private readonly UnitOfWork _unitOfWork;
    private readonly UserRepository _userRepository;
    private readonly TaskItemRepository _taskItemRepository;

    public UnitOfWorkTests()
    {
        _unitOfWork = new UnitOfWork(Context);
        _userRepository = new UserRepository(Context);
        _taskItemRepository = new TaskItemRepository(Context);
    }

    [Fact]
    public async Task SaveChangesAsync_WithValidChanges_ShouldPersistChanges()
    {
        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("unittest@test.com")
            .WithUsername("unittest")
            .WithEntraId("entra-unittest")
            .WithRole("User")
            .Build();

        await _userRepository.AddAsync(user);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        
        var savedUser = await _userRepository.GetByIdAsync(user.Id);
        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("unittest@test.com");
    }

    [Fact]
    public async Task Transaction_WithCommit_ShouldPersistAllChanges()
    {
        // Skip transaction tests with in-memory provider
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        
        var user = UserBuilder.Create()
            .WithEmail("transaction@test.com")
            .WithUsername("transactionuser")
            .WithEntraId("entra-transaction")
            .WithRole("User")
            .Build();

        var task = TaskItemBuilder.CreateTaskForBoard(boardId)
            .WithTitle("Transaction Test Task")
            .WithStateId(1)
            .WithAssigneeId(user.Id)
            .Build();

        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        await _userRepository.AddAsync(user);
        await _taskItemRepository.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedUser = await _userRepository.GetByIdAsync(user.Id);
        var savedTask = await _taskItemRepository.GetByIdAsync(task.Id);

        savedUser.Should().NotBeNull();
        savedTask.Should().NotBeNull();
        savedTask!.AssigneeId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Transaction_WithRollback_ShouldNotPersistChanges()
    {
        // Skip transaction tests with in-memory provider
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        
        var user = UserBuilder.Create()
            .WithEmail("rollback@test.com")
            .WithUsername("rollbackuser")
            .WithEntraId("entra-rollback")
            .WithRole("User")
            .Build();

        var task = TaskItemBuilder.CreateTaskForBoard(boardId)
            .WithTitle("Rollback Test Task")
            .WithStateId(1)
            .WithAssigneeId(user.Id)
            .Build();

        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        await _userRepository.AddAsync(user);
        await _taskItemRepository.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        var savedUser = await _userRepository.GetByIdAsync(user.Id);
        var savedTask = await _taskItemRepository.GetByIdAsync(task.Id);

        savedUser.Should().BeNull();
        savedTask.Should().BeNull();
    }

    [Fact]
    public async Task Transaction_WithMultipleOperations_ShouldMaintainConsistency()
    {
        // Skip transaction tests with in-memory provider
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange
        await SeedDatabaseAsync();
        var boardId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var existingUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var newUser = UserBuilder.Create()
            .WithEmail("multiop@test.com")
            .WithUsername("multiopuser")
            .WithEntraId("entra-multiop")
            .WithRole("User")
            .Build();

        var tasks = new[]
        {
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Multi-op Task 1")
                .WithStateId(1)
                .WithAssigneeId(newUser.Id)
                .Build(),
            TaskItemBuilder.CreateTaskForBoard(boardId)
                .WithTitle("Multi-op Task 2")
                .WithStateId(2)
                .WithAssigneeId(existingUserId)
                .Build()
        };

        // Act
        await _unitOfWork.BeginTransactionAsync();
        
        // Add new user
        await _userRepository.AddAsync(newUser);
        
        // Add multiple tasks
        await _taskItemRepository.AddRangeAsync(tasks);
        
        // Update existing user
        var existingUser = await _userRepository.GetByIdAsync(existingUserId);
        existingUser!.Role = "Admin";
        _userRepository.Update(existingUser);
        
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedNewUser = await _userRepository.GetByIdAsync(newUser.Id);
        var savedTasks = await _taskItemRepository.FindAsync(t => 
            t.Title == "Multi-op Task 1" || t.Title == "Multi-op Task 2");
        var updatedExistingUser = await _userRepository.GetByIdAsync(existingUserId);

        savedNewUser.Should().NotBeNull();
        savedTasks.Should().HaveCount(2);
        updatedExistingUser!.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Transaction_WithException_ShouldNotAffectSubsequentOperations()
    {
        // Skip transaction tests with in-memory provider
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange
        var user1 = UserBuilder.Create()
            .WithEmail("before@test.com")
            .WithUsername("beforeuser")
            .WithEntraId("entra-before")
            .WithRole("User")
            .Build();

        var user2 = UserBuilder.Create()
            .WithEmail("after@test.com")
            .WithUsername("afteruser")
            .WithEntraId("entra-after")
            .WithRole("User")
            .Build();

        // Act & Assert - First successful transaction
        await _unitOfWork.BeginTransactionAsync();
        await _userRepository.AddAsync(user1);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Verify first user was saved
        var savedUser1 = await _userRepository.GetByIdAsync(user1.Id);
        savedUser1.Should().NotBeNull();

        // Failed transaction (rollback)
        await _unitOfWork.BeginTransactionAsync();
        await _userRepository.AddAsync(user2);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.RollbackTransactionAsync();

        // Verify second user was not saved
        var savedUser2 = await _userRepository.GetByIdAsync(user2.Id);
        savedUser2.Should().BeNull();

        // Another successful transaction after rollback
        var user3 = UserBuilder.Create()
            .WithEmail("final@test.com")
            .WithUsername("finaluser")
            .WithEntraId("entra-final")
            .WithRole("User")
            .Build();

        await _unitOfWork.BeginTransactionAsync();
        await _userRepository.AddAsync(user3);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Verify third user was saved
        var savedUser3 = await _userRepository.GetByIdAsync(user3.Id);
        savedUser3.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutTransaction_ShouldWorkNormally()
    {
        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("notransaction@test.com")
            .WithUsername("notransactionuser")
            .WithEntraId("entra-notransaction")
            .WithRole("User")
            .Build();

        // Act
        await _userRepository.AddAsync(user);
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        
        var savedUser = await _userRepository.GetByIdAsync(user.Id);
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task UnitOfWork_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("cancellation@test.com")
            .WithUsername("cancellationuser")
            .WithEntraId("entra-cancellation")
            .WithRole("User")
            .Build();

        await _userRepository.AddAsync(user);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _unitOfWork.SaveChangesAsync(cts.Token));

        // Skip transaction test with in-memory provider
        if (!Context.Database.IsInMemory())
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => _unitOfWork.BeginTransactionAsync(cts.Token));
        }
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutBeginTransaction_ShouldNotThrow()
    {
        // Arrange - No transaction started

        // Act & Assert - Should not throw an exception
        await _unitOfWork.CommitTransactionAsync();
        
        // No exception means the test passes
        true.Should().BeTrue();
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutBeginTransaction_ShouldNotThrow()
    {
        // Arrange - No transaction started

        // Act & Assert - Should not throw an exception
        await _unitOfWork.RollbackTransactionAsync();
        
        // No exception means the test passes
        true.Should().BeTrue();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _unitOfWork?.Dispose();
        }
        base.Dispose(disposing);
    }
}
