using Microsoft.Extensions.Logging;
using Moq;
using ScavengerHunt.Controllers;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.UnitTests
{
    public class AuthControllerTests
    {
        public readonly Mock<IRepositoryService<User>> repositoryStub = new();
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