using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ScavengerHunt.Controllers;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.UnitTests
{
    public class AuthControllerTests
    {
        private readonly Mock<IRepositoryService<User>> userRepo = new();
        private readonly Mock<ILogger<AuthController>> logger = new();
        private readonly Mock<ITokenService> tokenService = new();
        private readonly Mock<IHelperService> helpMethod = new();
        private readonly Random rand = new();

        [Fact]
        public void Register_WithEmailAlreadyExist_ReturnsBadRequest()
        {
            //Arrange
            RegisterDto createdItem = new()
            {
                Name = Guid.NewGuid().ToString(),
                Email = "jerishbradlyb@gmail.com",
                Password = Guid.NewGuid().ToString()   
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync(new User());
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);
            
            //Act
            var result = ac.Register(createdItem);
            //Assert
            result.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public void Register_WithValidModel_ReturnsBadRequest()
        {
            //Arrange
            RegisterDto creatItem = new(){
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);
            //Act
            var result = ac.Register(creatItem);
            //Assert
            var createdItem = ((CreatedAtActionResult)result.Result).Value;
            createdItem.Should().BeEquivalentTo
            (
                creatItem,
                options => options.ComparingByMembers<RegisterDto>().ExcludingMissingMembers()
            );
        }

        [Fact]
        public void Login_WithInvalidUser_ReturnsNotFound()
        {
            //Arrange
            LoginDto loginItem = new()
            {
                Email = "jerishbradlyb@gmail.com",
                Password = Guid.NewGuid().ToString()
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync((User?)null);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);
            
            //Act
            var result = ac.Login(loginItem);
            //Assert
            result.Result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Login_WithIncorrectPassword_ReturnsBadRequest()
        {
            //Arrange
            LoginDto loginItem = new()
            {
                Email = "jerishbradlyb@gmail.com",
                Password = Guid.NewGuid().ToString()
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync(new User());
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);

            //Act
            var result = ac.Login(loginItem);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Login_WithDatabaseError_ReturnsBadGatewayError()
        {
            //Arrange
            LoginDto loginItem = new()
            {
                Email = "jerishbradlyb@gmail.com",
                Password = Guid.NewGuid().ToString()
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync(new User());
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);

            //Act
            var result = ac.Login(loginItem);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}