namespace robot_controller_api;

public class RobotCommand
{
    /// Implement <see cref="RobotCommand"> here following the task sheet requirements
    /// 
    public RobotCommand(
        int id,
        string name,
        string? description,
        bool isMoveCommand,
        DateTime CreatedDate,
        DateTime ModifiedDate
        )
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.IsMoveCommand = isMoveCommand;
        this.CreatedDate = CreatedDate;
        this.ModifiedDate = ModifiedDate;
    }
    int Id { get; set; }
    string Name { get; set; }
    string? Description { get; set; }

    bool IsMoveCommand { get; set; }

    DateTime CreatedDate { get; set; }

    DateTime ModifiedDate { get; set; }
}
