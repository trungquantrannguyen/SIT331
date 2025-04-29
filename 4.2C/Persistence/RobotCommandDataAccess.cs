using Npgsql;
namespace robot_controller_api.Persistence;


public class RobotCommandADO : IRobotCommandDataAccess
{
    // Connection string is usually set in a config file for the ease of change.
    private const string CONNECTION_STRING =
    "Host=localhost;Username=trungquan;Password=2209quan;Database=sit331";
    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            var id = (int)dr["id"];
            var Name = (string)dr["name"];
            string? Description = dr["description"] != DBNull.Value ? (string?)dr["description"] : null;
            bool isMoveCommand = dr["ismovecommand"] != DBNull.Value && (bool)dr["ismovecommand"];
            var CreatedDate = (DateTime)dr["createddate"];
            var ModifiedDate = (DateTime)dr["modifieddate"];
            RobotCommand robotCommand = new RobotCommand(id, Name, isMoveCommand, CreatedDate, ModifiedDate, Description);
            //read values off the data reader and create a new robotCommand here and then add it to the result list.
            robotCommands.Add(robotCommand);
        }
        return robotCommands;
    }

    public RobotCommand GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            var Name = (string)dr["name"];
            string? Description = dr["description"] != DBNull.Value ? (string?)dr["description"] : null;
            bool isMoveCommand = dr["ismovecommand"] != DBNull.Value && (bool)dr["ismovecommand"];
            var CreatedDate = (DateTime)dr["createddate"];
            var ModifiedDate = (DateTime)dr["modifieddate"];
            return new RobotCommand(id, Name, isMoveCommand, CreatedDate, ModifiedDate, Description);
        }
        return null;
    }
    public RobotCommand UpdateRobotCommand(RobotCommand upddateRobotCommand)
    {
        var id = upddateRobotCommand.Id;
        var name = upddateRobotCommand.Name;
        var description = upddateRobotCommand.Description;
        var isMoveCommand = upddateRobotCommand.IsMoveCommand;

        var existRobotCommand = GetRobotCommandById(id);
        if (existRobotCommand == null)
        {
            throw new Exception("Robot command not found");
        }
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("UPDATE robotcommand SET \"Name\" = @name, description = @description, isMoveCommand = @isMoveCommand, modifieddate = @ModifiedDate WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("isMoveCommand", isMoveCommand);
        cmd.Parameters.AddWithValue("ModifiedDate", DateTime.Now);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var CreatedDate = (DateTime)reader["createddate"];
            var ModifiedDate = (DateTime)reader["modifieddate"];
            return new RobotCommand(id, name, isMoveCommand, CreatedDate, ModifiedDate, description);
        }
        return null;
    }

    public RobotCommand InsertRobotCommand(string name, string? description, bool isMoveCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("INSERT INTO robotcommand (  \"Name\", description, ismovecommand, createddate, modifieddate) VALUES ( @name, @description, @isMoveCommand, @CreatedDate, @ModifiedDate)", conn);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("description", description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("isMoveCommand", isMoveCommand);
        cmd.Parameters.AddWithValue("CreatedDate", DateTime.Now);
        cmd.Parameters.AddWithValue("ModifiedDate", DateTime.Now);
        var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            var id = (int)reader["id"];
            var CreatedDate = (DateTime)reader["createddate"];
            var ModifiedDate = (DateTime)reader["modifieddate"];
            return new RobotCommand(id, name, isMoveCommand, CreatedDate, ModifiedDate, description);
        }
        return null;
    }

    public void DeleteRobotCommand(int id)
    {
        var existRobotCommand = GetRobotCommandById(id);
        if (existRobotCommand == null)
        {
            throw new Exception("Robot command not found");
        }
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }
}