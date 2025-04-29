using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapController : ControllerBase
{
    public static readonly List<Map> _maps = new List<Map> { };

    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        // Return all maps
        return MapDataAccess.GetMaps();
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMap(int id)
    {
        Map map = MapDataAccess.GetMapById(id);
        if (map == null)
        {
            return NotFound();
        }
        return Ok(map);
    }

    [HttpPost()]
    public IActionResult CreateMap(Map newMap)
    {
        try
        {
            if (newMap == null)
            {
                return BadRequest();
            }
            if (newMap.Columns <= 0 || newMap.Rows <= 0)
            {
                return BadRequest("Columns and Rows must be greater than zero.");
            }
            MapDataAccess.InsertMap(newMap.Name, newMap.Description, newMap.Columns, newMap.Rows);
            return CreatedAtRoute("GetMap", new { id = newMap.Id }, newMap);
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        try
        {
            if (updatedMap == null)
            {
                return BadRequest();
            }
            MapDataAccess.UpdateMap(id, updatedMap.Name, updatedMap.Description, updatedMap.Columns, updatedMap.Rows);
            return NoContent();
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        try
        {
            MapDataAccess.DeleteMap(id);
            return NoContent();
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}