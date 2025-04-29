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

    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        // Return all robot commands
        return _robotCommandRepo.GetRobotCommands();
    }

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

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        _robotCommandRepo.DeleteRobotCommand(id);
        return NoContent();
    }
}
