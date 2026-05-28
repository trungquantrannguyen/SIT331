using Npgsql;

namespace robot_controller_api.Persistence;

public class RobotCommandADO : IRobotCommandDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Port=5432;Username=trungquan;Password=0812;Database=sit331";

    public List<RobotCommand> GetRobotCommands()
    {
        var commands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            commands.Add(new RobotCommand
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                IsMoveCommand = dr.GetBoolean(3),
                CreatedDate = dr.GetDateTime(4),
                ModifiedDate = dr.GetDateTime(5)
            });
        }

        return commands;
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
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

        return null;
    }

    public List<RobotCommand> GetMoveCommandsOnly()
    {
        var commands = new List<RobotCommand>();

        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand WHERE ismovecommand = true ORDER BY id",
            conn
        );

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            commands.Add(new RobotCommand
            {
                Id = dr.GetInt32(0),
                Name = dr.GetString(1),
                Description = dr.IsDBNull(2) ? null : dr.GetString(2),
                IsMoveCommand = dr.GetBoolean(3),
                CreatedDate = dr.GetDateTime(4),
                ModifiedDate = dr.GetDateTime(5)
            });
        }

        return commands;
    }

    public RobotCommand AddRobotCommand(RobotCommand newCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            """
            INSERT INTO public.robotcommand 
            ("Name", description, ismovecommand, createddate, modifieddate)
            VALUES (@name, @description, @ismovecommand, current_timestamp, current_timestamp)
            RETURNING id, "Name", description, ismovecommand, createddate, modifieddate;
            """,
            conn
        );

        cmd.Parameters.AddWithValue("name", newCommand.Name);
        cmd.Parameters.AddWithValue("description", newCommand.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ismovecommand", newCommand.IsMoveCommand);

        using var dr = cmd.ExecuteReader();

        dr.Read();

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

    public RobotCommand? UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            """
            UPDATE public.robotcommand
            SET "Name" = @name,
                description = @description,
                ismovecommand = @ismovecommand,
                modifieddate = current_timestamp
            WHERE id = @id
            RETURNING id, "Name", description, ismovecommand, createddate, modifieddate;
            """,
            conn
        );

        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", updatedCommand.Name);
        cmd.Parameters.AddWithValue("description", updatedCommand.Description ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("ismovecommand", updatedCommand.IsMoveCommand);

        using var dr = cmd.ExecuteReader();

        if (dr.Read())
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

        return null;
    }

    public bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "DELETE FROM public.robotcommand WHERE id = @id",
            conn
        );

        cmd.Parameters.AddWithValue("id", id);

        return cmd.ExecuteNonQuery() > 0;
    }
}