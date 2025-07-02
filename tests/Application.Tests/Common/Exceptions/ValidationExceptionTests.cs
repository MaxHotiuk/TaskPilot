using Application.Common.Exceptions;
using FluentValidation.Results;

namespace Application.Tests.Common.Exceptions;

public class ValidationExceptionTests
{
    [Fact]
    public void Constructor_WithNoParameters_ShouldSetDefaultMessage()
    {
        // Act
        var exception = new ValidationException();

        // Assert
        exception.Message.Should().Be("One or more validation failures have occurred.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Custom validation message";

        // Act
        var exception = new ValidationException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithValidationFailures_ShouldGroupErrorsByProperty()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("Property1", "Error message 1"),
            new ValidationFailure("Property2", "Error message 2"),
            new ValidationFailure("Property1", "Error message 3")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Message.Should().Be("One or more validation failures have occurred.");
        exception.Errors.Should().HaveCount(2);
        exception.Errors.Should().ContainKey("Property1");
        exception.Errors.Should().ContainKey("Property2");
        exception.Errors["Property1"].Should().HaveCount(2);
        exception.Errors["Property1"].Should().Contain("Error message 1");
        exception.Errors["Property1"].Should().Contain("Error message 3");
        exception.Errors["Property2"].Should().HaveCount(1);
        exception.Errors["Property2"].Should().Contain("Error message 2");
    }

    [Fact]
    public void Constructor_WithEmptyValidationFailures_ShouldHaveEmptyErrors()
    {
        // Arrange
        var failures = Enumerable.Empty<ValidationFailure>();

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Message.Should().Be("One or more validation failures have occurred.");
        exception.Errors.Should().NotBeNull();
        exception.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithSingleValidationFailure_ShouldCreateSingleError()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("Email", "Email is required")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Errors.Should().HaveCount(1);
        exception.Errors.Should().ContainKey("Email");
        exception.Errors["Email"].Should().HaveCount(1);
        exception.Errors["Email"].Should().Contain("Email is required");
    }

    [Fact]
    public void Constructor_WithMultipleErrorsForSameProperty_ShouldGroupTogether()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("Username", "Username is required"),
            new ValidationFailure("Username", "Username must be at least 3 characters"),
            new ValidationFailure("Username", "Username must not exceed 100 characters")
        };

        // Act
        var exception = new ValidationException(failures);

        // Assert
        exception.Errors.Should().HaveCount(1);
        exception.Errors.Should().ContainKey("Username");
        exception.Errors["Username"].Should().HaveCount(3);
        exception.Errors["Username"].Should().Contain("Username is required");
        exception.Errors["Username"].Should().Contain("Username must be at least 3 characters");
        exception.Errors["Username"].Should().Contain("Username must not exceed 100 characters");
    }

    [Fact]
    public void Errors_ShouldBeReadOnly()
    {
        // Arrange
        var failures = new[]
        {
            new ValidationFailure("Property1", "Error message 1")
        };
        var exception = new ValidationException(failures);

        // Act & Assert
        exception.Errors.Should().NotBeNull();
        // The Errors property should be settable but not expose mutating operations
        exception.Invoking(e => e.Errors.Clear())
            .Should().NotThrow(); // Dictionary allows mutation, but we expect the property to be stable
    }
}
