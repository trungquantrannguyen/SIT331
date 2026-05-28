using RobotController.Commands;
using System.Collections.Generic;

namespace RobotController.CommandProviders
{
    public interface ICommandProvider
    {
        IEnumerable<ICommand> GetCommands(string[] args);
    }

    public interface IMetaCommandProvider : ICommandProvider
    {
        string MetaCommand { get; }
    }
}