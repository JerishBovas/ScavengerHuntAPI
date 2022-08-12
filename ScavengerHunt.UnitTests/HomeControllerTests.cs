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
	private readonly Mock<IUserService> userRepo = new();
    private readonly Mock<ILogger<HomeController>> logger = new();
	private readonly Mock<IHelperService> helpMethod = new();
	private readonly Mock<IBlobService> blobService = new();
	private readonly Mock<IGameService> gameService = new();
	private readonly Random rand = new();
	HomeController hc;

	public HomeControllerTests()
	{
		hc = new(userRepo.Object, logger.Object, helpMethod.Object, blobService.Object, gameService.Object);
	}

	[Fact]
	public async void GetInfo_withInvalidUser_ReturnsNotFound()
	{
		//Arrange
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync((User?)null);
		//Act
		var result = await hc.GetUser();

		//Assert
		result.Result.Should().BeOfType<NotFoundObjectResult>();
	}

	[Fact]
	public async void GetInfo_withValidUser_ReturnsExpectedItem()
	{
		//Arrange
		User expectedUser = CreateRandomUser();
		helpMethod.Setup(method => method.GetCurrentUser(It.IsAny<HttpContext>())).ReturnsAsync(expectedUser);

		//Act
		var result = await hc.GetUser();

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

		//Act
		var result = await hc.GetScoreLog();

		//Assert
		result.Should().BeOfType<ActionResult<List<GameScoreDto>>>();
		result.Value.Should().BeEquivalentTo
		(
			expectedUser.UserLog.ScoreLog,
			options => options.ComparingByMembers<GameScore>()
		);
	}

	private User CreateRandomUser()
	{
		return new()
		{
			id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Email = Guid.NewGuid().ToString(),
            Role = "user",
            PasswordHash = Guid.NewGuid().ToString(),
            PasswordSalt = Guid.NewGuid().ToString(),
            UserLog = new()
			{
				UserScore = 0,
				LastUpdated = DateTimeOffset.UtcNow,
				ScoreLog = new List<GameScore>()
			},
            Games = new HashSet<Guid>(),
            Teams = new HashSet<Guid>(),
            CreatedDate = DateTimeOffset.UtcNow
		};
	}
	private GameScore CreateRandomScoreLog()
	{
		return new(
			DateTimeOffset.UtcNow,
            Guid.NewGuid().ToString(),
            rand.Next(1000)
		);
	}
}