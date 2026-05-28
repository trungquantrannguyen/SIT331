using Npgsql;

namespace robot_controller_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository Repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        return Repo.ExecuteReader<RobotCommand>(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand ORDER BY id"
        );
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        return Repo.ExecuteReader<RobotCommand>(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand WHERE id = @id",
            sqlParams
        ).SingleOrDefault();
    }

    public List<RobotCommand> GetMoveCommandsOnly()
    {
        return Repo.ExecuteReader<RobotCommand>(
            "SELECT id, \"Name\", description, ismovecommand, createddate, modifieddate FROM public.robotcommand WHERE ismovecommand = true ORDER BY id"
        );
    }

    public RobotCommand AddRobotCommand(RobotCommand newCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("name", newCommand.Name),
            new("description", newCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", newCommand.IsMoveCommand)
        };

        return Repo.ExecuteReader<RobotCommand>(
            """
            INSERT INTO public.robotcommand 
            ("Name", description, ismovecommand, createddate, modifieddate)
            VALUES (@name, @description, @ismovecommand, current_timestamp, current_timestamp)
            RETURNING id, "Name", description, ismovecommand, createddate, modifieddate;
            """,
            sqlParams
        ).Single();
    }

    public RobotCommand? UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", updatedCommand.IsMoveCommand)
        };

        return Repo.ExecuteReader<RobotCommand>(
            """
            UPDATE public.robotcommand
            SET \"Name\" = @name,
                description = @description,
                ismovecommand = @ismovecommand,
                modifieddate = current_timestamp
            WHERE id = @id
            RETURNING id, "Name", description, ismovecommand, createddate, modifieddate;
            """,
            sqlParams
        ).SingleOrDefault();
    }

    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id)
        };

        var affectedRows = Repo.ExecuteNonQuery(
            "DELETE FROM public.robotcommand WHERE id = @id",
            sqlParams
        );

        return affectedRows > 0;
    }
}