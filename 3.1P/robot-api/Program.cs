using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

var commandSets = new List<CommandSet>
{
    new CommandSet(
        comment: "HD rollback capable workflow",
        schemaVersion: "2.0",
        executionMode: "BestEffort",
        commands: new List<RobotCommandRecord>
        {
            new RobotCommandRecord("PLACE", X: 2, Y: 2, Direction: "North"),
            new RobotCommandRecord("MOVE"),
            new RobotCommandRecord("RIGHT"),
            new RobotCommandRecord("MOVE"),
            new RobotCommandRecord("REPORT")
        })
    {
        Id = 1
    },

    new CommandSet(
        comment: "AllOrNothing workflow that should succeed",
        schemaVersion: "2.0",
        executionMode: "AllOrNothing",
        commands: new List<RobotCommandRecord>
        {
            new RobotCommandRecord("PLACE", X: 0, Y: 0, Direction: "North"),
            new RobotCommandRecord("MOVE", NumberOfSteps: 2),
            new RobotCommandRecord("RIGHT"),
            new RobotCommandRecord("JUMP_FORWARD", NumberOfSteps: 2),
            new RobotCommandRecord("REPORT")
        })
    {
        Id = 2
    },

    new CommandSet(
        comment: "AllOrNothing workflow that should fail near edge",
        schemaVersion: "2.0",
        executionMode: "AllOrNothing",
        commands: new List<RobotCommandRecord>
        {
            new RobotCommandRecord("PLACE", X: 0, Y: 9, Direction: "North"),
            new RobotCommandRecord("MOVE"),
            new RobotCommandRecord("REPORT")
        })
    {
        Id = 3
    }
};

var executionLogs = new List<CommandExecutionLog>();
var rollbackSets = new Dictionary<int, CommandSet>();

app.MapGet("/", () => Results.Ok("SIT331 3.1QP Robot API is running."));

app.MapGet("/command-sets", () =>
{
    return Results.Ok(commandSets);
});

app.MapGet("/command-sets/{id:int}", (int id) =>
{
    var commandSet = commandSets.FirstOrDefault(x => x.Id == id);

    if (commandSet == null)
    {
        return Results.NotFound(new
        {
            message = $"Command set {id} was not found."
        });
    }

    return Results.Ok(commandSet);
});

app.MapPost("/command-sets", (CommandSet commandSet) =>
{
    commandSet.Id = commandSets.Count == 0
        ? 1
        : commandSets.Max(x => x.Id) + 1;

    commandSet.SchemaVersion ??= "2.0";
    commandSet.ExecutionMode ??= "BestEffort";

    commandSets.Add(commandSet);

    return Results.Created($"/command-sets/{commandSet.Id}", commandSet);
});

app.MapPut("/command-sets/{id:int}", (int id, CommandSet updated) =>
{
    var existing = commandSets.FirstOrDefault(x => x.Id == id);

    if (existing == null)
    {
        return Results.NotFound(new
        {
            message = $"Command set {id} was not found."
        });
    }

    existing.Comment = updated.Comment;
    existing.SchemaVersion = updated.SchemaVersion ?? "2.0";
    existing.ExecutionMode = updated.ExecutionMode ?? "BestEffort";
    existing.Commands = updated.Commands ?? new List<RobotCommandRecord>();

    return Results.Ok(existing);
});

app.MapDelete("/command-sets/{id:int}", (int id) =>
{
    var existing = commandSets.FirstOrDefault(x => x.Id == id);

    if (existing == null)
    {
        return Results.NotFound(new
        {
            message = $"Command set {id} was not found."
        });
    }

    commandSets.Remove(existing);

    return Results.Ok(new
    {
        message = $"Command set {id} was deleted."
    });
});

app.MapPost("/command-executions", (CommandExecutionLog log) =>
{
    if (log.Commands == null || log.Commands.Count == 0)
    {
        return Results.BadRequest(new
        {
            message = "Execution log must contain at least one command."
        });
    }

    log.SchemaVersion ??= "2.0";
    executionLogs.RemoveAll(x => x.WorkflowId == log.WorkflowId);
    executionLogs.Add(log);

    return Results.Ok(new
    {
        message = "Execution log saved successfully.",
        workflowId = log.WorkflowId,
        commandCount = log.Commands.Count
    });
});

