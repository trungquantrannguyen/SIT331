using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapController : ControllerBase
{
    private readonly IMapDataAccess _mapRepo;
    public MapController(IMapDataAccess mapDataAccess)
    {
        _mapRepo = mapDataAccess;
    }
    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        // Return all maps
        return _mapRepo.GetMaps();
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMap(int id)
    {
        Map map = _mapRepo.GetMapById(id);
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
            _mapRepo.InsertMap(newMap.Name, newMap.Description, newMap.Columns, newMap.Rows);
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
            _mapRepo.UpdateMap(id, updatedMap.Name, updatedMap.Description, updatedMap.Columns, updatedMap.Rows);
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
            _mapRepo.DeleteMap(id);
            return NoContent();
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}