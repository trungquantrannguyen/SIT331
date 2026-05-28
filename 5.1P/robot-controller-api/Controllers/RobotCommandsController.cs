using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

/// <summary>
/// Provides API endpoints for managing robot commands.
/// </summary>
[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo;

    /// <summary>
    /// Initialises a new instance of the <see cref="RobotCommandsController"/> class.
    /// </summary>
    /// <param name="robotCommandsRepo">The robot command data access implementation.</param>
    public RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo)
    {
        _robotCommandsRepo = robotCommandsRepo;
    }

    /// <summary>
    /// Gets all robot commands.
    /// </summary>
    /// <returns>A list of all robot commands stored in the backend.</returns>
    /// <response code="200">Returns the list of robot commands.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllRobotCommands()
    {
        var commands = _robotCommandsRepo.GetRobotCommands();
        return Ok(commands);
    }

    /// <summary>
    /// Gets only robot commands that move the robot.
    /// </summary>
    /// <returns>A list of robot commands where IsMoveCommand is true.</returns>
    /// <response code="200">Returns the list of move commands.</response>
    [HttpGet("move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetMoveCommandsOnly()
    {
        var moveCommands = _robotCommandsRepo.GetMoveCommandsOnly();
        return Ok(moveCommands);
    }

    /// <summary>
    /// Gets a robot command by its ID.
    /// </summary>
    /// <param name="id">The unique ID of the robot command.</param>
    /// <returns>The robot command with the matching ID.</returns>
    /// <response code="200">Returns the matching robot command.</response>
    /// <response code="404">If no robot command exists with the supplied ID.</response>
    [HttpGet("{id}", Name = "GetRobotCommand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _robotCommandsRepo.GetRobotCommandById(id);

        if (command == null)
        {
            return NotFound($"Robot command with id {id} was not found.");
        }

        return Ok(command);
    }

    /// <summary>
    /// Creates a new robot command.
    /// </summary>
    /// <param name="newCommand">The robot command details from the HTTP request body.</param>
    /// <returns>The newly created robot command.</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// POST /api/robot-commands
    /// {
    ///   "name": "MOVE_FORWARD",
    ///   "description": "Moves the robot forward on the map",
    ///   "isMoveCommand": true
    /// }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command.</response>
    /// <response code="400">If the request body is null or the command name is missing.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Updates an existing robot command.
    /// </summary>
    /// <param name="id">The ID of the robot command to update.</param>
    /// <param name="updatedCommand">The updated robot command details from the HTTP request body.</param>
    /// <returns>The updated robot command.</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// PUT /api/robot-commands/1
    /// {
    ///   "name": "TURN_LEFT",
    ///   "description": "Turns the robot left",
    ///   "isMoveCommand": false
    /// }
    ///
    /// </remarks>
    /// <response code="200">Returns the updated robot command.</response>
    /// <response code="400">If the request body is null or the command name is missing.</response>
    /// <response code="404">If no robot command exists with the supplied ID.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes a robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to delete.</param>
    /// <returns>No content if the robot command is deleted successfully.</returns>
    /// <response code="204">If the robot command is deleted successfully.</response>
    /// <response code="404">If no robot command exists with the supplied ID.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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