using Npgsql;
namespace robot_controller_api.Persistence;
public static class MapDataAccess
{
    // Connection string is usually set in a config file for the ease of change.
    private const string CONNECTION_STRING =
    "Host=localhost;Username=trungquan;Password=2209quan;Database=sit331";
    public static List<Map> GetMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var id = (int)dr["id"];
            var Columns = (int)dr["columns"];
            var Rows = (int)dr["rows"];
            var Name = (string)dr["name"];
            string? Description = dr["description"] != DBNull.Value ? (string?)dr["description"] : null;
            var CreatedDate = (DateTime)dr["createddate"];
            var ModifiedDate = (DateTime)dr["modifieddate"];
            Map map = new Map(id, Name, Description, Columns, Rows, CreatedDate, ModifiedDate);
            //read values off the data reader and create a new robotCommand here and then add it to the result list.
            maps.Add(map);
        }
        return maps;
    }

    public static Map GetMapById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            var Columns = (int)dr["columns"];
            var Rows = (int)dr["rows"];
            var Name = (string)dr["Name"];
            string? Description = dr["description"] != DBNull.Value ? (string?)dr["description"] : null;
            var CreatedDate = (DateTime)dr["createddate"];
            var ModifiedDate = (DateTime)dr["modifieddate"];
            return new Map(id, Name, Description, Columns, Rows, CreatedDate, ModifiedDate);
        }
        return null;
    }

    public static void UpdateMap(int id, string name, string? description, int columns, int rows)
    {
        var existMap = GetMapById(id);
        if (existMap == null)
        {
            throw new Exception("Map not found");
        }
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("UPDATE map SET \"Name\" = @name, description = @description, columns = @columns, rows = @rows WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("columns", columns);
        cmd.Parameters.AddWithValue("rows", rows);
        cmd.ExecuteNonQuery();
    }
    public static Map InsertMap(string name, string? description, int columns, int rows)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("INSERT INTO map (\"Name\", description, columns, rows, createddate, modifieddate) VALUES (@name, @description, @columns, @rows, @CreatedDate, @ModifiedDate)", conn);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("columns", columns);
        cmd.Parameters.AddWithValue("rows", rows);
        cmd.Parameters.AddWithValue("CreatedDate", DateTime.Now);
        cmd.Parameters.AddWithValue("ModifiedDate", DateTime.Now);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var id = (int)reader["id"];
            var CreatedDate = (DateTime)reader["createddate"];
            var ModifiedDate = (DateTime)reader["modifieddate"];
            return new Map(id, name, description, columns, rows, CreatedDate, ModifiedDate);
        }
        return null;
    }
    public static void DeleteMap(int id)
    {
        var existMap = GetMapById(id);
        if (existMap == null)
        {
            throw new Exception("Map not found");
        }
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}