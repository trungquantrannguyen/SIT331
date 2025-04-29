using Npgsql;

namespace robot_controller_api.Persistence;
public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;
    public List<RobotCommand> GetRobotCommands()
    {
        var commands = _repo.ExecuteReader<RobotCommand>("SELECT * FROMpublic.robotcommand");
        return commands;
    }
    public RobotCommand UpdateRobotCommand(RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", updatedCommand.Id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("ismovecommand", updatedCommand.IsMoveCommand)
        };
        var result = _repo.ExecuteReader<RobotCommand>(
        "UPDATE robotcommand SET name=@name, description=@description, ismovecommand = @ismovecommand, modifieddate = current_timestamp WHERE id = @id RETURNING *; ",
    sqlParams)
.Single();
        return result;
    }

    public void DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", id)
        };
        _repo.ExecuteReader<RobotCommand>("DELETE FROM robotcommand WHERE id = @id", sqlParams);
    }

    public RobotCommand GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("id", id)
        };
        var result = _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM robotcommand WHERE id = @id", sqlParams).SingleOrDefault();
        return result;
    }

    public RobotCommand InsertRobotCommand(string name, string? description, bool isMoveCommand)
    {
        var sqlParams = new NpgsqlParameter[]{
            new("name", name),
            new("description", description ?? (object)DBNull.Value),
            new("ismovecommand", isMoveCommand)
        };
        var result = _repo.ExecuteReader<RobotCommand>(
            "INSERT INTO robotcommand (name, description, ismovecommand) VALUES (@name, @description, @ismovecommand) RETURNING *;", sqlParams).Single();
        return result;
    }
}