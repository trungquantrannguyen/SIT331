using Microsoft.EntityFrameworkCore;

namespace robot_controller_api.Persistence;

public class MapEF : IMapDataAccess
{
    private readonly RobotContext _context;

    public MapEF(RobotContext context)
    {
        _context = context;
    }

    public List<Map> GetMaps()
    {
        return _context.Maps
            .AsNoTracking()
            .OrderBy(map => map.Id)
            .ToList();
    }

    public Map? GetMapById(int id)
    {
        return _context.Maps
            .AsNoTracking()
            .FirstOrDefault(map => map.Id == id);
    }

    public List<Map> GetSquareMaps()
    {
        return _context.Maps
            .AsNoTracking()
            .Where(map => EF.Property<bool>(map, "IsSquare"))
            .OrderBy(map => map.Id)
            .ToList();
    }

    public Map AddMap(Map map)
    {
        map.Id = 0;
        map.CreatedDate = DateTime.Now;
        map.ModifiedDate = DateTime.Now;

        _context.Maps.Add(map);
        _context.SaveChanges();

        return map;
    }

    public Map? UpdateMap(int id, Map map)
    {
        var existingMap = _context.Maps
            .FirstOrDefault(m => m.Id == id);

        if (existingMap == null)
        {
            return null;
        }

        existingMap.Name = map.Name;
        existingMap.Rows = map.Rows;
        existingMap.Columns = map.Columns;
        existingMap.Description = map.Description;
        existingMap.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        return existingMap;
    }

    public bool DeleteMap(int id)
    {
        var existingMap = _context.Maps
            .FirstOrDefault(map => map.Id == id);

        if (existingMap == null)
        {
            return false;
        }

        _context.Maps.Remove(existingMap);
        _context.SaveChanges();

        return true;
    }

    public bool IsCoordinateOnMap(int id, int x, int y)
    {
        var map = _context.Maps
            .AsNoTracking()
            .FirstOrDefault(m => m.Id == id);

        if (map == null)
        {
            return false;
        }

        return x >= 0 && x < map.Columns && y >= 0 && y < map.Rows;
    }
}