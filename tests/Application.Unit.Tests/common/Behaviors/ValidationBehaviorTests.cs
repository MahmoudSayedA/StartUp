using Application.Common.Behaviours;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using Moq;

namespace Application.Unit.Tests.common.Behaviors;
public class ValidationBehaviorTests
{

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehaviour<TestRequest, Result>(validators);
        var request = new TestRequest();
        var nextCalled = false;
        RequestHandlerDelegate<Result> next = async (cancellation) =>
        {
            nextCalled = true;
            return await Task.FromResult(Result.Success());
        };
        // Act
        var response = await behavior.Handle(request, next, CancellationToken.None);
        // Assert
        Assert.True(nextCalled);
    }
    [Fact]
    public void Handle_WithValidatorsWithoutFailures_ShouldCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehaviour<TestRequest, Result>(validators);
        var request = new TestRequest();
        var nextCalled = false;
        RequestHandlerDelegate<Result> next = async (cancellation) =>
        {
            nextCalled = true;
            return await Task.FromResult(Result.Success());
        };
        // Act
        var response = behavior.Handle(request, next, CancellationToken.None);
        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task Handle_WithValidatorsWithFailures_ShouldThrowValidationExceptionAndNotCallNext()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<TestRequest>>();
        var failures = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Property1", "Error message 1"),
            new FluentValidation.Results.ValidationFailure("Property2", "Error message 2")
        };
        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(failures));
        var validators = new List<IValidator<TestRequest>> { validatorMock.Object };
        var behavior = new ValidationBehaviour<TestRequest, Result>(validators);
        var request = new TestRequest();
        var nextCalled = false;
        RequestHandlerDelegate<Result> next = async (cancellation) =>
        {
            nextCalled = true;
            return await Task.FromResult(Result.Success());
        };
        // Act & Assert
        await Assert.ThrowsAsync<Application.Common.Exceptions.ValidationException>(() =>
            behavior.Handle(request, next, CancellationToken.None));
        Assert.False(nextCalled);
    }


}
