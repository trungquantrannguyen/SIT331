namespace robot_controller_api;

public class Map
{
    public int Id { get; set; }
    public int Columns { get; set; }
    public int Rows { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public Map(int id, string name, string? description, int columns, int rows, DateTime createdDate, DateTime modifiedDate)
    {
        Id = id;
        Name = name;
        Description = description;
        Columns = columns;
        Rows = rows;
        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
    }
}