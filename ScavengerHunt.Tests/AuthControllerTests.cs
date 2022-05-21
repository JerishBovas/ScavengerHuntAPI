using Microsoft.Extensions.Logging;
using Moq;
using ScavengerHunt.Controllers;
using ScavengerHunt.Services;

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