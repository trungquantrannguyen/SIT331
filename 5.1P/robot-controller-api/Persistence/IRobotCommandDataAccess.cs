namespace robot_controller_api.Persistence;

public interface IRobotCommandDataAccess
{
    List<RobotCommand> GetRobotCommands();

    RobotCommand? GetRobotCommandById(int id);

    List<RobotCommand> GetMoveCommandsOnly();

    RobotCommand AddRobotCommand(RobotCommand newCommand);

    RobotCommand? UpdateRobotCommand(int id, RobotCommand updatedCommand);

    bool DeleteRobotCommand(int id);
}