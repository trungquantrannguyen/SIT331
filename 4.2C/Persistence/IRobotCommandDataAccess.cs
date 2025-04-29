namespace robot_controller_api.Persistence;
public interface IRobotCommandDataAccess
{
    void DeleteRobotCommand(int id);
    RobotCommand GetRobotCommandById(int id);
    List<RobotCommand> GetRobotCommands();
    RobotCommand InsertRobotCommand(string name, string? description, bool isMoveCommand);
    RobotCommand UpdateRobotCommand(RobotCommand updatedCommand);
}