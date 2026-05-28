using robot_controller_api.Persistence;

namespace robot_controller_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess
{
    private readonly RobotContext _context;

    public RobotCommandRepository(RobotContext context)
    {
        _context = context;
    }

    public List<RobotCommand> GetRobotCommands()
    {
        return _context.RobotCommands
            .Select(command => new RobotCommand
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Description,
                IsMoveCommand = command.IsMoveCommand,
                CreatedDate = command.CreatedDate,
                ModifiedDate = command.ModifiedDate
            })
            .ToList();
    }

    public List<RobotCommand> GetMoveCommandsOnly()
    {
        return _context.RobotCommands
            .Where(command => command.IsMoveCommand)
            .Select(command => new RobotCommand
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Description,
                IsMoveCommand = command.IsMoveCommand,
                CreatedDate = command.CreatedDate,
                ModifiedDate = command.ModifiedDate
            })
            .ToList();
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        var command = _context.RobotCommands.FirstOrDefault(c => c.Id == id);

        if (command == null)
        {
            return null;
        }

        return new RobotCommand
        {
            Id = command.Id,
            Name = command.Name,
            Description = command.Description,
            IsMoveCommand = command.IsMoveCommand,
            CreatedDate = command.CreatedDate,
            ModifiedDate = command.ModifiedDate
        };
    }

    public RobotCommand AddRobotCommand(RobotCommand robotCommand)
    {
        var entity = new RobotCommandEF
        {
            Name = robotCommand.Name,
            Description = robotCommand.Description,
            IsMoveCommand = robotCommand.IsMoveCommand,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        _context.RobotCommands.Add(entity);
        _context.SaveChanges();

        robotCommand.Id = entity.Id;
        robotCommand.CreatedDate = entity.CreatedDate;
        robotCommand.ModifiedDate = entity.ModifiedDate;

        return robotCommand;
    }

    public RobotCommand? UpdateRobotCommand(int id, RobotCommand robotCommand)
    {
        var entity = _context.RobotCommands.FirstOrDefault(c => c.Id == id);

        if (entity == null)
        {
            return null;
        }

        entity.Name = robotCommand.Name;
        entity.Description = robotCommand.Description;
        entity.IsMoveCommand = robotCommand.IsMoveCommand;
        entity.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        robotCommand.Id = entity.Id;
        robotCommand.CreatedDate = entity.CreatedDate;
        robotCommand.ModifiedDate = entity.ModifiedDate;

        return robotCommand;
    }

    public bool DeleteRobotCommand(int id)
    {
        var entity = _context.RobotCommands.FirstOrDefault(c => c.Id == id);

        if (entity == null)
        {
            return false;
        }

        _context.RobotCommands.Remove(entity);
        _context.SaveChanges();

        return true;
    }
}