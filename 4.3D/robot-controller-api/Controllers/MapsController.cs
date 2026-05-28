using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private readonly IMapDataAccess _mapsRepo;

    public MapsController(IMapDataAccess mapsRepo)
    {
        _mapsRepo = mapsRepo;
    }

    [HttpGet]
    public IActionResult GetAllMaps()
    {
        var maps = _mapsRepo.GetMaps();
        return Ok(maps);
    }

    [HttpGet("square")]
    public IActionResult GetSquareMaps()
    {
        var squareMaps = _mapsRepo.GetSquareMaps();
        return Ok(squareMaps);
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        var map = _mapsRepo.GetMapById(id);

        if (map == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        return Ok(map);
    }

    [HttpPost]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
        {
            return BadRequest("Map cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(newMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (newMap.Columns <= 0 || newMap.Rows <= 0)
        {
            return BadRequest("Map columns and rows must be greater than 0.");
        }

        var addedMap = _mapsRepo.AddMap(newMap);

        return CreatedAtRoute(
            "GetMap",
            new { id = addedMap.Id },
            addedMap
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        if (updatedMap == null)
        {
            return BadRequest("Map cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(updatedMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (updatedMap.Columns <= 0 || updatedMap.Rows <= 0)
        {
            return BadRequest("Map columns and rows must be greater than 0.");
        }

        var map = _mapsRepo.UpdateMap(id, updatedMap);

        if (map == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        return Ok(map);
    }

    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        var map = _mapsRepo.GetMapById(id);

        if (map == null)
        {
            return NotFound($"Map with ID {id} does not exist.");
        }

        var isOnMap = _mapsRepo.IsCoordinateOnMap(id, x, y);

        return Ok(isOnMap);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        var deleted = _mapsRepo.DeleteMap(id);

        if (!deleted)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        return NoContent();
    }
}