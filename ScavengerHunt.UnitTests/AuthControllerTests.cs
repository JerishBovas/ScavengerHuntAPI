using System.Security.Cryptography;
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
        private readonly Mock<IUserService> userRepo = new();
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
        public void Register_WithValidModel_ReturnsRegisteredUser()
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
            result.Result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Login_WithDatabaseError_ReturnsBadGatewayError()
        {
            //Arrange
            LoginDto loginItem = new()
            {
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };
            CreatePassword(loginItem.Password, out string hash, out string salt);
            User user = new()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Email = loginItem.Email,
                Role = "user",
                PasswordHash = hash,
                PasswordSalt = salt,
                RefToken = null,
                RefTokenExpiry = null,
                UserLog = new(),
                Games = new HashSet<Guid>(),
                Groups = new HashSet<Guid>(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync(user);
            userRepo.Setup(X => X.SaveChangesAsync()).ThrowsAsync(new SystemException());
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object);

            //Act
            var result = ac.Login(loginItem).Result;

            //Assert
            result.Result.Should().BeOfType<ObjectResult>();
        }

        private static void CreatePassword(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            byte[] salt = hmac.Key;
            byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            passwordHash = Convert.ToBase64String(hash);
            passwordSalt = Convert.ToBase64String(salt);
        }

        private static bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] passwordHash = Convert.FromBase64String(hash);
            byte[] passwordSalt = Convert.FromBase64String(salt);

            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}