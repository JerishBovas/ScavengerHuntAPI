using System;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ScavengerHunt.Controllers;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.UnitTests;

public class HomeControllerTests
{
	private readonly Mock<IRepositoryService<User>> userRepo = new();
    private readonly Mock<ILogger<HomeController>> logger = new();
	private readonly Mock<IHelperService> helpMethod = new();
	private readonly Random rand = new();

	[Fact]
	public async void GetInfo_withInvalidUser_ReturnsNotFound()
	{
		//Arrange
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync((User?)null);
		HomeController hc = new(userRepo.Object, logger.Object, helpMethod.Object);

		//Act
		var result = await hc.GetInfo();

		//Assert
		result.Result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async void GetInfo_withValidUser_ReturnsExpectedItem()
	{
		//Arrange
		User expectedUser = CreateRandomUser();
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync(expectedUser);
		var hc = new HomeController(userRepo.Object, logger.Object, helpMethod.Object);

		//Act
		var result = await hc.GetInfo();

		//Assert
		result.Value.Should().BeEquivalentTo(
			expectedUser,
			options => options.ComparingByMembers<User>().ExcludingMissingMembers()
		);
	}

	[Fact]
	public async void GetScoreLog_withInvalidUser_ReturnsNotFound()
	{
		//Arrange
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync((User?)null);
		HomeController hc = new(userRepo.Object, logger.Object, helpMethod.Object);

		//Act
		var result = await hc.GetScoreLog();

		//Assert
		result.Result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async void GetScoreLog_withValidUser_ReturnsExpectedItem()
	{
		//Arrange
		User expectedUser = CreateRandomUser();
		expectedUser.UserLog.ScoreLog = new [] { CreateRandomScoreLog(), CreateRandomScoreLog(), 
		CreateRandomScoreLog(), CreateRandomScoreLog(), CreateRandomScoreLog()};
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync(expectedUser);
		var hc = new HomeController(userRepo.Object, logger.Object, helpMethod.Object);

		//Act
		var result = await hc.GetScoreLog();

		//Assert
		result.Should().BeOfType<ActionResult<List<ScoreLogDto>>>();
		result.Value.Should().BeEquivalentTo
		(
			expectedUser.UserLog.ScoreLog,
			options => options.ComparingByMembers<ScoreLog>()
		);
	}

	private User CreateRandomUser()
	{
		return new()
		{
			Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Email = Guid.NewGuid().ToString(),
            Role = "user",
            PasswordHash = Guid.NewGuid().ToString(),
            PasswordSalt = Guid.NewGuid().ToString(),
            UserLog = new()
			{
				UserScore = 0,
				LastUpdated = DateTimeOffset.UtcNow,
				ScoreLog = new List<ScoreLog>()
			},
            Locations = new HashSet<Guid>(),
            Groups = new HashSet<Guid>(),
            CreatedDate = DateTimeOffset.UtcNow
		};
	}
	private ScoreLog CreateRandomScoreLog()
	{
		return new(
			DateTimeOffset.UtcNow,
            Guid.NewGuid().ToString(),
            rand.Next(1000)
		);
	}
}