using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit.Abstractions;

namespace Tests.CommonUtils.Helpers;

public static class LoggerUtils
{
    public static ILoggerFactory CreateLoggerFactory(ITestOutputHelper output, string name)
    {
        var mockLoggerFactory = new Mock<ILoggerFactory>();

        _ = mockLoggerFactory
            .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
            .Returns<string>(_ => GetLogger(output, name));

        return mockLoggerFactory.Object;
    }

    public static ILogger<T> GetLogger<T>(ITestOutputHelper output)
    {
        return GetLogger<ILogger<T>>(output, typeof(T).Name);
    }

    public static TLogger GetLogger<TLogger>(ITestOutputHelper output, string name)
        where TLogger : class, ILogger
    {
        var mockLogger = new Mock<TLogger>();
        _ = mockLogger
            .Setup(logger => logger.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction((invocation) => output.WriteLine(
                $"{Environment.CurrentManagedThreadId}> [{DateTime.UtcNow.TimeOfDay} | {invocation.Arguments[0]} | {name}] " +
                $"{invocation.Arguments[2]} {invocation.Arguments[3]}")));

        _ = mockLogger
            .Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
            .Returns(true);

        return mockLogger.Object;
    }

    private static ILogger GetLogger(ITestOutputHelper output, string name)
    {
        return GetLogger<ILogger>(output, name);
    }
}
