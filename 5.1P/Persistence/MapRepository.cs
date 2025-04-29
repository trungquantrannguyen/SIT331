
using Npgsql;

namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
    private IRepository _repo => this;
    public void DeleteMap(int id)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", id)
        };
        _repo.ExecuteReader<Map>("DELETE FROM map WHERE id = @id", sqlParams);
    }

    public Map GetMapById(int id)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", id)
        };
        var result = _repo.ExecuteReader<Map>(
            "SELECT * FROM map WHERE id = @id", sqlParams).SingleOrDefault();
        return result;
    }

    public List<Map> GetMaps()
    {
        var maps = _repo.ExecuteReader<Map>("SELECT * FROM map");
        return maps;
    }

    public Map InsertMap(string name, string? description, int columns, int rows)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("name", name),
            new("description", description ?? (object)DBNull.Value),
            new("columns", columns),
            new("rows", rows)
        };
        var result = _repo.ExecuteReader<Map>(
            "INSERT INTO map (name, description, columns, rows) VALUES (@name, @description, @columns, @rows) RETURNING *;", sqlParams).Single();
        return result;
    }

    public Map UpdateMap(int id, string name, string? description, int columns, int rows)
    {
        var existMap = GetMapById(id);
        if (existMap == null)
        {
            throw new Exception("Map not found");
        }
        var sqlParams = new NpgsqlParameter[]{
            new("id", id),
            new("name", name),
            new("description", description ?? (object)DBNull.Value),
            new("columns", columns),
            new("rows", rows)
        };
        var result = _repo.ExecuteReader<Map>(
            "UPDATE map SET name=@name, description=@description, columns=@columns, rows=@rows WHERE id = @id RETURNING *;", sqlParams).Single();
        return result;
    }
}