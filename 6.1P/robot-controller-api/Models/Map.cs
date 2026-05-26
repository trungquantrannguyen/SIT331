namespace robot_controller_api.Models;

public class Map
{
    public int Id { get; set; }

    public int Columns { get; set; }

    public int Rows { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Parameterless constructor is useful for ASP.NET Core JSON model binding.
    public Map()
    {
    }

    // Parameterized constructor required by the task sheet.
    public Map(
        int id,
        int columns,
        int rows,
        string name,
        DateTime createdDate,
        DateTime modifiedDate,
        string? description = null)
    {
        Id = id;
        Columns = columns;
        Rows = rows;
        Name = name;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
        Description = description;
    }
}