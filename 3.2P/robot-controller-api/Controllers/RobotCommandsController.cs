using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private static readonly List<RobotCommand> _commands = new List<RobotCommand>
    {
        new RobotCommand(
            id: 1,
            name: "LEFT",
            isMoveCommand: true,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "Turns the robot left."
        ),

        new RobotCommand(
            id: 2,
            name: "RIGHT",
            isMoveCommand: true,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "Turns the robot right."
        ),

        new RobotCommand(
            id: 3,
            name: "MOVE",
            isMoveCommand: true,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "Moves the robot forward by one unit."
        ),

        new RobotCommand(
            id: 4,
            name: "PLACE",
            isMoveCommand: false,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "Places the robot on the map."
        ),

        new RobotCommand(
            id: 5,
            name: "REPORT",
            isMoveCommand: false,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "Reports the robot's current position."
        )
    };

    [HttpGet]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return _commands;
    }

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return _commands.Where(command => command.IsMoveCommand);
    }

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        RobotCommand? command = _commands.FirstOrDefault(command => command.Id == id);

        if (command == null)
        {
            return NotFound();
        }

        return Ok(command);
    }

    [HttpPost]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(newCommand.Name))
        {
            return BadRequest("Command name is required.");
        }

        bool commandNameAlreadyExists = _commands.Any(command =>
            command.Name.Equals(newCommand.Name, StringComparison.OrdinalIgnoreCase));

        if (commandNameAlreadyExists)
        {
            return Conflict("A command with the same name already exists.");
        }

        int newId = _commands.Any()
            ? _commands.Max(command => command.Id) + 1
            : 1;

        RobotCommand command = new RobotCommand(
            id: newId,
            name: newCommand.Name.ToUpper(),
            isMoveCommand: newCommand.IsMoveCommand,
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: newCommand.Description
        );

        _commands.Add(command);

        return CreatedAtRoute(
            routeName: "GetRobotCommand",
            routeValues: new { id = command.Id },
            value: command
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        if (updatedCommand == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(updatedCommand.Name))
        {
            return BadRequest("Command name is required.");
        }

        RobotCommand? existingCommand = _commands.FirstOrDefault(command => command.Id == id);

        if (existingCommand == null)
        {
            return NotFound();
        }

        bool anotherCommandWithSameNameExists = _commands.Any(command =>
            command.Id != id &&
            command.Name.Equals(updatedCommand.Name, StringComparison.OrdinalIgnoreCase));

        if (anotherCommandWithSameNameExists)
        {
            return Conflict("Another command with the same name already exists.");
        }

        existingCommand.Name = updatedCommand.Name.ToUpper();
        existingCommand.Description = updatedCommand.Description;
        existingCommand.IsMoveCommand = updatedCommand.IsMoveCommand;
        existingCommand.ModifiedDate = DateTime.Now;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        RobotCommand? command = _commands.FirstOrDefault(command => command.Id == id);

        if (command == null)
        {
            return NotFound();
        }

        _commands.Remove(command);

        return NoContent();
    }
}