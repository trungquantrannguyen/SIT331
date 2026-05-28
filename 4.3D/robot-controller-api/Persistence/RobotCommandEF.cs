using Microsoft.EntityFrameworkCore;

namespace robot_controller_api.Persistence;

public class RobotCommandEF : IRobotCommandDataAccess
{
    private readonly RobotContext _context;

    public RobotCommandEF(RobotContext context)
    {
        _context = context;
    }

    public List<RobotCommand> GetRobotCommands()
    {
        return _context.RobotCommands
            .AsNoTracking()
            .OrderBy(command => command.Id)
            .ToList();
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        return _context.RobotCommands
            .AsNoTracking()
            .FirstOrDefault(command => command.Id == id);
    }

    public List<RobotCommand> GetMoveCommandsOnly()
    {
        return _context.RobotCommands
            .AsNoTracking()
            .Where(command => command.IsMoveCommand)
            .OrderBy(command => command.Id)
            .ToList();
    }

    public RobotCommand AddRobotCommand(RobotCommand robotCommand)
    {
        robotCommand.Id = 0;
        robotCommand.CreatedDate = DateTime.Now;
        robotCommand.ModifiedDate = DateTime.Now;

        _context.RobotCommands.Add(robotCommand);
        _context.SaveChanges();

        return robotCommand;
    }

    public RobotCommand? UpdateRobotCommand(int id, RobotCommand robotCommand)
    {
        var existingCommand = _context.RobotCommands
            .FirstOrDefault(command => command.Id == id);

        if (existingCommand == null)
        {
            return null;
        }

        existingCommand.Name = robotCommand.Name;
        existingCommand.Description = robotCommand.Description;
        existingCommand.IsMoveCommand = robotCommand.IsMoveCommand;
        existingCommand.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        return existingCommand;
    }

    public bool DeleteRobotCommand(int id)
    {
        var existingCommand = _context.RobotCommands
            .FirstOrDefault(command => command.Id == id);

        if (existingCommand == null)
        {
            return false;
        }

        _context.RobotCommands.Remove(existingCommand);
        _context.SaveChanges();

        return true;
    }
}