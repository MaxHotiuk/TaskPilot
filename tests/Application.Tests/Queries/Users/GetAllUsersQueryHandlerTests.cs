using Application.Queries.Users;

namespace Application.Tests.Queries.Users;

public class GetAllUsersQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly GetAllUsersQueryHandler _handler;

    public GetAllUsersQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new GetAllUsersQueryHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithUsersInRepository_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "user1@example.com",
                Username = "user1",
                Role = "User",
                EntraId = "entra-id-1",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "user2@example.com",
                Username = "user2",
                Role = "Admin",
                EntraId = "entra-id-2",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "user3@example.com",
                Username = "user3",
                Role = "User",
                EntraId = "entra-id-3",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var query = new GetAllUsersQuery();

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        
        var resultArray = result.ToArray();
        resultArray[0].Id.Should().Be(expectedUsers[0].Id);
        resultArray[0].EntraId.Should().Be(expectedUsers[0].EntraId);
        resultArray[0].Username.Should().Be(expectedUsers[0].Username);
        resultArray[0].Email.Should().Be(expectedUsers[0].Email);
        resultArray[0].Role.Should().Be(expectedUsers[0].Role);
        
        resultArray[1].Id.Should().Be(expectedUsers[1].Id);
        resultArray[1].EntraId.Should().Be(expectedUsers[1].EntraId);
        resultArray[1].Username.Should().Be(expectedUsers[1].Username);
        resultArray[1].Email.Should().Be(expectedUsers[1].Email);
        resultArray[1].Role.Should().Be(expectedUsers[1].Role);
        
        resultArray[2].Id.Should().Be(expectedUsers[2].Id);
        resultArray[2].EntraId.Should().Be(expectedUsers[2].EntraId);
        resultArray[2].Username.Should().Be(expectedUsers[2].Username);
        resultArray[2].Email.Should().Be(expectedUsers[2].Email);
        resultArray[2].Role.Should().Be(expectedUsers[2].Role);

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithEmptyRepository_ShouldReturnEmptyCollection()
    {
        // Arrange
        var query = new GetAllUsersQuery();

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<User>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithSingleUser_ShouldReturnSingleUserCollection()
    {
        // Arrange
        var singleUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "single@example.com",
            Username = "singleuser",
            Role = "User",
            EntraId = "single-entra-id",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var query = new GetAllUsersQuery();

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { singleUser });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        
        var firstResult = result.First();
        firstResult.Id.Should().Be(singleUser.Id);
        firstResult.EntraId.Should().Be(singleUser.EntraId);
        firstResult.Username.Should().Be(singleUser.Username);
        firstResult.Email.Should().Be(singleUser.Email);
        firstResult.Role.Should().Be(singleUser.Role);

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(100)]
    public async Task Handle_WithVariousUserCounts_ShouldReturnCorrectCount(int userCount)
    {
        // Arrange
        var users = Enumerable.Range(1, userCount)
            .Select(i => new User
            {
                Id = Guid.NewGuid(),
                Email = $"user{i}@example.com",
                Username = $"user{i}",
                Role = i % 2 == 0 ? "Admin" : "User",
                EntraId = $"entra-id-{i}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            })
            .ToArray();

        var query = new GetAllUsersQuery();

        _userRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(userCount);

        _userRepositoryMock.Verify(
            x => x.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
