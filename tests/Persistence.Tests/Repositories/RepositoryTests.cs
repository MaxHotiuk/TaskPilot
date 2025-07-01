using Domain.Entities;
using FluentAssertions;
using Persistence.Repositories;
using Persistence.Tests.Builders;
using Persistence.Tests.Infrastructure;

namespace Persistence.Tests.Repositories;

public class RepositoryTests : BaseRepositoryTest
{
    private readonly Repository<User, Guid> _userRepository;

    public RepositoryTests()
    {
        _userRepository = new Repository<User, Guid>(Context);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        var result = await _userRepository.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Email.Should().Be("user1@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _userRepository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WithEntitiesInDatabase_ShouldReturnAllEntities()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(u => u.Email).Should().Contain(new[] { "user1@test.com", "user2@test.com" });
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyDatabase_ShouldReturnEmptyCollection()
    {
        // Arrange - Empty database (no seeding)

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_WithValidPredicate_ShouldReturnMatchingEntities()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.FindAsync(u => u.Email.Contains("user1"));

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Email.Should().Be("user1@test.com");
    }

    [Fact]
    public async Task FindAsync_WithPredicateNoMatches_ShouldReturnEmptyCollection()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.FindAsync(u => u.Email.Contains("nonexistent"));

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithValidPredicate_ShouldReturnFirstMatchingEntity()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.FirstOrDefaultAsync(u => u.Username == "user1");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("user1");
        result.Email.Should().Be("user1@test.com");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithPredicateNoMatches_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.FirstOrDefaultAsync(u => u.Username == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_ShouldAddEntityToDatabase()
    {
        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("newuser@test.com")
            .WithUsername("newuser")
            .WithEntraId("entra-new-user")
            .WithRole("User")
            .Build();

        // Act
        await _userRepository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Assert
        var addedUser = await Context.Users.FindAsync(user.Id);
        addedUser.Should().NotBeNull();
        addedUser!.Email.Should().Be("newuser@test.com");
        addedUser.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task AddRangeAsync_WithValidEntities_ShouldAddAllEntitiesToDatabase()
    {
        // Arrange
        var users = new[]
        {
            UserBuilder.Create().WithEmail("user1@batch.com").WithUsername("batch1").Build(),
            UserBuilder.Create().WithEmail("user2@batch.com").WithUsername("batch2").Build(),
            UserBuilder.Create().WithEmail("user3@batch.com").WithUsername("batch3").Build()
        };

        // Act
        await _userRepository.AddRangeAsync(users);
        await Context.SaveChangesAsync();

        // Assert
        var allUsers = await _userRepository.GetAllAsync();
        allUsers.Should().HaveCount(3);
        allUsers.Select(u => u.Email).Should().Contain(new[] 
        { 
            "user1@batch.com", 
            "user2@batch.com", 
            "user3@batch.com" 
        });
    }

    [Fact]
    public async Task Update_WithValidEntity_ShouldUpdateEntityInDatabase()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var user = await _userRepository.GetByIdAsync(userId);
        user!.Username = "UpdatedUser1";
        user.Email = "updated@test.com";

        // Act
        _userRepository.Update(user);
        await Context.SaveChangesAsync();

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(userId);
        updatedUser.Should().NotBeNull();
        updatedUser!.Username.Should().Be("UpdatedUser1");
        updatedUser.Email.Should().Be("updated@test.com");
    }

    [Fact]
    public async Task Remove_WithValidEntity_ShouldRemoveEntityFromDatabase()
    {
        // Arrange
        await SeedDatabaseAsync();
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var user = await _userRepository.GetByIdAsync(userId);

        // Remove dependent entities first to avoid cascade delete issues
        var dependentBoards = Context.Boards.Where(b => b.OwnerId == userId);
        Context.Boards.RemoveRange(dependentBoards);
        await Context.SaveChangesAsync();

        // Act
        _userRepository.Remove(user!);
        await Context.SaveChangesAsync();

        // Assert
        var removedUser = await _userRepository.GetByIdAsync(userId);
        removedUser.Should().BeNull();
        
        var remainingUsers = await _userRepository.GetAllAsync();
        remainingUsers.Should().HaveCount(1);
    }

    [Fact]
    public async Task RemoveRange_WithValidEntities_ShouldRemoveAllEntitiesFromDatabase()
    {
        // Arrange
        await SeedDatabaseAsync();
        var allUsers = await _userRepository.GetAllAsync();

        // Remove dependent entities first to avoid cascade delete issues
        Context.Boards.RemoveRange(Context.Boards);
        Context.Tasks.RemoveRange(Context.Tasks);
        Context.States.RemoveRange(Context.States);
        await Context.SaveChangesAsync();

        // Act
        _userRepository.RemoveRange(allUsers);
        await Context.SaveChangesAsync();

        // Assert
        var remainingUsers = await _userRepository.GetAllAsync();
        remainingUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task Repository_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        await SeedDatabaseAsync();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _userRepository.GetAllAsync(cts.Token));
    }
}
