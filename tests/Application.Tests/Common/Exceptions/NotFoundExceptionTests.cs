using Application.Common.Exceptions;

namespace Application.Tests.Common.Exceptions;

public class NotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithNoParameters_ShouldSetDefaultMessage()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        exception.Message.Should().Be("Exception of type 'Application.Common.Exceptions.NotFoundException' was thrown.");
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Resource not found";

        // Act
        var exception = new NotFoundException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
    {
        // Arrange
        const string message = "Resource not found";
        var innerException = new InvalidOperationException("Inner exception");

        // Act
        var exception = new NotFoundException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Constructor_WithNameAndKey_ShouldCreateFormattedMessage()
    {
        // Arrange
        const string name = "User";
        var key = Guid.Parse("12345678-1234-1234-1234-123456789012");

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" ({key}) was not found.");
        exception.InnerException.Should().BeNull();
    }

    [Theory]
    [InlineData("User", "123")]
    [InlineData("Product", 456)]
    [InlineData("Order", "ORDER-789")]
    [InlineData("Customer", 0)]
    public void Constructor_WithVariousNameAndKeyTypes_ShouldCreateCorrectMessage(string name, object key)
    {
        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" ({key}) was not found.");
    }

    [Fact]
    public void Constructor_WithNameAndNullKey_ShouldHandleNullKey()
    {
        // Arrange
        const string name = "User";
        object key = null!;

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be($"Entity \"{name}\" () was not found.");
    }

    [Fact]
    public void Constructor_WithNameAndGuidKey_ShouldFormatGuidCorrectly()
    {
        // Arrange
        const string name = "User";
        var key = Guid.Parse("12345678-1234-1234-1234-123456789012");

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be("Entity \"User\" (12345678-1234-1234-1234-123456789012) was not found.");
    }

    [Fact]
    public void Constructor_WithNameAndIntegerKey_ShouldFormatIntegerCorrectly()
    {
        // Arrange
        const string name = "Product";
        const int key = 42;

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        exception.Message.Should().Be("Entity \"Product\" (42) was not found.");
    }

    [Fact]
    public void ShouldInheritFromException()
    {
        // Act
        var exception = new NotFoundException();

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
