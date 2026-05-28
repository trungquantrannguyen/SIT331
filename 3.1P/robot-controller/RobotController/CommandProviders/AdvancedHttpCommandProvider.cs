using RobotController.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace RobotController.CommandProviders
{
    internal class AdvancedHttpCommandProvider : IMetaCommandProvider
    {
        private readonly HttpClient httpClient;

        public string MetaCommand => ":api";

        public AdvancedHttpCommandProvider() : this(new HttpClient())
        {
        }

        public AdvancedHttpCommandProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IEnumerable<ICommand> GetCommands(string[] args)
        {
            if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
            {
                yield break;
            }

            var url = args[0];
            var response = httpClient.GetStringAsync(url).Result;

            if (string.IsNullOrWhiteSpace(response))
            {
                yield break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var commandSet = JsonSerializer.Deserialize<CommandSetDto>(response, options);

            if (commandSet == null || commandSet.Commands == null)
            {
                yield break;
            }

            var mappedCommands = new List<ICommand>();

            foreach (var dto in commandSet.Commands)
            {
                foreach (var mapped in Map(dto))
                {
                    mappedCommands.Add(mapped);
                }
            }

            var executionMode = string.IsNullOrWhiteSpace(commandSet.ExecutionMode)
                ? "BestEffort"
                : commandSet.ExecutionMode;

            if (executionMode.Equals("AllOrNothing", StringComparison.OrdinalIgnoreCase))
            {
                yield return new AtomicCommand(mappedCommands);
                yield break;
            }

            foreach (var command in mappedCommands)
            {
                yield return command;
            }
        }

        private static IEnumerable<ICommand> Map(RobotCommandDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                yield break;
            }

            var name = dto.Name.Trim().ToUpperInvariant();

            switch (name)
            {
                case "MOVE":
                    var moveSteps = dto.NumberOfSteps.GetValueOrDefault(1);
                    if (moveSteps < 1)
                    {
                        moveSteps = 1;
                    }

                    for (var i = 0; i < moveSteps; i++)
                    {
                        yield return new MoveCommand();
                    }

                    yield break;

                case "LEFT":
                    yield return new LeftCommand();
                    yield break;

                case "RIGHT":
                    yield return new RightCommand();
                    yield break;

                case "REPORT":
                    yield return new ReportCommand();
                    yield break;

                case "PLACE":
                    yield return DeserializePlace(dto);
                    yield break;

                case "STEP_BACK":
                    yield return new StepBackCommand(dto.NumberOfSteps.GetValueOrDefault(1));
                    yield break;

                case "JUMP_FORWARD":
                    yield return new JumpForwardCommand(dto.NumberOfSteps.GetValueOrDefault(2));
                    yield break;

                case "JUMP_BACKWARD":
                    yield return new JumpBackwardCommand(dto.NumberOfSteps.GetValueOrDefault(2));
                    yield break;
            }
        }

        private static PlaceCommand DeserializePlace(RobotCommandDto dto)
        {
            var directionText = dto.Direction ?? "North";

            Direction direction;

            switch (directionText.Trim().ToUpperInvariant())
            {
                case "NORTH":
                    direction = Direction.North;
                    break;
                case "EAST":
                    direction = Direction.East;
                    break;
                case "SOUTH":
                    direction = Direction.South;
                    break;
                case "WEST":
                    direction = Direction.West;
                    break;
                default:
                    direction = Direction.North;
                    break;
            }

            return new PlaceCommand(
                dto.X.GetValueOrDefault(0),
                dto.Y.GetValueOrDefault(0),
                direction);
        }
    }

    internal class RobotCommandDto
    {
        public string Name { get; set; }
        public bool? IsMoveCommand { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public string Direction { get; set; }
        public string Comment { get; set; }
        public int? NumberOfSteps { get; set; }
    }

    internal class CommandSetDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string SchemaVersion { get; set; }
        public string ExecutionMode { get; set; }
        public List<RobotCommandDto> Commands { get; set; } = new List<RobotCommandDto>();
    }
}