namespace robot_controller_api;

public class RobotCommand
{
    /// Implement <see cref="RobotCommand"> here following the task sheet requirements
    ///

    public RobotCommand() { }
    public RobotCommand(
        int id,
        string name,
        bool isMoveCommand,
        DateTime CreatedDate,
        DateTime ModifiedDate,
        string? description
        )
    {
        this.Id = id;
        this.Name = name;
        this.Description = description;
        this.IsMoveCommand = isMoveCommand;
        this.CreatedDate = CreatedDate;
        this.ModifiedDate = ModifiedDate;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public bool IsMoveCommand { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }
}
