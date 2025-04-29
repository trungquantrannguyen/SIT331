using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandRepo;
    public RobotCommandsController(IRobotCommandDataAccess robotCommandRepo)
    {
        _robotCommandRepo = robotCommandRepo;
    }

    // Robot commands endpoints here

    /// <summary>
    /// Get all robot commands.
    /// </summary>
    /// <param name= "getCommand">Get all commands</param>
    /// <returns>All commands in the system</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// GET /api/robot-commands
    ///
    /// </remarks>
    /// <response code="200">Returns all the commands in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        // Return all robot commands
        return _robotCommandRepo.GetRobotCommands();
    }

    /// <summary>
    /// Get a robot command by id.
    /// </summary>
    /// <param name= "getCommandById">Get a command id</param>
    /// <returns>A robot command match with id</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// GET /api/robot-commands/id
    ///
    /// </remarks>
    /// <response code="200">Returns all the commands in the system</response>
    /// <response code="404">Returns not found when the command's id is not in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetRobotCommad")]
    public IActionResult GetRobotCommand(int id)
    {
        RobotCommand command = _robotCommandRepo.GetRobotCommandById(id);
        if (command == null)
        {
            return NotFound();
        }
        return Ok(command);
    }

    /// <summary>
    /// Creates a robot command.
    /// </summary>
    /// <param name="newCommand">A new robot command from the HTTP request.</param>
    /// <returns>A newly created robot command</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// POST /api/robot-commands
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost()]
    public IActionResult CreateRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
        {
            return BadRequest();
        }
        RobotCommand createdRobotCommand = _robotCommandRepo.InsertRobotCommand(newCommand.Name, newCommand.Description, newCommand.IsMoveCommand);
        if (createdRobotCommand == null)
        {
            return BadRequest("Error creating command");
        }
        return CreatedAtRoute("GetRobotCommand", new { id = createdRobotCommand.Id }, createdRobotCommand);
    }

    /// <summary>
    /// Update a robot command.
    /// </summary>
    /// <param name= "updateCommand">Update a robot command</param>
    /// <returns>no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// PUT /api/robot-commands/id
    ///
    /// </remarks>
    /// <response code="204">Returns no content</response>
    /// <response code="404">Returns not found when the command's id is not in the system</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        try
        {
            _robotCommandRepo.UpdateRobotCommand(id, updatedCommand.Name, updatedCommand.Description, updatedCommand.IsMoveCommand);

            return NoContent();
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Delete a robot command.
    /// </summary>
    /// <param name= "deleteRobotCommand">Delete a robot command</param>
    /// <returns>no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// DELETE /api/robot-commands/id
    ///
    /// </remarks>
    /// <response code="204">Returns no content</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        _robotCommandRepo.DeleteRobotCommand(id);
        return NoContent();
    }
}