app.MapGet("/command-executions/{workflowId:int}", (int workflowId) =>
{
    var log = executionLogs.FirstOrDefault(x => x.WorkflowId == workflowId);

    if (log == null)
    {
        return Results.NotFound(new
        {
            message = $"No execution log exists for workflow {workflowId}."
        });
    }

    return Results.Ok(log);
});

app.MapPost("/command-sets/{id:int}/rollback", (int id) =>
{
    var log = executionLogs.FirstOrDefault(x => x.WorkflowId == id);

    if (log == null)
    {
        return Results.NotFound(new
        {
            message = $"Cannot generate rollback because no execution log exists for workflow {id}."
        });
    }

    var successfulCommands = log.Commands
        .Where(x => x.Success)
        .ToList();

    successfulCommands.Reverse();

    var rollbackCommands = new List<RobotCommandRecord>();

    foreach (var command in successfulCommands)
    {
        if (command.Name.Equals("PLACE", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        var compensatingCommand = CreateCompensatingCommand(command);

        if (compensatingCommand != null)
        {
            rollbackCommands.Add(compensatingCommand);
        }
    }

    rollbackCommands.Add(new RobotCommandRecord("REPORT"));

    var rollbackSet = new CommandSet(
        comment: $"Generated rollback for workflow {id}",
        schemaVersion: log.SchemaVersion ?? "2.0",
        executionMode: "AllOrNothing",
        commands: rollbackCommands)
    {
        Id = id
    };

    rollbackSets[id] = rollbackSet;

    return Results.Ok(rollbackSet);
});

app.MapGet("/command-sets/{id:int}/rollback", (int id) =>
{
    if (!rollbackSets.TryGetValue(id, out var rollbackSet))
    {
        return Results.NotFound(new
        {
            message = $"No rollback command set has been generated for workflow {id}."
        });
    }

    return Results.Ok(rollbackSet);
});

app.Run();

static RobotCommandRecord? CreateCompensatingCommand(ExecutedCommandRecord command)
{
    var name = command.Name.ToUpperInvariant();

    return name switch
    {
        "LEFT" => new RobotCommandRecord("RIGHT"),
        "RIGHT" => new RobotCommandRecord("LEFT"),
        "MOVE" => new RobotCommandRecord("STEP_BACK", NumberOfSteps: command.NumberOfSteps),
        "JUMP_FORWARD" => new RobotCommandRecord("JUMP_BACKWARD", NumberOfSteps: command.NumberOfSteps),
        "STEP_BACK" => new RobotCommandRecord("MOVE", NumberOfSteps: command.NumberOfSteps),
        "JUMP_BACKWARD" => new RobotCommandRecord("JUMP_FORWARD", NumberOfSteps: command.NumberOfSteps),
        "REPORT" => null,
        _ => null
    };
}

public record CommandSet
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public string? SchemaVersion { get; set; }
    public string? ExecutionMode { get; set; }
    public List<RobotCommandRecord> Commands { get; set; }

    public CommandSet(
        string comment,
        string? schemaVersion,
        string? executionMode,
        List<RobotCommandRecord> commands)
    {
        Id = 0;
        Comment = comment;
        SchemaVersion = schemaVersion;
        ExecutionMode = executionMode;
        Commands = commands ?? new List<RobotCommandRecord>();
    }
}

public record RobotCommandRecord(
    string Name,
    bool? IsMoveCommand = null,
    int? X = null,
    int? Y = null,
    string? Direction = null,
    string? Comment = null,
    int? NumberOfSteps = null
);

public record ExecutedCommandRecord
{
    public string Name { get; set; } = "";
    public bool Executed { get; set; }
    public bool Success { get; set; }
    public int? X { get; set; }
    public int? Y { get; set; }
    public string? Direction { get; set; }
    public string? Comment { get; set; }
    public int? NumberOfSteps { get; set; }
}

public record CommandExecutionLog
{
    public int WorkflowId { get; set; }
    public string? SchemaVersion { get; set; }
    public List<ExecutedCommandRecord> Commands { get; set; } = new();
}