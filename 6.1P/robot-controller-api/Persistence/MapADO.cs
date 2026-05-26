using Npgsql;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class MapADO : IMapDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Port=5432;Username=trungquan;Password=0812;Database=sit331";

    public List<Map> GetMaps()
    {
        var maps = new List<Map>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            maps.Add(new Map
            {
                Id = dr.GetInt32(0),
                Columns = dr.GetInt32(1),
                Rows = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            });
        }

        return maps;
    }

    public Map? GetMapById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return new Map
            {
                Id = dr.GetInt32(0),
                Columns = dr.GetInt32(1),
                Rows = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            };
        }

        return null;
    }

    public List<Map> GetSquareMaps()
    {
        var maps = new List<Map>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, columns, rows, \"Name\", description, createddate, modifieddate FROM public.map WHERE columns = rows ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            maps.Add(new Map
            {
                Id = dr.GetInt32(0),
                Columns = dr.GetInt32(1),
                Rows = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            });
        }

        return maps;
    }

    public Map AddMap(Map newMap)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            """
            INSERT INTO public.map 
            (columns, rows, "Name", description, createddate, modifieddate)
            VALUES (@columns, @rows, @name, @description, current_timestamp, current_timestamp)
            RETURNING id, columns, rows, "Name", description, createddate, modifieddate;
            """,
            conn
        );

        cmd.Parameters.AddWithValue("columns", newMap.Columns);
        cmd.Parameters.AddWithValue("rows", newMap.Rows);
        cmd.Parameters.AddWithValue("name", newMap.Name);
        cmd.Parameters.AddWithValue("description", newMap.Description ?? (object)DBNull.Value);

        using var dr = cmd.ExecuteReader();

        dr.Read();

        return new Map
        {
            Id = dr.GetInt32(0),
            Columns = dr.GetInt32(1),
            Rows = dr.GetInt32(2),
            Name = dr.GetString(3),
            Description = dr.IsDBNull(4) ? null : dr.GetString(4),
            CreatedDate = dr.GetDateTime(5),
            ModifiedDate = dr.GetDateTime(6)
        };
    }

    public Map? UpdateMap(int id, Map updatedMap)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            """
            UPDATE public.map
            SET columns = @columns,
                rows = @rows,
                "Name" = @name,
                description = @description,
                modifieddate = current_timestamp
            WHERE id = @id
            RETURNING id, columns, rows, "Name", description, createddate, modifieddate;
            """,
            conn
        );

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("columns", updatedMap.Columns);
        cmd.Parameters.AddWithValue("rows", updatedMap.Rows);
        cmd.Parameters.AddWithValue("name", updatedMap.Name);
        cmd.Parameters.AddWithValue("description", updatedMap.Description ?? (object)DBNull.Value);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return new Map
            {
                Id = dr.GetInt32(0),
                Columns = dr.GetInt32(1),
                Rows = dr.GetInt32(2),
                Name = dr.GetString(3),
                Description = dr.IsDBNull(4) ? null : dr.GetString(4),
                CreatedDate = dr.GetDateTime(5),
                ModifiedDate = dr.GetDateTime(6)
            };
        }

        return null;
    }

    public bool IsCoordinateOnMap(int id, int x, int y)
    {
        var map = GetMapById(id);

        if (map == null)
        {
            return false;
        }

        return x >= 0 && x < map.Columns && y >= 0 && y < map.Rows;
    }

    public bool DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "DELETE FROM public.map WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
}