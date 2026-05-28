var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

var robotCommands = new List<RobotCommand>
{
    new RobotCommand(1, "LEFT", false, "Turns the robot 90 degrees to the left."),
    new RobotCommand(2, "RIGHT", false, "Turns the robot 90 degrees to the right."),
    new RobotCommand(3, "PLACE", false, "Places the robot on the map using X, Y and direction."),
    new RobotCommand(4, "MOVE", true, "Moves the robot one square forward in the direction it is facing.")
};

var robotMap = new RobotMap(5);

app.MapGet("/", () => Results.Ok("Hello, Robot!"));

app.MapGet("/robot-commands", () => Results.Ok(robotCommands));

app.MapGet("/robot-commands/move", () =>
{
    var moveCommands = robotCommands.Where(command => command.IsMoveCommand).ToList();
    return Results.Ok(moveCommands);
});

app.MapGet("/robot-commands/{id:int}", (int id) =>
{
    var command = robotCommands.FirstOrDefault(command => command.Id == id);
    return command is null ? Results.NotFound($"Robot command with ID {id} was not found.") : Results.Ok(command);
});

app.MapPost("/robot-commands", (RobotCommand command) =>
{
    if (string.IsNullOrWhiteSpace(command.Name))
    {
        return Results.BadRequest("Command name is required.");
    }

    if (robotCommands.Any(existing => existing.Id == command.Id))
    {
        return Results.BadRequest($"Robot command with ID {command.Id} already exists.");
    }

    var newCommand = command with
    {
        Name = command.Name.Trim().ToUpperInvariant(),
        Description = command.Description?.Trim() ?? string.Empty
    };

    robotCommands.Add(newCommand);
    return Results.Created($"/robot-commands/{newCommand.Id}", newCommand);
});

app.MapPut("/robot-commands/{id:int}", (int id, RobotCommand updatedCommand) =>
{
    var commandIndex = robotCommands.FindIndex(command => command.Id == id);

    if (commandIndex == -1)
    {
        return Results.NotFound($"Robot command with ID {id} was not found.");
    }

    if (string.IsNullOrWhiteSpace(updatedCommand.Name))
    {
        return Results.BadRequest("Command name is required.");
    }

    robotCommands[commandIndex] = updatedCommand with
    {
        Id = id,
        Name = updatedCommand.Name.Trim().ToUpperInvariant(),
        Description = updatedCommand.Description?.Trim() ?? string.Empty
    };

    return Results.NoContent();
});

app.MapGet("/robot-map", () => Results.Ok(robotMap));

app.MapGet("/robot-map/{coordinate}", (string coordinate) =>
{
    var parts = coordinate.Split('-');

    if (parts.Length != 2 || !int.TryParse(parts[0], out var x) || !int.TryParse(parts[1], out var y))
    {
        return Results.BadRequest("Coordinate must be in the form x-y, for example 0-0 or 3-5.");
    }

    var isOnMap = robotMap.ContainsCoordinate(x, y);
    return Results.Ok(isOnMap);
});

app.MapPut("/robot-map", (RobotMap updatedMap) =>
{
    if (!updatedMap.IsValid())
    {
        return Results.BadRequest("Robot map must be square and have a size between 2x2 and 100x100.");
    }

    robotMap = updatedMap;
    return Results.NoContent();
});

app.Run();

public record RobotCommand(int Id, string Name, bool IsMoveCommand, string? Description);

public record RobotMap(int Size)
{
    public int Width => Size;
    public int Height => Size;

    public bool IsValid()
    {
        return Size >= 2 && Size <= 100 && Width == Height;
    }

    public bool ContainsCoordinate(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
