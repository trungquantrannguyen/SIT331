namespace robot_controller_api;

public class RobotCommand
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsMoveCommand { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Parameterless constructor is useful for ASP.NET Core JSON model binding.
    public RobotCommand()
    {
    }

    // Parameterized constructor required by the task sheet.
    public RobotCommand(
        int id,
        string name,
        bool isMoveCommand,
        DateTime createdDate,
        DateTime modifiedDate,
        string? description = null)
    {
        Id = id;
        Name = name;
        IsMoveCommand = isMoveCommand;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
        Description = description;
    }
}