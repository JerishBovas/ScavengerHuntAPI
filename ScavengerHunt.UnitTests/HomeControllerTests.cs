using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
	private Mock<IHostEnvironment> hostingEnvironment = new();
	private Mock<IConfiguration> configuration = new();
	private readonly Random rand = new();
	HomeController hc;

	public HomeControllerTests()
	{
		hc = new(userRepo.Object, logger.Object, helpMethod.Object, blobService.Object, gameService.Object, hostingEnvironment.Object, configuration.Object);
	}
}