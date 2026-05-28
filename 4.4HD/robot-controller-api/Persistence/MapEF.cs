using System;
using System.Collections.Generic;

namespace robot_controller_api.Persistence;

public class MapEF
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Rows { get; set; }

    public int Columns { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool? IsSquare { get; set; }
}
