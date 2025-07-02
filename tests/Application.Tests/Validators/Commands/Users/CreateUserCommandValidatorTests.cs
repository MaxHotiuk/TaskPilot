using Application.Commands.Users;
using Application.Validators.Commands.Users;

namespace Application.Tests.Validators.Commands.Users;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _validator = new CreateUserCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidEmail_ShouldHaveEmailRequiredError(string email)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: email,
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email) && e.ErrorMessage == "Email is required.");
    }

    [Fact]
    public void Validate_WithNullEmail_ShouldHaveEmailRequiredError()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: null!,
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email) && e.ErrorMessage == "Email is required.");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid.email")]
    [InlineData("invalid email space")]
    public void Validate_WithInvalidEmailFormat_ShouldHaveEmailFormatError(string email)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: email,
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email) && e.ErrorMessage == "Email must be a valid email address.");
    }

    [Fact]
    public void Validate_WithEmailTooLong_ShouldHaveEmailLengthError()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@test.com"; // Over 256 characters
        var command = new CreateUserCommand(
            Email: longEmail,
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email) && e.ErrorMessage == "Email must not exceed 256 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidUsername_ShouldHaveUsernameRequiredError(string username)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: username,
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username) && e.ErrorMessage == "Username is required.");
    }

    [Fact]
    public void Validate_WithNullUsername_ShouldHaveUsernameRequiredError()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: null!,
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username) && e.ErrorMessage == "Username is required.");
    }

    [Theory]
    [InlineData("ab")] // 2 characters
    [InlineData("x")]  // 1 character
    public void Validate_WithUsernameTooShort_ShouldHaveUsernameMinLengthError(string username)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: username,
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username) && e.ErrorMessage == "Username must be at least 3 characters.");
    }

    [Fact]
    public void Validate_WithUsernameTooLong_ShouldHaveUsernameMaxLengthError()
    {
        // Arrange
        var longUsername = new string('a', 101); // Over 100 characters
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: longUsername,
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username) && e.ErrorMessage == "Username must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidEntraId_ShouldHaveEntraIdRequiredError(string entraId)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: entraId,
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.EntraId) && e.ErrorMessage == "EntraId is required.");
    }

    [Fact]
    public void Validate_WithNullEntraId_ShouldHaveEntraIdRequiredError()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: null!,
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.EntraId) && e.ErrorMessage == "EntraId is required.");
    }

    [Fact]
    public void Validate_WithEntraIdTooLong_ShouldHaveEntraIdMaxLengthError()
    {
        // Arrange
        var longEntraId = new string('a', 101); // Over 100 characters
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: longEntraId,
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.EntraId) && e.ErrorMessage == "EntraId must not exceed 100 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidRole_ShouldHaveRoleRequiredError(string role)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: role
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Role) && e.ErrorMessage == "Role is required.");
    }

    [Fact]
    public void Validate_WithNullRole_ShouldHaveRoleRequiredError()
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: null!
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Role) && e.ErrorMessage == "Role is required.");
    }

    [Fact]
    public void Validate_WithRoleTooLong_ShouldHaveRoleMaxLengthError()
    {
        // Arrange
        var longRole = new string('a', 51); // Over 50 characters
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: longRole
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Role) && e.ErrorMessage == "Role must not exceed 50 characters.");
    }

    [Theory]
    [InlineData("abc")] // Minimum length
    [InlineData("testuser")]
    [InlineData("test_user")]
    [InlineData("test-user")]
    [InlineData("TestUser123")]
    public void Validate_WithValidUsernames_ShouldNotHaveUsernameErrors(string username)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: "test@example.com",
            Username: username,
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(command.Username));
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("test+tag@example.co.uk")]
    [InlineData("simple@test.io")]
    public void Validate_WithValidEmails_ShouldNotHaveEmailErrors(string email)
    {
        // Arrange
        var command = new CreateUserCommand(
            Email: email,
            Username: "testuser",
            EntraId: "entra-id-123",
            Role: "User"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == nameof(command.Email));
    }
}
