using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private static readonly List<Map> _maps = new List<Map>
    {
        // maps here
    };

    // Endpoints here
}
