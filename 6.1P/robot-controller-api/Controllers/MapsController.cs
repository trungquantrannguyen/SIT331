using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Persistence;
using robot_controller_api.Models;
using Microsoft.AspNetCore.Authorization;

namespace robot_controller_api.Controllers;

/// <summary>
/// Provides API endpoints for managing maps used by the robot simulator.
/// </summary>
[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private readonly IMapDataAccess _mapsRepo;

    /// <summary>
    /// Initialises a new instance of the <see cref="MapsController"/> class.
    /// </summary>
    /// <param name="mapsRepo">The map data access implementation.</param>
    public MapsController(IMapDataAccess mapsRepo)
    {
        _mapsRepo = mapsRepo;
    }

    /// <summary>
    /// Gets all maps.
    /// </summary>
    /// <returns>A list of all maps stored in the backend.</returns>
    /// <response code="200">Returns the list of maps.</response>
    [HttpGet]
    [Authorize(Policy = "UserOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAllMaps()
    {
        var maps = _mapsRepo.GetMaps();
        return Ok(maps);
    }

    /// <summary>
    /// Gets all square maps.
    /// </summary>
    /// <returns>A list of maps where the number of rows and columns are equal.</returns>
    /// <response code="200">Returns the list of square maps.</response>
    [HttpGet("square")]
    [Authorize(Policy = "UserOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetSquareMaps()
    {
        var squareMaps = _mapsRepo.GetSquareMaps();
        return Ok(squareMaps);
    }

    /// <summary>
    /// Gets a map by its ID.
    /// </summary>
    /// <param name="id">The unique ID of the map.</param>
    /// <returns>The map with the matching ID.</returns>
    /// <response code="200">Returns the matching map.</response>
    /// <response code="404">If no map exists with the supplied ID.</response>
    [HttpGet("{id}", Name = "GetMap")]
    [Authorize(Policy = "UserOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetMapById(int id)
    {
        var map = _mapsRepo.GetMapById(id);

        if (map == null)
        {
            return NotFound($"Map with id {id} was not found.");
        }

        return Ok(map);
    }

    /// <summary>
    /// Creates a new map.
    /// </summary>
    /// <param name="newMap">The map details from the HTTP request body.</param>
    /// <returns>The newly created map.</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// POST /api/maps
    /// {
    ///   "name": "Moon Base Map",
    ///   "description": "A square map for robot movement testing",
    ///   "rows": 5,
    ///   "columns": 5
    /// }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created map.</response>
    /// <response code="400">If the request body is null, the name is missing, or rows/columns are invalid.</response>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The ID of the map to update.</param>
    /// <param name="updatedMap">The updated map details from the HTTP request body.</param>
    /// <returns>The updated map.</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// PUT /api/maps/1
    /// {
    ///   "name": "Updated Moon Base Map",
    ///   "description": "Updated map size for robot testing",
    ///   "rows": 6,
    ///   "columns": 6
    /// }
    ///
    /// </remarks>
    /// <response code="200">Returns the updated map.</response>
    /// <response code="400">If the request body is null, the name is missing, or rows/columns are invalid.</response>
    /// <response code="404">If no map exists with the supplied ID.</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Checks whether a coordinate is located on a specific map.
    /// </summary>
    /// <param name="id">The ID of the map to check.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>True if the coordinate is on the map; otherwise, false.</returns>
    /// <remarks>
    /// Sample request:
    ///
    /// GET /api/maps/1/2-3
    ///
    /// </remarks>
    /// <response code="200">Returns true or false depending on whether the coordinate is on the map.</response>
    /// <response code="404">If no map exists with the supplied ID.</response>
    [HttpGet("{id}/{x}-{y}")]
    [Authorize(Policy = "UserOnly")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Deletes a map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map to delete.</param>
    /// <returns>No content if the map is deleted successfully.</returns>
    /// <response code="204">If the map is deleted successfully.</response>
    /// <response code="404">If no map exists with the supplied ID.</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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