using Application.Abstractions.Authentication;
using Application.Queries.Users;

namespace Application.Tests.Queries.Users;

public class GetUserByIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrganizationMemberRepository> _organizationMemberRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IAuthenticationService> _authenticationServiceMock;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _organizationMemberRepositoryMock = _fixture.Freeze<Mock<IOrganizationMemberRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        _authenticationServiceMock = _fixture.Freeze<Mock<IAuthenticationService>>();

        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkMock.Setup(x => x.OrganizationMembers).Returns(_organizationMemberRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);

        _handler = new GetUserByIdQueryHandler(_authenticationServiceMock.Object, _unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var currentEntraId = "current-entra-id";

        var currentUser = new User
        {
            Id = currentUserId,
            Email = "current@example.com",
            Username = "currentuser",
            Role = "User",
            EntraId = currentEntraId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var expectedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            Role = "User",
            EntraId = "entra-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetUserByIdQuery(userId);

        _authenticationServiceMock
            .Setup(x => x.GetCurrentUserEntraIdAsync())
            .ReturnsAsync(currentEntraId);

        _userRepositoryMock
            .Setup(x => x.GetByEntraIdAsync(currentEntraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(currentUser);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        _organizationMemberRepositoryMock
            .Setup(x => x.GetOrganizationIdsByUserIdAsync(currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { organizationId });

        _organizationMemberRepositoryMock
            .Setup(x => x.GetOrganizationIdsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { organizationId });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.EntraId.Should().Be(expectedUser.EntraId);
        result.Email.Should().Be("test@example.com");
        result.Username.Should().Be("testuser");
        result.Role.Should().Be(expectedUser.Role);

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetUserByIdQuery(userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")]
    public async Task Handle_WithVariousValidIds_ShouldCallRepositoryWithCorrectId(string guidString)
    {
        // Arrange
        var userId = Guid.Parse(guidString);
        var query = new GetUserByIdQuery(userId);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
