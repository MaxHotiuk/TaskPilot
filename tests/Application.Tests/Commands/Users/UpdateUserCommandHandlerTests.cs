using Application.Commands.Users;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Users;

public class UpdateUserCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            Username = "oldusername",
            Role = "User",
            EntraId = "entra-id",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var command = new UpdateUserCommand(
            Id: userId,
            Email: "new@example.com",
            Username: "newusername",
            Role: "Admin"
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var beforeUpdate = DateTime.UtcNow;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterUpdate = DateTime.UtcNow;

        // Assert
        existingUser.Email.Should().Be(command.Email);
        existingUser.Username.Should().Be(command.Username);
        existingUser.Role.Should().Be(command.Role);
        existingUser.UpdatedAt.Should().BeOnOrAfter(beforeUpdate);
        existingUser.UpdatedAt.Should().BeOnOrBefore(afterUpdate);

        _userRepositoryMock.Verify(x => x.Update(existingUser), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateUserCommand(
            Id: Guid.NewGuid(),
            Email: "test@example.com",
            Username: "testuser",
            Role: "User"
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"User\" ({command.Id}) was not found.");

        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithEmailTakenByAnotherUser_ShouldThrowValidationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        
        var existingUser = new User
        {
            Id = userId,
            Email = "old@example.com",
            Username = "username",
            Role = "User",
            EntraId = "entra-id"
        };

        var anotherUser = new User
        {
            Id = anotherUserId,
            Email = "taken@example.com",
            Username = "anotherusername",
            Role = "User",
            EntraId = "another-entra-id"
        };

        var command = new UpdateUserCommand(
            Id: userId,
            Email: "taken@example.com", // Email taken by another user
            Username: "newusername",
            Role: "User"
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(anotherUser);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("Email is already taken by another user");

        _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithSameUserEmail_ShouldUpdateSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingUser = new User
        {
            Id = userId,
            Email = "same@example.com",
            Username = "oldusername",
            Role = "User",
            EntraId = "entra-id"
        };

        var command = new UpdateUserCommand(
            Id: userId,
            Email: "same@example.com", // Same email as existing user
            Username: "newusername",
            Role: "Admin"
        );

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser); // Same user

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingUser.Email.Should().Be(command.Email);
        existingUser.Username.Should().Be(command.Username);
        existingUser.Role.Should().Be(command.Role);

        _userRepositoryMock.Verify(x => x.Update(existingUser), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
