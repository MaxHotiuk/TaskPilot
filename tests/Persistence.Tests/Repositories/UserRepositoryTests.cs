using FluentAssertions;
using Persistence.Repositories;
using Persistence.Tests.Builders;
using Persistence.Tests.Infrastructure;

namespace Persistence.Tests.Repositories;

public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _userRepository = new UserRepository(Context);
    }

    [Fact]
    public async Task GetByEntraIdAsync_WithValidEntraId_ShouldReturnUser()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string entraId = "entra-user-1";

        // Act
        var result = await _userRepository.GetByEntraIdAsync(entraId);

        // Assert
        result.Should().NotBeNull();
        result!.EntraId.Should().Be(entraId);
        result.Email.Should().Be("user1@test.com");
        result.Username.Should().Be("user1");
    }

    [Fact]
    public async Task GetByEntraIdAsync_WithInvalidEntraId_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string nonExistentEntraId = "non-existent-entra-id";

        // Act
        var result = await _userRepository.GetByEntraIdAsync(nonExistentEntraId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string email = "user1@test.com";

        // Act
        var result = await _userRepository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        result.Username.Should().Be("user1");
        result.EntraId.Should().Be("entra-user-1");
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string nonExistentEmail = "nonexistent@test.com";

        // Act
        var result = await _userRepository.GetByEmailAsync(nonExistentEmail);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("USER1@TEST.COM")] // Different case
    [InlineData("User1@Test.Com")] // Mixed case
    public async Task GetByEmailAsync_WithDifferentCasing_ShouldHandleCaseInsensitivity(string emailToSearch)
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync(emailToSearch);

        // Assert
        // Note: This test documents current behavior. 
        // If email lookup should be case-insensitive, the repository implementation would need to change
        result.Should().BeNull(); // Current implementation is case-sensitive
    }

    [Fact]
    public async Task ExistsByEntraIdAsync_WithExistingEntraId_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string entraId = "entra-user-1";

        // Act
        var result = await _userRepository.ExistsByEntraIdAsync(entraId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEntraIdAsync_WithNonExistingEntraId_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string nonExistentEntraId = "non-existent-entra-id";

        // Act
        var result = await _userRepository.ExistsByEntraIdAsync(nonExistentEntraId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string email = "user1@test.com";

        // Act
        var result = await _userRepository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        await SeedDatabaseAsync();
        const string nonExistentEmail = "nonexistent@test.com";

        // Act
        var result = await _userRepository.ExistsByEmailAsync(nonExistentEmail);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithEmptyDatabase_ShouldReturnFalse()
    {
        // Arrange - Empty database (no seeding)
        const string email = "user1@test.com";

        // Act
        var result = await _userRepository.ExistsByEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UserRepository_WithMultipleUsersHavingSameRole_ShouldHandleCorrectly()
    {
        // Arrange
        await SeedDatabaseAsync();
        
        var additionalUsers = new[]
        {
            UserBuilder.Create()
                .WithEmail("admin1@test.com")
                .WithUsername("admin1")
                .WithEntraId("entra-admin-1")
                .WithRole("Admin")
                .Build(),
            UserBuilder.Create()
                .WithEmail("admin2@test.com")
                .WithUsername("admin2")
                .WithEntraId("entra-admin-2")
                .WithRole("Admin")
                .Build()
        };

        await Context.Users.AddRangeAsync(additionalUsers);
        await Context.SaveChangesAsync();

        // Act
        var userResult = await _userRepository.GetByEmailAsync("user1@test.com");
        var adminResult = await _userRepository.GetByEmailAsync("admin1@test.com");

        // Assert
        userResult.Should().NotBeNull();
        userResult!.Role.Should().Be("User");
        
        adminResult.Should().NotBeNull();
        adminResult!.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Repository_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        await SeedDatabaseAsync();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _userRepository.GetByEmailAsync("user1@test.com", cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _userRepository.GetByEntraIdAsync("entra-user-1", cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _userRepository.ExistsByEmailAsync("user1@test.com", cts.Token));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _userRepository.ExistsByEntraIdAsync("entra-user-1", cts.Token));
    }

    [Fact]
    public async Task Repository_ShouldInheritBaseRepositoryFunctionality()
    {
        // Arrange
        var user = UserBuilder.Create()
            .WithEmail("inheritance@test.com")
            .WithUsername("inheritanceuser")
            .WithEntraId("entra-inheritance")
            .WithRole("User")
            .Build();

        // Act - Test inherited AddAsync functionality
        await _userRepository.AddAsync(user);
        await Context.SaveChangesAsync();

        // Assert
        var addedUser = await _userRepository.GetByIdAsync(user.Id);
        addedUser.Should().NotBeNull();
        addedUser!.Email.Should().Be("inheritance@test.com");

        // Act - Test specific UserRepository functionality
        var userByEmail = await _userRepository.GetByEmailAsync("inheritance@test.com");
        var userByEntraId = await _userRepository.GetByEntraIdAsync("entra-inheritance");

        // Assert
        userByEmail.Should().NotBeNull();
        userByEmail!.Id.Should().Be(user.Id);
        
        userByEntraId.Should().NotBeNull();
        userByEntraId!.Id.Should().Be(user.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByEntraIdAsync_WithEmptyOrWhitespaceEntraId_ShouldReturnNull(string entraId)
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.GetByEntraIdAsync(entraId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetByEmailAsync_WithEmptyOrWhitespaceEmail_ShouldReturnNull(string email)
    {
        // Arrange
        await SeedDatabaseAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync(email);

        // Assert
        result.Should().BeNull();
    }
}
