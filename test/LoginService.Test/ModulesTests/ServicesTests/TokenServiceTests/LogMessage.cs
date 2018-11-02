using System;
using System.Diagnostics;
using JwtAuth.LoginService.Modules.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LoginService.Test.ModulesTests.ServicesTests.TokenServiceTests
{
    [TestClass]
    public class LogMessage
    {
        private Mock<MockLogger<TokenService>> _logger;
        TokenService _service;

        [TestInitialize]
        public void Initialize()
        {
            _logger = new Mock<MockLogger<TokenService>>();

            _service = new TokenService(_logger.Object);
        }

        [TestMethod]
        public void ItLogsMessage()
        {
            // Arrange
            const string message = "Hi";

            _logger.Setup(x => x.LogInformation(It.IsAny<string>()))
                .Verifiable();

            // Act
            _service.LogMessage(message);

            // Assert
            _logger.Verify();
            //_logger.Verify(x => x.LogInformation(message, It.IsAny<object[]>()), Times.Once);
        }

        public class MockLogger<T>: NullLogger<T>
        {
            public virtual void LogInformation(string message, params object[] args)
            {
            }
        }

        // private class FakeLogger<T> : ILogger<T>
        // {
        //     public static Exception ProvidedException { get; set; }
        //     public static string ProvidedMessage { get; set; }
        //     public static object[] ProvidedArgs { get; set; }
        //     public IDisposable BeginScope<TState>(TState state)
        //     {
        //         return null;
        //     }

        //     public bool IsEnabled(LogLevel logLevel)
        //     {
        //         return true;
        //     }

        //     public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        //     {
        //     }

        //     public virtual void LogError(Exception ex, string message, params object[] args)
        //     {
        //         ProvidedException = ex;
        //         ProvidedMessage = message;
        //         ProvidedArgs = args;
        //     }
        // }
    }
}
