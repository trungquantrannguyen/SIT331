using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using robot_controller_api.Controllers;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using Xunit;

namespace robot_controller_api.Tests;

public class MapsControllerTests
{
    private readonly Mock<IMapDataAccess> _mockRepo;
    private readonly MapsController _controller;

    public MapsControllerTests()
    {
        _mockRepo = new Mock<IMapDataAccess>();
        _controller = new MapsController(_mockRepo.Object);
    }

    [Fact]
    public void GetAllMaps_ReturnsOkWithMaps()
    {
        var maps = new List<Map>
        {
            new Map
            {
                Id = 1,
                Name = "Moon Base",
                Description = "Test map",
                Rows = 5,
                Columns = 5,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }
        };

        _mockRepo.Setup(repo => repo.GetMaps()).Returns(maps);

        var result = _controller.GetAllMaps();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedMaps = Assert.IsType<List<Map>>(okResult.Value);
        Assert.Single(returnedMaps);
        Assert.Equal("Moon Base", returnedMaps[0].Name);
    }

    [Fact]
    public void AddMap_WithValidMap_ReturnsCreatedAtRoute()
    {
        var newMap = new Map
        {
            Name = "Mars Test Map",
            Description = "Valid test map",
            Rows = 8,
            Columns = 8
        };

        var addedMap = new Map
        {
            Id = 2,
            Name = "Mars Test Map",
            Description = "Valid test map",
            Rows = 8,
            Columns = 8,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _mockRepo.Setup(repo => repo.AddMap(newMap)).Returns(addedMap);

        var result = _controller.AddMap(newMap);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal("GetMap", createdResult.RouteName);

        var returnedMap = Assert.IsType<Map>(createdResult.Value);
        Assert.Equal(2, returnedMap.Id);
        Assert.Equal(8, returnedMap.Rows);
        Assert.Equal(8, returnedMap.Columns);
    }

    [Fact]
    public void AddMap_WithEmptyName_ReturnsBadRequest()
    {
        var invalidMap = new Map
        {
            Name = "",
            Description = "Invalid map",
            Rows = 5,
            Columns = 5
        };

        var result = _controller.AddMap(invalidMap);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("name is required", badRequestResult.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(5, 0)]
    [InlineData(-1, 5)]
    [InlineData(5, -1)]
    public void AddMap_WithInvalidDimensions_ReturnsBadRequest(int rows, int columns)
    {
        var invalidMap = new Map
        {
            Name = "Invalid Map",
            Description = "Invalid dimensions",
            Rows = rows,
            Columns = columns
        };

        var result = _controller.AddMap(invalidMap);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("greater than 0", badRequestResult.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CheckCoordinate_WhenMapDoesNotExist_ReturnsNotFound()
    {
        _mockRepo.Setup(repo => repo.GetMapById(999)).Returns((Map?)null);

        var result = _controller.CheckCoordinate(999, 1, 1);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("999", notFoundResult.Value?.ToString());
    }

    [Fact]
    public void CheckCoordinate_WhenCoordinateIsOnMap_ReturnsOkTrue()
    {
        var map = new Map
        {
            Id = 1,
            Name = "Moon Base",
            Rows = 5,
            Columns = 5
        };

        _mockRepo.Setup(repo => repo.GetMapById(1)).Returns(map);
        _mockRepo.Setup(repo => repo.IsCoordinateOnMap(1, 2, 3)).Returns(true);

        var result = _controller.CheckCoordinate(1, 2, 3);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }

    [Fact]
    public void CheckCoordinate_WhenCoordinateIsOutsideMap_ReturnsOkFalse()
    {
        var map = new Map
        {
            Id = 1,
            Name = "Moon Base",
            Rows = 5,
            Columns = 5
        };

        _mockRepo.Setup(repo => repo.GetMapById(1)).Returns(map);
        _mockRepo.Setup(repo => repo.IsCoordinateOnMap(1, 10, 10)).Returns(false);

        var result = _controller.CheckCoordinate(1, 10, 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.False((bool)okResult.Value!);
    }

    [Fact]
    public void DeleteMap_WhenMapExists_ReturnsNoContent()
    {
        _mockRepo.Setup(repo => repo.DeleteMap(1)).Returns(true);

        var result = _controller.DeleteMap(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void AddMap_HasAdminOnlyAuthorizePolicy()
    {
        var method = typeof(MapsController).GetMethod(nameof(MapsController.AddMap));

        var authorizeAttribute = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("AdminOnly", authorizeAttribute.Policy);
    }

    [Fact]
    public void GetAllMaps_HasUserOnlyAuthorizePolicy()
    {
        var method = typeof(MapsController).GetMethod(nameof(MapsController.GetAllMaps));

        var authorizeAttribute = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("UserOnly", authorizeAttribute.Policy);
    }
}