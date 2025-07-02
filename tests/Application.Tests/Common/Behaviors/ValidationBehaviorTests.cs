using Application.Common.Behaviors;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using FluentAssertions;
using ApplicationException = Application.Common.Exceptions.ValidationException;

namespace Application.Tests.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithNoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var response = new TestResponse();
        
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = (cancellationToken) =>
        {
            nextCalled = true;
            return Task.FromResult(response);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithValidatorsButNoErrors_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var response = new TestResponse();
        
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = (cancellationToken) =>
        {
            nextCalled = true;
            return Task.FromResult(response);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        nextCalled.Should().BeTrue();
        validatorMock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidationErrors_ShouldThrowValidationException()
    {
        // Arrange
        var validationFailures = new[]
        {
            new ValidationFailure("Property1", "Error message 1"),
            new ValidationFailure("Property2", "Error message 2")
        };

        var validationResult = new ValidationResult(validationFailures);

        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var validators = new[] { validatorMock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = (cancellationToken) =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        };

        // Act & Assert
        var exception = await behavior.Invoking(b => b.Handle(request, next, CancellationToken.None))
            .Should().ThrowAsync<ApplicationException>();

        exception.Which.Errors.Should().ContainKey("Property1");
        exception.Which.Errors.Should().ContainKey("Property2");
        exception.Which.Errors["Property1"].Should().Contain("Error message 1");
        exception.Which.Errors["Property2"].Should().Contain("Error message 2");

        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WithMultipleValidators_ShouldRunAllValidators()
    {
        // Arrange
        var validator1Mock = new Mock<IValidator<TestRequest>>();
        var validator2Mock = new Mock<IValidator<TestRequest>>();
        
        validator1Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
            
        validator2Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var validators = new[] { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var response = new TestResponse();
        
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = (cancellationToken) =>
        {
            nextCalled = true;
            return Task.FromResult(response);
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        nextCalled.Should().BeTrue();
        validator1Mock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
        validator2Mock.Verify(
            x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleValidatorsAndErrors_ShouldCombineAllErrors()
    {
        // Arrange
        var validator1Failures = new[]
        {
            new ValidationFailure("Property1", "Error from validator 1")
        };
        
        var validator2Failures = new[]
        {
            new ValidationFailure("Property2", "Error from validator 2"),
            new ValidationFailure("Property1", "Another error from validator 2")
        };

        var validationResult1 = new ValidationResult(validator1Failures);
        var validationResult2 = new ValidationResult(validator2Failures);

        var validator1Mock = new Mock<IValidator<TestRequest>>();
        var validator2Mock = new Mock<IValidator<TestRequest>>();
        
        validator1Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult1);
            
        validator2Mock
            .Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult2);

        var validators = new[] { validator1Mock.Object, validator2Mock.Object };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        
        var nextCalled = false;
        RequestHandlerDelegate<TestResponse> next = (cancellationToken) =>
        {
            nextCalled = true;
            return Task.FromResult(new TestResponse());
        };

        // Act & Assert
        var exception = await behavior.Invoking(b => b.Handle(request, next, CancellationToken.None))
            .Should().ThrowAsync<ApplicationException>();

        exception.Which.Errors.Should().ContainKey("Property1");
        exception.Which.Errors.Should().ContainKey("Property2");
        exception.Which.Errors["Property1"].Should().HaveCount(2);
        exception.Which.Errors["Property2"].Should().HaveCount(1);

        nextCalled.Should().BeFalse();
    }

    // Test classes for the behavior tests
    public class TestRequest
    {
        public string Property1 { get; set; } = string.Empty;
        public string Property2 { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
    }
}
