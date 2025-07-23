using Application.Commands.Tasks;
using Application.Validators.Commands.Tasks;

namespace Application.Tests.Validators.Commands.Tasks;

public class CreateTaskItemCommandValidatorTests
{
    private readonly CreateTaskItemCommandValidator _validator;

    public CreateTaskItemCommandValidatorTests()
    {
        _validator = new CreateTaskItemCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            TagId: 1,
            Title: "Test Task",
            Description: "Test Description",
            StateId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
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
    public void Validate_WithInvalidTitle_ShouldHaveTitleRequiredError(string title)
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: title,
            Description: "Test Description",
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title) && e.ErrorMessage == "Task title is required.");
    }

    [Fact]
    public void Validate_WithNullTitle_ShouldHaveTitleRequiredError()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: null!,
            Description: "Test Description",
            StateId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7),
            TagId: 1
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title) && e.ErrorMessage == "Task title is required.");
    }

    [Fact]
    public void Validate_WithTitleTooLong_ShouldHaveTitleLengthError()
    {
        // Arrange
        var longTitle = new string('a', 301); // Over 300 characters
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: longTitle,
            Description: "Test Description",
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title) && e.ErrorMessage == "Task title must not exceed 200 characters.");
    }

    [Fact]
    public void Validate_WithEmptyBoardId_ShouldHaveBoardIdRequiredError()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.Empty,
            Title: "Test Task",
            Description: "Test Description",
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.BoardId) && e.ErrorMessage == "Board ID is required.");
    }

    [Fact]
    public void Validate_WithInvalidStateId_ShouldHaveStateIdGreaterThanZeroError()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: "Test Task",
            Description: "Test Description",
            StateId: 0,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.StateId) && e.ErrorMessage == "State ID must be greater than 0.");
    }

    [Fact]
    public void Validate_WithNullOptionalFields_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: "Test Task",
            Description: null,
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: null,
            DueDate: null
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
        var longDescription = new string('a', 2001); // Over 2000 characters
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: "Test Task",
            Description: longDescription,
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(7)
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Description) && e.ErrorMessage == "Task description must not exceed 2000 characters.");
    }

    [Fact]
    public void Validate_WithPastDueDate_ShouldHaveDueDateFutureError()
    {
        // Arrange
        var command = new CreateTaskItemCommand(
            BoardId: Guid.NewGuid(),
            Title: "Test Task",
            Description: "Test Description",
            StateId: 1,
            TagId: 1,
            Priority: 2,
            AssigneeId: Guid.NewGuid(),
            DueDate: DateTime.UtcNow.AddDays(-1) // Past date
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.DueDate) && e.ErrorMessage == "Due date cannot be in the past.");
    }
}
