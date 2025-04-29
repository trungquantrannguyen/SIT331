using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private static readonly List<RobotCommand> _commands = new List<RobotCommand>
    {
        new (1,"LEFT",false,DateTime.Now,DateTime.Now,"Turn left"),
        new (2,"RIGHT",false,DateTime.Now,DateTime.Now,"Turn right"),
        new (3,"MOVE",true,DateTime.Now,DateTime.Now,"Move"),
        new (4,"PLACE",false,DateTime.Now,DateTime.Now,"Place"),
        new (5,"REPORT",false,DateTime.Now,DateTime.Now,"Report"),
    };

    // Robot commands endpoints here

    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        // Return all robot commands
        return RobotCommandDataAccess.GetRobotCommands();
    }

    [HttpGet("{id}", Name = "GetRobotCommad")]
    public IActionResult GetRobotCommand(int id)
    {
        RobotCommand command = RobotCommandDataAccess.GetRobotCommandById(id);
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
        RobotCommand createdRobotCommand = RobotCommandDataAccess.InsertRobotCommand(newCommand.Name, newCommand.Description, newCommand.IsMoveCommand);
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
            RobotCommandDataAccess.UpdateRobotCommand(id, updatedCommand.Name, updatedCommand.Description, updatedCommand.IsMoveCommand);

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
        RobotCommandDataAccess.DeleteRobotCommand(id);
        return NoContent();
    }
}
