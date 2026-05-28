using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return RobotCommandADO.GetRobotCommands();
    }

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return RobotCommandADO.GetMoveCommandsOnly();
    }

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var robotCommand = RobotCommandADO.GetRobotCommandById(id);

        if (robotCommand == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        return Ok(robotCommand);
    }

    [HttpPost]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest("Robot command data is required.");
        }

        if (string.IsNullOrWhiteSpace(newCommand.Name))
        {
            return BadRequest("Robot command name is required.");
        }

        var insertedCommand = RobotCommandADO.InsertRobotCommand(newCommand);

        return CreatedAtRoute(
            "GetRobotCommand",
            new { id = insertedCommand.Id },
            insertedCommand
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (updatedCommand == null)
        {
            return BadRequest("Robot command data is required.");
        }

        if (string.IsNullOrWhiteSpace(updatedCommand.Name))
        {
            return BadRequest("Robot command name is required.");
        }

        var existingCommand = RobotCommandADO.GetRobotCommandById(id);

        if (existingCommand == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        updatedCommand.Id = id;
        RobotCommandADO.UpdateRobotCommand(updatedCommand);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var existingCommand = RobotCommandADO.GetRobotCommandById(id);

        if (existingCommand == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        RobotCommandADO.DeleteRobotCommand(id);

        return NoContent();
    }
}