using Application.Common.Behaviours;
using Application.Common.Models;
using Application.Identity.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Unit.Tests.common.Behaviors;
public partial class PerformanceBehaviorTests
{
    public void Handle_WithRequestTakeTimeMoreThan500ms_ShouldLogWarning()
    {
        //// Arrange
        //var request = new TestRequest();
        //var identityServiceMock = new Mock<IIdentityService>();
        //identityServiceMock.Setup(s => s.GetUserNameAsync(It.IsAny<string>()))
        //    .ReturnsAsync("TestUser");
        //var loggerMock = new Mock<ILogger<TestRequest>>();
        //var userMock = new Mock<IUser>();
        //userMock.Setup(u => u.Id).Returns("TestUserId");
        //var behavior = new PerformanceBehaviour<TestRequest, Unit>(
        //    loggerMock.Object,
        //    userMock.Object,
        //    identityServiceMock.Object);
        //RequestHandlerDelegate<Unit> next = async (can) =>
        //{
        //    Task.Delay(1000).Wait();
        //    //await Task.Delay(1000); // Simulate a delay less than 500ms
        //    return await Task.FromResult(Unit.Value);
        //};

        //// Act
        //behavior.Handle(request, next, CancellationToken.None).Wait();

        //// Assert
        //loggerMock.Verify(logger => logger.Log(
        //        It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
        //        It.Is<EventId>(eventId => eventId.Id == 0),
        //        It.Is<It.IsAnyType>((@object, @type) => string.IsNullOrEmpty(@object.ToString())),
        //        It.IsAny<Exception>(),
        //        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        //    Times.Once);

        //// message: It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == "myMessage" && @type.Name == "FormattedLogValues"),

    }

}
