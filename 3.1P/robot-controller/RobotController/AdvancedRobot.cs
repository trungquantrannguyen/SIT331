using RobotController.Commands;
using RobotController.States;
using System.Collections.Generic;

namespace RobotController
{
    public class AdvancedRobot : IRobot
    {
        private readonly List<ICommand> commandHistory = new List<ICommand>();

        public IReadOnlyList<ICommand> CommandHistory
        {
            get { return commandHistory.AsReadOnly(); }
        }

        public Map CurrentMap { get; set; }

        public Coordinate CurrentPosition { get; set; }

        public Direction Facing { get; set; }

        public IState CurrentState { get; set; }

        public bool CanMove
        {
            get
            {
                return CanMoveSteps(1);
            }
        }

        public AdvancedRobot(Map map)
        {
            CurrentState = new IdleState(this);
            CurrentMap = map;
        }

        public bool CanMoveSteps(int steps)
        {
            if (steps < 1)
            {
                steps = 1;
            }

            if (CurrentMap == null || CurrentPosition == null)
            {
                return false;
            }

            switch (Facing)
            {
                case Direction.North:
                    return CurrentMap.IsOnMap(CurrentPosition.X, CurrentPosition.Y + steps);
                case Direction.East:
                    return CurrentMap.IsOnMap(CurrentPosition.X + steps, CurrentPosition.Y);
                case Direction.South:
                    return CurrentMap.IsOnMap(CurrentPosition.X, CurrentPosition.Y - steps);
                case Direction.West:
                    return CurrentMap.IsOnMap(CurrentPosition.X - steps, CurrentPosition.Y);
                default:
                    return false;
            }
        }

        public void ExecuteCommand(ICommand command)
        {
            CurrentState.ExecuteCommand(command);
            commandHistory.Add(command);
        }

        public static AdvancedRobot Clone(AdvancedRobot original)
        {
            var clone = new AdvancedRobot(original.CurrentMap);

            if (original.CurrentPosition != null)
            {
                clone.CurrentPosition = new Coordinate(
                    original.CurrentPosition.X,
                    original.CurrentPosition.Y);
            }

            clone.Facing = original.Facing;

            if (original.CurrentState is ActiveState)
            {
                clone.CurrentState = new ActiveState(clone);
            }
            else
            {
                clone.CurrentState = new IdleState(clone);
            }

            return clone;
        }
    }
}