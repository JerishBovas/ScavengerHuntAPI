using Microsoft.Extensions.Logging;
using Moq;
using ScavengerHunt.API.Controllers;
using ScavengerHunt.API.Services;

namespace ScavengerHunt_API.Tests
{
    public class AuthControllerTests
    {
        public readonly Mock<IUserRepository> repositoryStub = new();
        public readonly Mock<ILogger<AuthController>> loggerStub = new();

        [Fact]
        public void Register_WithInvalidModel_ReturnsBadRequest()
        {
            //Arrange



            //Act

            //Assert
        }
    }
}