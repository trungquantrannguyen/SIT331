using Microsoft.AspNetCore.Mvc;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private static readonly List<Map> _maps = new List<Map>
    {
        new Map(
            id: 1,
            columns: 5,
            rows: 5,
            name: "MOON",
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "A small square moon map."
        ),

        new Map(
            id: 2,
            columns: 10,
            rows: 10,
            name: "DEAKIN",
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "A square Deakin map."
        ),

        new Map(
            id: 3,
            columns: 8,
            rows: 6,
            name: "BURWOOD",
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: "A rectangular Burwood map."
        )
    };

    [HttpGet]
    public IEnumerable<Map> GetAllMaps()
    {
        return _maps;
    }

    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return _maps.Where(map => map.Columns == map.Rows);
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        Map? map = _maps.FirstOrDefault(map => map.Id == id);

        if (map == null)
        {
            return NotFound();
        }

        return Ok(map);
    }

    [HttpPost]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(newMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (newMap.Columns <= 0 || newMap.Rows <= 0)
        {
            return BadRequest("Map columns and rows must be greater than zero.");
        }

        bool mapNameAlreadyExists = _maps.Any(map =>
            map.Name.Equals(newMap.Name, StringComparison.OrdinalIgnoreCase));

        if (mapNameAlreadyExists)
        {
            return Conflict("A map with the same name already exists.");
        }

        int newId = _maps.Any()
            ? _maps.Max(map => map.Id) + 1
            : 1;

        Map map = new Map(
            id: newId,
            columns: newMap.Columns,
            rows: newMap.Rows,
            name: newMap.Name.ToUpper(),
            createdDate: DateTime.Now,
            modifiedDate: DateTime.Now,
            description: newMap.Description
        );

        _maps.Add(map);

        return CreatedAtRoute(
            routeName: "GetMap",
            routeValues: new { id = map.Id },
            value: map
        );
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        if (updatedMap == null)
        {
            return BadRequest();
        }

        if (string.IsNullOrWhiteSpace(updatedMap.Name))
        {
            return BadRequest("Map name is required.");
        }

        if (updatedMap.Columns <= 0 || updatedMap.Rows <= 0)
        {
            return BadRequest("Map columns and rows must be greater than zero.");
        }

        Map? existingMap = _maps.FirstOrDefault(map => map.Id == id);

        if (existingMap == null)
        {
            return NotFound();
        }

        bool anotherMapWithSameNameExists = _maps.Any(map =>
            map.Id != id &&
            map.Name.Equals(updatedMap.Name, StringComparison.OrdinalIgnoreCase));

        if (anotherMapWithSameNameExists)
        {
            return Conflict("Another map with the same name already exists.");
        }

        existingMap.Name = updatedMap.Name.ToUpper();
        existingMap.Columns = updatedMap.Columns;
        existingMap.Rows = updatedMap.Rows;
        existingMap.Description = updatedMap.Description;
        existingMap.ModifiedDate = DateTime.Now;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        Map? map = _maps.FirstOrDefault(map => map.Id == id);

        if (map == null)
        {
            return NotFound();
        }

        _maps.Remove(map);

        return NoContent();
    }

    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return BadRequest("Coordinate values cannot be negative.");
        }

        Map? map = _maps.FirstOrDefault(map => map.Id == id);

        if (map == null)
        {
            return NotFound();
        }

        bool isOnMap = x < map.Columns && y < map.Rows;

        return Ok(isOnMap);
    }
}