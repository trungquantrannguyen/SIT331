using System;

namespace _2._1P.Entities;

public class Command
{
    public int Id { get; set; }
    public required string CommandName { get; set; }

    public required bool IsMove { get; set; }
}
