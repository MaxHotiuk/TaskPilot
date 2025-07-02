using Application.Queries.Users;

namespace Application.Tests.Queries.Users;

public class GetUserByEntraIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserByEntraIdQueryHandler _handler;

    public GetUserByEntraIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _handler = new GetUserByEntraIdQueryHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidEntraId_ShouldReturnUser()
    {
        // Arrange
        const string entraId = "entra-id-12345";
        var expectedUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            Role = "User",
            EntraId = entraId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetUserByEntraIdQuery(entraId);

        _userRepositoryMock
            .Setup(x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(expectedUser.Id);
        result.EntraId.Should().Be(entraId);
        result.Email.Should().Be(expectedUser.Email);
        result.Username.Should().Be(expectedUser.Username);
        result.Role.Should().Be(expectedUser.Role);

        _userRepositoryMock.Verify(
            x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentEntraId_ShouldReturnNull()
    {
        // Arrange
        const string entraId = "nonexistent-entra-id";
        var query = new GetUserByEntraIdQuery(entraId);

        _userRepositoryMock
            .Setup(x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _userRepositoryMock.Verify(
            x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("entra-id-123")]
    [InlineData("user-object-id-456")]
    [InlineData("aad-object-id-789")]
    [InlineData("12345678-1234-1234-1234-123456789012")]
    public async Task Handle_WithVariousValidEntraIds_ShouldCallRepositoryWithCorrectId(string entraId)
    {
        // Arrange
        var query = new GetUserByEntraIdQuery(entraId);

        _userRepositoryMock
            .Setup(x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByEntraIdAsync(entraId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
