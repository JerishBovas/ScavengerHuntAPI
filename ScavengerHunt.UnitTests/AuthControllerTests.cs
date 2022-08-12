using System.Security.Claims;
using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        private readonly Mock<IBlobService> blobService = new();
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
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);
            
            //Act
            var result = ac.Register(createdItem);
            //Assert
            result.Result.Should().BeOfType<ConflictObjectResult>();
        }

        [Fact]
        public void Register_WithValidModel_ReturnsTokenObject()
        {
            //Arrange
            RegisterDto creatItem = new(){
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);
            //Act
            var result = ac.Register(creatItem);
            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult) result.Result).Value.Should().BeEquivalentTo
            (
                new AuthResponseDto()
                {
                    AccessToken = It.IsAny<string>(),
                    RefreshToken = It.IsAny<string>()
                }
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
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);
            
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
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Login(loginItem);

            //Assert
            result.Result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Login_WithValidDetails_ReturnsTokenObject()
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
                id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Email = loginItem.Email,
                Role = "user",
                PasswordHash = hash,
                PasswordSalt = salt,
                RefToken = null,
                RefTokenExpiry = null,
                UserLog = new(),
                Games = new HashSet<Guid>(),
                Teams = new HashSet<Guid>(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            helpMethod.Setup(x => x.GetUserFromEmail(It.IsAny<string>())).ReturnsAsync(user);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Login(loginItem).Result;

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeEquivalentTo
            (
                new AuthResponseDto()
                {
                    AccessToken = It.IsAny<string>(),
                    RefreshToken = It.IsAny<string>()
                }
            );
        }

        [Fact]
        public void Refresh_WithUserNull_ReturnsBadRequest()
        {
            //Arrange
            AuthResponseDto authRes = new()
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            tokenService.Setup(x => x.GetPrincipalFromExpiredToken(authRes.AccessToken)).Returns(claimsPrincipal);
            userRepo.Setup(x => x.GetAsync(Guid.NewGuid())).ReturnsAsync((User?)null);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Refresh(authRes);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult) result.Result).Value.Should().BeEquivalentTo
            (
                new CustomError("Session Expired", 400, new string[]{"Session expired. Please login again."})
            );
        }

        [Fact]
        public void Refresh_WithInvalidRefreshToken_ReturnsBadRequest()
        {
            //Arrange
            AuthResponseDto authRes = new()
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            User testUser = new()
            {
                id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = "User",
                ProfileImage = "",
                PasswordHash = Guid.NewGuid().ToString(),
                PasswordSalt = Guid.NewGuid().ToString(),
                RefToken = Guid.NewGuid().ToString(),
                RefTokenExpiry = DateTime.Now.AddHours(1),
                UserLog = new UserLog(),
                Games = new List<Guid>(),
                Teams = new List<Guid>(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            userRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Refresh(authRes);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult) result.Result).Value.Should().BeEquivalentTo
            (
                new CustomError("Session Expired", 400, new string[]{"Session expired. Please login again."})
            );
        }

        [Fact]
        public void Refresh_WithRefreshTokenExpired_ReturnsBadRequest()
        {
            //Arrange
            AuthResponseDto authRes = new()
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            User testUser = new()
            {
                id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = "User",
                ProfileImage = "",
                PasswordHash = Guid.NewGuid().ToString(),
                PasswordSalt = Guid.NewGuid().ToString(),
                RefToken = authRes.RefreshToken,
                RefTokenExpiry = DateTime.Now,
                UserLog = new UserLog(),
                Games = new List<Guid>(),
                Teams = new List<Guid>(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            userRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Refresh(authRes);

            //Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult) result.Result).Value.Should().BeEquivalentTo
            (
                new CustomError("Session Expired", 400, new string[]{"Session expired. Please login again."})
            );
        }

        [Fact]
        public void Refresh_WithValid_ReturnsOkObjectResult()
        {
            //Arrange
            AuthResponseDto authRes = new()
            {
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString()
            };
            User testUser = new()
            {
                id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Role = "User",
                ProfileImage = "",
                PasswordHash = Guid.NewGuid().ToString(),
                PasswordSalt = Guid.NewGuid().ToString(),
                RefToken = authRes.RefreshToken,
                RefTokenExpiry = DateTime.Now.AddHours(1),
                UserLog = new UserLog(),
                Games = new List<Guid>(),
                Teams = new List<Guid>(),
                CreatedDate = DateTimeOffset.UtcNow
            };
            
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.Name, "username"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            tokenService.Setup(x => x.GetPrincipalFromExpiredToken(It.IsAny<string>())).Returns(claimsPrincipal);
            userRepo.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Refresh(authRes);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult) result.Result).Value.Should().BeEquivalentTo
            (
                new AuthResponseDto()
                {
                    AccessToken = It.IsAny<string>(),
                    RefreshToken = It.IsAny<string>()
                }
            );
        }

        [Fact]
        public void Revoke_WithUserNull_ReturnsLoginError()
        {
            //Arrange
            helpMethod.Setup(x => x.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync((User?)null);
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Revoke();

            //Assert
            result.Result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void Revoke_WithValidUser_ReturnsLoginError()
        {
            //Arrange
            helpMethod.Setup(x => x.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync(new User());
            AuthController ac = new(tokenService.Object, userRepo.Object, logger.Object, helpMethod.Object, blobService.Object);

            //Act
            var result = ac.Revoke();

            //Assert
            result.Result.Should().BeOfType<OkResult>();
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