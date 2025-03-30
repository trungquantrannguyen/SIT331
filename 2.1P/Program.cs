using _2._1P.Dtos;
using _2._1P.Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();

var commands = new List<Command>{
    new() { Id = 1, CommandName = "MOVE", IsMove = true },
    new() { Id = 2, CommandName = "TURN_LEFT", IsMove = false },
    new()  { Id = 3, CommandName = "TURN_RIGHT", IsMove = false },
    new() { Id = 4, CommandName = "PICK_UP", IsMove = false },
    new()  { Id = 5, CommandName = "PLACE", IsMove = false }
};

var map = new Map { X = 5, Y = 5 };

app.MapGet("/", () => "Hello, Robot!");

app.MapGet("/robot-commands", () =>
{
    return commands;
});

app.MapGet("/robot-commands/move", () =>
{
    return commands.Where(c => c.IsMove);
});

app.MapGet("/robot-commands/{id}", (int id) =>
{
    var command = commands.Find(c => c.Id == id);
    if (command == null)
    {
        return Results.NotFound("Command not found.");
    }
    else
    {
        return Results.Ok(command);
    }

});

app.MapPost("/robot-commands", (CreateCommandDto newCommand) =>
{
    var command = new Command
    {
        Id = commands.Max(c => c.Id) + 1,
        CommandName = newCommand.CommandName,
        IsMove = newCommand.IsMove
    };
    commands.Add(command);

    return Results.Created($"/robot-commands/{command.Id}", command);
});

app.MapPut("/robot-commands/{id}", (int id, UpdateCommandDto updateCommand) =>
{
    var command = commands.Find(c => c.Id == id);
    if (command == null)
    {
        return Results.NotFound();
    }
    if (commands.Find(c => c.CommandName == updateCommand.CommandName) != null)
    {
        return Results.Conflict("Command name already exists.");
    }
    command.CommandName = updateCommand.CommandName;
    command.IsMove = updateCommand.IsMove;
    return Results.NoContent();
});

app.MapGet("robot-map", () =>
{
    return Results.Ok(map);
});

app.MapGet("robot-map/{coordinate}", (string coordinate) =>
{
    var coordinates = coordinate.Split('-');
    int x = int.Parse(coordinates[0]);
    int y = int.Parse(coordinates[1]);

    return Results.Ok(map.X >= x && map.Y >= y);
});

app.MapPut("robot-map", (UpdateMapDto updateMap) =>
{
    if (updateMap.X != updateMap.Y)
    {
        return Results.BadRequest("Map must be square.");
    }
    if (updateMap.X < 2 || updateMap.Y < 2)
    {
        return Results.BadRequest("Map cannot be smaller than 2x2.");
    }
    if (updateMap.X > 100 || updateMap.Y > 100)
    {
        return Results.BadRequest("Map cannot be larger than 100x100.");
    }
    map.X = updateMap.X;
    map.Y = updateMap.Y;
    return Results.NoContent();
});
app.Run();