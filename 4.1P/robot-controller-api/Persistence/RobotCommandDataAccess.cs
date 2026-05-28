using Npgsql;
using robot_controller_api;

namespace robot_controller_api.Persistence;

public static class RobotCommandADO
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=trungquan;Password=0812;Database=sit331";

    public static List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", description, ismovecommand, createddate, modifieddate
              FROM robotcommand
              ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            robotCommands.Add(ReadRobotCommand(dr));
        }

        return robotCommands;
    }

    public static List<RobotCommand> GetMoveCommandsOnly()
    {
        var robotCommands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", description, ismovecommand, createddate, modifieddate
              FROM robotcommand
              WHERE ismovecommand = true
              ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            robotCommands.Add(ReadRobotCommand(dr));
        }

        return robotCommands;
    }

    public static RobotCommand? GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"SELECT id, ""Name"", description, ismovecommand, createddate, modifieddate
              FROM robotcommand
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return ReadRobotCommand(dr);
        }

        return null;
    }

    public static RobotCommand InsertRobotCommand(RobotCommand robotCommand)
    {
        var now = DateTime.Now;

        if (robotCommand.CreatedDate == default)
        {
            robotCommand.CreatedDate = now;
        }

        robotCommand.ModifiedDate = now;

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"INSERT INTO robotcommand 
              (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES 
              (@name, @description, @ismovecommand, @createddate, @modifieddate)
              RETURNING id",
            conn
        );

        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue("description", (object?)robotCommand.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ismovecommand", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("createddate", robotCommand.CreatedDate);
        cmd.Parameters.AddWithValue("modifieddate", robotCommand.ModifiedDate);

        robotCommand.Id = Convert.ToInt32(cmd.ExecuteScalar());

        return robotCommand;
    }

    public static void UpdateRobotCommand(RobotCommand robotCommand)
    {
        robotCommand.ModifiedDate = DateTime.Now;

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"UPDATE robotcommand
              SET ""Name"" = @name,
                  description = @description,
                  ismovecommand = @ismovecommand,
                  modifieddate = @modifieddate
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", robotCommand.Id);
        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue("description", (object?)robotCommand.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ismovecommand", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("modifieddate", robotCommand.ModifiedDate);

        cmd.ExecuteNonQuery();
    }

    public static void DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            @"DELETE FROM robotcommand
              WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        cmd.ExecuteNonQuery();
    }

    private static RobotCommand ReadRobotCommand(NpgsqlDataReader dr)
    {
        return new RobotCommand
        {
            Id = dr.GetInt32(0),
            Name = dr.GetString(1),
            Description = dr.IsDBNull(2) ? null : dr.GetString(2),
            IsMoveCommand = dr.GetBoolean(3),
            CreatedDate = dr.GetDateTime(4),
            ModifiedDate = dr.GetDateTime(5)
        };
    }
}