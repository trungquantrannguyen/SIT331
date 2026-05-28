using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo;

    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
    {
        _robotCommandsRepo = robotCommandsRepo;
    }

    [HttpGet]
    public IActionResult GetAllRobotCommands()
    {
        var commands = _robotCommandsRepo.GetRobotCommands();
        return Ok(commands);
    }

    [HttpGet("move")]
    public IActionResult GetMoveCommandsOnly()
    {
        var moveCommands = _robotCommandsRepo.GetMoveCommandsOnly();
        return Ok(moveCommands);
    }

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _robotCommandsRepo.GetRobotCommandById(id);

        if (command == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        return Ok(command);
    }

    [HttpPost]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest("Robot command cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(newCommand.Name))
        {
            return BadRequest("Robot command name is required.");
        }

        var addedCommand = _robotCommandsRepo.AddRobotCommand(newCommand);

        return CreatedAtRoute(
            "GetRobotCommand",
            new { id = addedCommand.Id },
            addedCommand
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (updatedCommand == null)
        {
            return BadRequest("Robot command cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(updatedCommand.Name))
        {
            return BadRequest("Robot command name is required.");
        }

        var command = _robotCommandsRepo.UpdateRobotCommand(id, updatedCommand);

        if (command == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        return Ok(command);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var deleted = _robotCommandsRepo.DeleteRobotCommand(id);

        if (!deleted)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        return NoContent();
    }
}