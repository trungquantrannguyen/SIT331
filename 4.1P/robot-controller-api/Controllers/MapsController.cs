using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    [HttpGet]
    public IEnumerable<Map> GetAllMaps()
    {
        return MapADO.GetMaps();
    }

    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return MapADO.GetSquareMapsOnly();
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        var map = MapADO.GetMapById(id);

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
            return BadRequest("Map data is required.");
        }

        if (string.IsNullOrWhiteSpace(newMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (newMap.Rows <= 0 || newMap.Columns <= 0)
        {
            return BadRequest("Rows and columns must be greater than 0.");
        }

        var insertedMap = MapADO.InsertMap(newMap);

        return CreatedAtRoute(
            "GetMap",
            new { id = insertedMap.Id },
            insertedMap
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        if (updatedMap == null)
        {
            return BadRequest("Map data is required.");
        }

        if (string.IsNullOrWhiteSpace(updatedMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (updatedMap.Rows <= 0 || updatedMap.Columns <= 0)
        {
            return BadRequest("Rows and columns must be greater than 0.");
        }

        var existingMap = MapADO.GetMapById(id);

        if (existingMap == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        updatedMap.Id = id;
        MapADO.UpdateMap(updatedMap);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        var existingMap = MapADO.GetMapById(id);

        if (existingMap == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        MapADO.DeleteMap(id);

        return NoContent();
    }

    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        var map = MapADO.GetMapById(id);

        if (map == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        var isOnMap = x >= 0 && x < map.Columns && y >= 0 && y < map.Rows;

        return Ok(isOnMap);
    }
}