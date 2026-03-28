using Application.Commands.Boards;
using Application.Validators.Commands.Boards;

namespace Application.Tests.Validators.Commands.Boards;

public class CreateBoardCommandValidatorTests
{
    private readonly CreateBoardCommandValidator _validator;

    public CreateBoardCommandValidatorTests()
    {
        _validator = new CreateBoardCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
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
    public void Validate_WithInvalidName_ShouldHaveNameRequiredError(string name)
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: name,
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name) && e.ErrorMessage == "Board name is required.");
    }

    [Fact]
    public void Validate_WithNullName_ShouldHaveNameRequiredError()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: null!,
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name) && e.ErrorMessage == "Board name is required.");
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldHaveNameLengthError()
    {
        // Arrange
        var longName = new string('a', 201); // Over 200 characters
        var command = new CreateBoardCommand(
            Name: longName,
            Description: "Test Description",
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name) && e.ErrorMessage == "Board name must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_WithEmptyOwnerId_ShouldHaveOwnerIdRequiredError()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: "Test Description",
            OwnerId: Guid.Empty,
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.OwnerId) && e.ErrorMessage == "Owner ID is required.");
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: null,
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_ShouldHaveDescriptionLengthError()
    {
        // Arrange
        var longDescription = new string('a', 1001); // Over 1000 characters
        var command = new CreateBoardCommand(
            Name: "Test Board",
            Description: longDescription,
            OwnerId: Guid.NewGuid(),
            OrganizationId: Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Description) && e.ErrorMessage == "Description must not exceed 1000 characters.");
    }
}
