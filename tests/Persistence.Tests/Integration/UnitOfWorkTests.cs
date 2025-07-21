using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;
using Application.Abstractions.Persistence;

namespace Persistence.Tests.Integration;

public class UnitOfWorkTests : BaseRepositoryTest
{
    private readonly UserRepository _userRepository;
    private readonly TaskItemRepository _taskItemRepository;

    public UnitOfWorkTests()
    {
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

        // Act & Assert
        if (Context.Database.IsInMemory())
        {
            // For in-memory database, just use direct SaveChanges without UnitOfWork
            var result = await Context.SaveChangesAsync();
            result.Should().BeGreaterThan(0);
            
            var savedUser = await _userRepository.GetByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be("unittest@test.com");
        }
        else
        {
            // For real database, use UnitOfWork with transactions
            var factory = new UnitOfWorkFactory(Context);
            using var unitOfWork = await factory.CreateAsync();
            
            var result = await unitOfWork.SaveChangesAsync();
            result.Should().BeGreaterThan(0);
            
            var savedUser = await unitOfWork.Users.GetByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
            savedUser!.Email.Should().Be("unittest@test.com");
        }
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
        var factory = new UnitOfWorkFactory(Context);
        using var unitOfWork = await factory.CreateAsync();
        
        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.Tasks.AddAsync(task);
        await unitOfWork.SaveChangesAsync();
        
        await unitOfWork.CommitTransactionAsync();

        // Assert
        var savedUser = await unitOfWork.Users.GetByIdAsync(user.Id);
        var savedTask = await unitOfWork.Tasks.GetByIdAsync(task.Id);

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
        var factory = new UnitOfWorkFactory(Context);
        using var unitOfWork = await factory.CreateAsync();
        
        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.Tasks.AddAsync(task);
        await unitOfWork.SaveChangesAsync();
        
        await unitOfWork.RollbackTransactionAsync();

        // Assert  
        // Create a new UnitOfWork to verify rollback worked
        using var verifyUnitOfWork = await factory.CreateAsync();
        var savedUser = await verifyUnitOfWork.Users.GetByIdAsync(user.Id);
        var savedTask = await verifyUnitOfWork.Tasks.GetByIdAsync(task.Id);

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
        var factory = new UnitOfWorkFactory(Context);
        using var unitOfWork = await factory.CreateAsync();
        
        // Add new user
        await unitOfWork.Users.AddAsync(newUser);
        
        // Add multiple tasks
        await unitOfWork.Tasks.AddRangeAsync(tasks);
        
        // Update existing user
        var existingUser = await unitOfWork.Users.GetByIdAsync(existingUserId);
        existingUser!.Role = "Admin";
        unitOfWork.Users.Update(existingUser);
        
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitTransactionAsync();

        // Assert
        var savedNewUser = await unitOfWork.Users.GetByIdAsync(newUser.Id);
        var savedTasks = await unitOfWork.Tasks.FindAsync(t => 
            t.Title == "Multi-op Task 1" || t.Title == "Multi-op Task 2");
        var updatedExistingUser = await unitOfWork.Users.GetByIdAsync(existingUserId);

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

        var factory = new UnitOfWorkFactory(Context);

        // Act & Assert - First successful transaction
        using (var unitOfWork1 = await factory.CreateAsync())
        {
            await unitOfWork1.Users.AddAsync(user1);
            await unitOfWork1.SaveChangesAsync();
            await unitOfWork1.CommitTransactionAsync();
        }

        // Verify first user was saved
        using (var verifyUnitOfWork1 = await factory.CreateAsync())
        {
            var savedUser1 = await verifyUnitOfWork1.Users.GetByIdAsync(user1.Id);
            savedUser1.Should().NotBeNull();
        }

        // Failed transaction (rollback)
        using (var unitOfWork2 = await factory.CreateAsync())
        {
            await unitOfWork2.Users.AddAsync(user2);
            await unitOfWork2.SaveChangesAsync();
            await unitOfWork2.RollbackTransactionAsync();
        }

        // Verify second user was not saved
        using (var verifyUnitOfWork2 = await factory.CreateAsync())
        {
            var savedUser2 = await verifyUnitOfWork2.Users.GetByIdAsync(user2.Id);
            savedUser2.Should().BeNull();
        }

        // Another successful transaction after rollback
        var user3 = UserBuilder.Create()
            .WithEmail("final@test.com")
            .WithUsername("finaluser")
            .WithEntraId("entra-final")
            .WithRole("User")
            .Build();

        using (var unitOfWork3 = await factory.CreateAsync())
        {
            await unitOfWork3.Users.AddAsync(user3);
            await unitOfWork3.SaveChangesAsync();
            await unitOfWork3.CommitTransactionAsync();
        }

        // Verify third user was saved
        using (var verifyUnitOfWork3 = await factory.CreateAsync())
        {
            var savedUser3 = await verifyUnitOfWork3.Users.GetByIdAsync(user3.Id);
            savedUser3.Should().NotBeNull();
        }
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

        // Act & Assert
        if (Context.Database.IsInMemory())
        {
            // For in-memory database, just use direct operations
            await _userRepository.AddAsync(user);
            var result = await Context.SaveChangesAsync();
            result.Should().BeGreaterThan(0);
            
            var savedUser = await _userRepository.GetByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
        }
        else
        {
            // For real database, use UnitOfWork
            var factory = new UnitOfWorkFactory(Context);
            using var unitOfWork = await factory.CreateAsync();
            
            await unitOfWork.Users.AddAsync(user);
            var result = await unitOfWork.SaveChangesAsync();
            result.Should().BeGreaterThan(0);
            
            var savedUser = await unitOfWork.Users.GetByIdAsync(user.Id);
            savedUser.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task UnitOfWork_WithCancellationToken_ShouldRespectCancellation()
    {
        // Skip this test for in-memory database since it doesn't support transactions
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("cancellation@test.com")
            .WithUsername("cancellationuser")
            .WithEntraId("entra-cancellation")
            .WithRole("User")
            .Build();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        var factory = new UnitOfWorkFactory(Context);
        
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => factory.CreateAsync(cts.Token));
    }

    [Fact]
    public async Task CommitTransactionAsync_WithoutBeginTransaction_ShouldNotThrow()
    {
        // Skip this test for in-memory database since it doesn't support transactions
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange & Act & Assert
        var factory = new UnitOfWorkFactory(Context);
        using var unitOfWork = await factory.CreateAsync();
        
        // Should not throw an exception
        await unitOfWork.CommitTransactionAsync();
        
        // No exception means the test passes
        true.Should().BeTrue();
    }

    [Fact]
    public async Task RollbackTransactionAsync_WithoutBeginTransaction_ShouldNotThrow()
    {
        // Skip this test for in-memory database since it doesn't support transactions
        if (Context.Database.IsInMemory())
        {
            return;
        }

        // Arrange & Act & Assert
        var factory = new UnitOfWorkFactory(Context);
        using var unitOfWork = await factory.CreateAsync();
        
        // Should not throw an exception
        await unitOfWork.RollbackTransactionAsync();
        
        // No exception means the test passes
        true.Should().BeTrue();
    }
}
