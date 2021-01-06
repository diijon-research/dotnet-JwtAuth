using System;
using JwtAuth.LoginService.Modules.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JwtAuth.LoginService.Modules.Services;

namespace JwtAuth.LoginService.Test.Unit.ModulesTests.ServicesTests.TokenServiceTests
{
    [TestClass]
    public class GenerateToken
    {
        private TokenService _service;

        [TestInitialize]
        public void Initialize()
        {
            _service = new TokenService();
        }

        [TestMethod]
        public void ItDoesNotEqualNull()
        {
            // Arrange

            // Act
            var actual = _service.GenerateToken(new User {Username = "me@gmail.com"});

            // Assert
            Assert.IsNotNull(actual);
        }
    }
}
