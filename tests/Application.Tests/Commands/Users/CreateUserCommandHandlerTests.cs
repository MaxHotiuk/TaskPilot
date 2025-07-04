using Application.Commands.Users;
using Application.Common.Exceptions;

namespace Application.Tests.Commands.Users;

public class CreateUserCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _unitOfWorkFactoryMock = _fixture.Freeze<Mock<IUnitOfWorkFactory>>();
        
        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _unitOfWorkFactoryMock.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_unitOfWorkMock.Object);
        
        _handler = new CreateUserCommandHandler(_unitOfWorkFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateUserAndReturnId()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.ExistsByEntraIdAsync(command.EntraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => 
                u.Email == command.Email &&
                u.Username == command.Username &&
                u.EntraId == command.EntraId &&
                u.Role == command.Role &&
                u.Id != Guid.Empty
            ), It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "existing@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("User with this email already exists");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithExistingEntraId_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "existing-entra-id",
            Role: "User"
        );

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.ExistsByEntraIdAsync(command.EntraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("User with this EntraId already exists");

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldSetCorrectTimestamps()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        var beforeExecution = DateTime.UtcNow;

        _userRepositoryMock
            .Setup(x => x.ExistsByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepositoryMock
            .Setup(x => x.ExistsByEntraIdAsync(command.EntraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        var afterExecution = DateTime.UtcNow;

        // Assert
        _userRepositoryMock.Verify(
            x => x.AddAsync(It.Is<User>(u => 
                u.CreatedAt >= beforeExecution &&
                u.CreatedAt <= afterExecution &&
                u.UpdatedAt >= beforeExecution &&
                u.UpdatedAt <= afterExecution
            ), It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
