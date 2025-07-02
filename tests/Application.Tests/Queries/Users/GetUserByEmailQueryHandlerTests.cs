using Application.Queries.Users;

namespace Application.Tests.Queries.Users;

public class GetUserByEmailQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserByEmailQueryHandler _handler;

    public GetUserByEmailQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _handler = new GetUserByEmailQueryHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        const string email = "test@example.com";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = "testuser",
            Role = "User",
            EntraId = "entra-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetUserByEmailQuery(email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedUser.Id);
        result.EntraId.Should().Be(expectedUser.EntraId);
        result.Email.Should().Be(email);
        result.Username.Should().Be(expectedUser.Username);
        result.Role.Should().Be(expectedUser.Role);

        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        const string email = "nonexistent@example.com";
        var query = new GetUserByEmailQuery(email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("admin@company.org")]
    [InlineData("test.user+tag@domain.co.uk")]
    [InlineData("simple@test.io")]
    public async Task Handle_WithVariousValidEmails_ShouldCallRepositoryWithCorrectEmail(string email)
    {
        // Arrange
        var query = new GetUserByEmailQuery(email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmailCasing_ShouldPreserveOriginalCasing()
    {
        // Arrange
        const string email = "Test.User@EXAMPLE.COM";
        var query = new GetUserByEmailQuery(email);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
