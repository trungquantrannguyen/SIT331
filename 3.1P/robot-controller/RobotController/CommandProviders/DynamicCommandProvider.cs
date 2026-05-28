using RobotController.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RobotController.CommandProviders
{
    internal class DynamicCommandProvider : ICommandProvider
    {
        private readonly Dictionary<string, IMetaCommandProvider> providersByMetaCommand;
        private readonly IMetaCommandProvider consoleProvider;
        private string[] currentArgs = Array.Empty<string>();
        private bool shouldExit;

        public DynamicCommandProvider(IReadOnlyCollection<IMetaCommandProvider> commandProviders)
        {
            if (commandProviders == null)
            {
                throw new ArgumentNullException(nameof(commandProviders));
            }

            providersByMetaCommand = commandProviders.ToDictionary(
                p => p.MetaCommand,
                p => p,
                StringComparer.OrdinalIgnoreCase);

            if (!providersByMetaCommand.TryGetValue(":console", out var foundConsoleProvider))
            {
                throw new InvalidOperationException("A :console provider must be registered.");
            }

            consoleProvider = foundConsoleProvider;
            CurrentProvider = consoleProvider;
        }

        public IMetaCommandProvider CurrentProvider { get; private set; }

        public void SwitchTo(IMetaCommandProvider provider, string[] args)
        {
            CurrentProvider = provider;
            currentArgs = args ?? Array.Empty<string>();
        }

        public IEnumerable<ICommand> GetCommands(string[] args)
        {
            while (!shouldExit)
            {
                var switchedProvider = false;

                foreach (var command in CurrentProvider.GetCommands(currentArgs))
                {
                    if (command is UnknownCommand unknownCommand &&
                        !string.IsNullOrWhiteSpace(unknownCommand.Input))
                    {
                        var parts = unknownCommand.Input
                            .Trim()
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        var metaCommand = parts[0];

                        if (metaCommand.Equals(":quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Exiting dynamic command provider.");
                            shouldExit = true;
                            yield break;
                        }

                        if (providersByMetaCommand.TryGetValue(metaCommand, out var provider))
                        {
                            SwitchTo(provider, parts.Skip(1).ToArray());
                            Console.WriteLine("Switched to provider " + metaCommand + ".");
                            switchedProvider = true;
                            break;
                        }

                        Console.WriteLine("Unsupported command: " + unknownCommand.Input);
                        continue;
                    }

                    yield return command;
                }

                if (shouldExit)
                {
                    yield break;
                }

                if (switchedProvider)
                {
                    continue;
                }

                if (!ReferenceEquals(CurrentProvider, consoleProvider))
                {
                    SwitchTo(consoleProvider, Array.Empty<string>());
                    Console.WriteLine("Switched back to console input.");
                }
            }
        }
    }
}