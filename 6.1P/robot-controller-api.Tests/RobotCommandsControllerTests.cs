using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using robot_controller_api.Controllers;
using robot_controller_api.Models;
using robot_controller_api.Persistence;
using Xunit;

namespace robot_controller_api.Tests;

public class RobotCommandsControllerTests
{
    private readonly Mock<IRobotCommandDataAccess> _mockRepo;
    private readonly RobotCommandsController _controller;

    public RobotCommandsControllerTests()
    {
        _mockRepo = new Mock<IRobotCommandDataAccess>();
        _controller = new RobotCommandsController(_mockRepo.Object);
    }

    [Fact]
    public void GetAllRobotCommands_ReturnsOkWithCommands()
    {
        var commands = new List<RobotCommand>
        {
            new RobotCommand
            {
                Id = 1,
                Name = "MOVE_FORWARD",
                Description = "Moves forward",
                IsMoveCommand = true,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            }
        };

        _mockRepo.Setup(repo => repo.GetRobotCommands()).Returns(commands);

        var result = _controller.GetAllRobotCommands();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCommands = Assert.IsType<List<RobotCommand>>(okResult.Value);
        Assert.Single(returnedCommands);
        Assert.Equal("MOVE_FORWARD", returnedCommands[0].Name);
    }

    [Fact]
    public void GetRobotCommandById_WhenCommandExists_ReturnsOk()
    {
        var command = new RobotCommand
        {
            Id = 1,
            Name = "TURN_LEFT",
            Description = "Turns left",
            IsMoveCommand = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _mockRepo.Setup(repo => repo.GetRobotCommandById(1)).Returns(command);

        var result = _controller.GetRobotCommandById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCommand = Assert.IsType<RobotCommand>(okResult.Value);
        Assert.Equal(1, returnedCommand.Id);
        Assert.Equal("TURN_LEFT", returnedCommand.Name);
    }

    [Fact]
    public void GetRobotCommandById_WhenCommandDoesNotExist_ReturnsNotFound()
    {
        _mockRepo.Setup(repo => repo.GetRobotCommandById(999)).Returns((RobotCommand?)null);

        var result = _controller.GetRobotCommandById(999);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("999", notFoundResult.Value?.ToString());
    }

    [Fact]
    public void AddRobotCommand_WithValidCommand_ReturnsCreatedAtRoute()
    {
        var newCommand = new RobotCommand
        {
            Name = "MOVE_BACKWARD",
            Description = "Moves backward",
            IsMoveCommand = true
        };

        var addedCommand = new RobotCommand
        {
            Id = 10,
            Name = "MOVE_BACKWARD",
            Description = "Moves backward",
            IsMoveCommand = true,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };

        _mockRepo.Setup(repo => repo.AddRobotCommand(newCommand)).Returns(addedCommand);

        var result = _controller.AddRobotCommand(newCommand);

        var createdResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal("GetRobotCommand", createdResult.RouteName);

        var returnedCommand = Assert.IsType<RobotCommand>(createdResult.Value);
        Assert.Equal(10, returnedCommand.Id);
    }

    [Fact]
    public void AddRobotCommand_WithEmptyName_ReturnsBadRequest()
    {
        var invalidCommand = new RobotCommand
        {
            Name = "",
            Description = "Invalid command",
            IsMoveCommand = true
        };

        var result = _controller.AddRobotCommand(invalidCommand);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("name is required", badRequestResult.Value?.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DeleteRobotCommand_WhenCommandExists_ReturnsNoContent()
    {
        _mockRepo.Setup(repo => repo.DeleteRobotCommand(1)).Returns(true);

        var result = _controller.DeleteRobotCommand(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteRobotCommand_WhenCommandDoesNotExist_ReturnsNotFound()
    {
        _mockRepo.Setup(repo => repo.DeleteRobotCommand(999)).Returns(false);

        var result = _controller.DeleteRobotCommand(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public void GetAllRobotCommands_HasUserOnlyAuthorizePolicy()
    {
        var method = typeof(RobotCommandsController).GetMethod(nameof(RobotCommandsController.GetAllRobotCommands));

        var authorizeAttribute = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("UserOnly", authorizeAttribute.Policy);
    }

    [Fact]
    public void AddRobotCommand_HasAdminOnlyAuthorizePolicy()
    {
        var method = typeof(RobotCommandsController).GetMethod(nameof(RobotCommandsController.AddRobotCommand));

        var authorizeAttribute = method!
            .GetCustomAttributes(typeof(AuthorizeAttribute), false)
            .Cast<AuthorizeAttribute>()
            .FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("AdminOnly", authorizeAttribute.Policy);
    }
}