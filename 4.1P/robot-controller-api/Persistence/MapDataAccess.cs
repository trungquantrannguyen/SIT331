using Npgsql;
using robot_controller_api;

namespace robot_controller_api.Persistence;

public static class MapADO
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=trungquan;Password=0812;Database=sit331";

    public static List<Map> GetMaps()
    {
        var maps = new List<Map>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", rows, columns, description, createddate, modifieddate
              FROM map
              ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            maps.Add(ReadMap(dr));
        }

        return maps;
    }

    public static List<Map> GetSquareMapsOnly()
    {
        var maps = new List<Map>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", rows, columns, description, createddate, modifieddate
              FROM map
              WHERE rows = columns
              ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            maps.Add(ReadMap(dr));
        }

        return maps;
    }

    public static Map? GetMapById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", rows, columns, description, createddate, modifieddate
              FROM map
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return ReadMap(dr);
        }

        return null;
    }

    public static Map InsertMap(Map map)
    {
        var now = DateTime.Now;

        if (map.CreatedDate == default)
        {
            map.CreatedDate = now;
        }

        map.ModifiedDate = now;

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO map
              (""Name"", rows, columns, description, createddate, modifieddate)
              VALUES
              (@name, @rows, @columns, @description, @createddate, @modifieddate)
              RETURNING id",
            conn
        );

        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("createddate", map.CreatedDate);
        cmd.Parameters.AddWithValue("modifieddate", map.ModifiedDate);

        map.Id = Convert.ToInt32(cmd.ExecuteScalar());

        return map;
    }

    public static void UpdateMap(Map map)
    {
        map.ModifiedDate = DateTime.Now;

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE map
              SET ""Name"" = @name,
                  rows = @rows,
                  columns = @columns,
                  description = @description,
                  modifieddate = @modifieddate
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", map.Id);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modifieddate", map.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public static void DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"DELETE FROM map
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        cmd.ExecuteNonQuery();
    }

    private static Map ReadMap(NpgsqlDataReader dr)
    {
        return new Map
        {
            Id = dr.GetInt32(0),
            Name = dr.GetString(1),
            Rows = dr.GetInt32(2),
            Columns = dr.GetInt32(3),
            Description = dr.IsDBNull(4) ? null : dr.GetString(4),
            CreatedDate = dr.GetDateTime(5),
            ModifiedDate = dr.GetDateTime(6)
        };
    }
}