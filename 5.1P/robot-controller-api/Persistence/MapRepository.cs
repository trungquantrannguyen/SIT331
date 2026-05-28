using robot_controller_api.Persistence;

namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess
{
    private readonly RobotContext _context;

    public MapRepository(RobotContext context)
    {
        _context = context;
    }

    public List<Map> GetMaps()
    {
        return _context.Maps
            .Select(map => new Map
            {
                Id = map.Id,
                Name = map.Name,
                Rows = map.Rows,
                Columns = map.Columns,
                Description = map.Description,
                CreatedDate = map.CreatedDate,
                ModifiedDate = map.ModifiedDate
            })
            .ToList();
    }

    public List<Map> GetSquareMaps()
    {
        return _context.Maps
            .Where(map => map.IsSquare == true)
            .Select(map => new Map
            {
                Id = map.Id,
                Name = map.Name,
                Rows = map.Rows,
                Columns = map.Columns,
                Description = map.Description,
                CreatedDate = map.CreatedDate,
                ModifiedDate = map.ModifiedDate
            })
            .ToList();
    }

    public Map? GetMapById(int id)
    {
        var map = _context.Maps.FirstOrDefault(m => m.Id == id);

        if (map == null)
        {
            return null;
        }

        return new Map
        {
            Id = map.Id,
            Name = map.Name,
            Rows = map.Rows,
            Columns = map.Columns,
            Description = map.Description,
            CreatedDate = map.CreatedDate,
            ModifiedDate = map.ModifiedDate
        };
    }

    public Map AddMap(Map map)
    {
        var entity = new MapEF
        {
            Name = map.Name,
            Rows = map.Rows,
            Columns = map.Columns,
            Description = map.Description,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        _context.Maps.Add(entity);
        _context.SaveChanges();

        map.Id = entity.Id;
        map.CreatedDate = entity.CreatedDate;
        map.ModifiedDate = entity.ModifiedDate;

        return map;
    }

    public Map? UpdateMap(int id, Map map)
    {
        var entity = _context.Maps.FirstOrDefault(m => m.Id == id);

        if (entity == null)
        {
            return null;
        }

        entity.Name = map.Name;
        entity.Rows = map.Rows;
        entity.Columns = map.Columns;
        entity.Description = map.Description;
        entity.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        map.Id = entity.Id;
        map.CreatedDate = entity.CreatedDate;
        map.ModifiedDate = entity.ModifiedDate;

        return map;
    }

    public bool DeleteMap(int id)
    {
        var entity = _context.Maps.FirstOrDefault(m => m.Id == id);

        if (entity == null)
        {
            return false;
        }

        _context.Maps.Remove(entity);
        _context.SaveChanges();

        return true;
    }

    public bool IsCoordinateOnMap(int mapId, int x, int y)
    {
        var map = _context.Maps.FirstOrDefault(m => m.Id == mapId);

        if (map == null)
        {
            return false;
        }

        return x >= 0 && y >= 0 && x < map.Columns && y < map.Rows;
    }
}