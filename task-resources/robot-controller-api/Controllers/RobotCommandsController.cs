using Microsoft.AspNetCore.Mvc;
using System.Web.

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private static readonly List<RobotCommand> _commands = new List<RobotCommand>
    {
        // commands here
        "LEFT",
        "RIGHT",
        "MOVE",
        "REPORT",
        "PLACE",
    };

    // Robot commands endpoints here

    [HTTPGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return _commands;
    }
}
