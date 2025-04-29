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

    /// <summary>
    /// Get all robot commands.
    /// </summary>
    /// <param name= "getMap">Get all maps</param>
    /// <returns>All maps in the system</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// GET /api/maps
    ///
    /// </remarks>
    /// <response code="200">Returns all maps in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        // Return all maps
        return _mapRepo.GetMaps();
    }

    /// <summary>
    /// Get a robot command by id.
    /// </summary>
    /// <param name= "getMapById">Get a map id</param>
    /// <returns>A map match with id</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// GET /api/maps/id
    ///
    /// </remarks>
    /// <response code="200">Returns all the commands in the system</response>
    /// <response code="404">Returns not found when the command's id is not in the system</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Creates a map.
    /// </summary>
    /// <param name="newMap">A new map from the HTTP request.</param>
    /// <returns>A newly created robot command</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// POST /api/maps
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created robot command</response>
    /// <response code="400">If the robot command is null</response>
    /// <response code="409">If a robot command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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

    /// <summary>
    /// Update a map.
    /// </summary>
    /// <param name= "updateMap">Update a map</param>
    /// <returns>no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// PUT /api/maps/id
    ///
    /// </remarks>
    /// <response code="204">Returns no content</response>
    /// <response code="404">Returns not found when the command's id is not in the system</response>
    /// <response code="404">Returns bad request if there are no body</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Delete a map.
    /// </summary>
    /// <param name= "deleteMap">Delete a map</param>
    /// <returns>no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// DELETE /api/maps/id
    ///
    /// </remarks>
    /// <response code="204">Returns no content</response>
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