using RobotController.CommandProviders;
using RobotController.States;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RobotTests")]

namespace RobotController
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommandProvider commandProvider;
            string[] providerArgs;

            if (args.Length > 0 &&
                string.Equals(args[0], "dynamic", StringComparison.OrdinalIgnoreCase))
            {
                commandProvider = new DynamicCommandProvider(new IMetaCommandProvider[]
                {
                    new ConsoleCommandProvider(),
                    new FileCommandProvider(),
                    new AdvancedHttpCommandProvider()
                });

                providerArgs = Array.Empty<string>();

                Console.WriteLine("Robot is not placed on the map and is waiting for commands.");
                Console.WriteLine("Meta-commands: :console, :file <path>, :api <url>, :quit");
            }
            else if (args.Length > 0 &&
                     Uri.IsWellFormedUriString(args[0], UriKind.Absolute))
            {
                commandProvider = new AdvancedHttpCommandProvider();
                providerArgs = args;
            }
            else if (args.Length > 0)
            {
                commandProvider = new FileCommandProvider();
                providerArgs = args;
            }
            else
            {
                commandProvider = new ConsoleCommandProvider();
                providerArgs = Array.Empty<string>();

                Console.WriteLine("Robot is not placed on the map and is waiting for commands.");
                Console.WriteLine("Please type one of the supported commands from the robot manual.");
            }

            var map = new Map(10, 10);
            var robot = new AdvancedRobot(map);
            robot.CurrentState = new IdleState(robot);

            foreach (var command in commandProvider.GetCommands(providerArgs))
            {
                robot.ExecuteCommand(command);
            }
        }
    }
}