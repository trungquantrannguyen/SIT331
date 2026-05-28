namespace RobotController.Commands
{
    internal class StepBackCommand : ICommand
    {
        private readonly int steps;

        public StepBackCommand(int steps = 1)
        {
            this.steps = steps < 1 ? 1 : steps;
        }

        public string Name => "STEP_BACK";

        public string Description => $"Moves backward {steps} cell(s).";

        public bool Executed { get; set; }

        public bool Success { get; set; }

        public bool Execute(IRobot robot)
        {
            Executed = true;

            if (robot.CurrentMap == null || robot.CurrentPosition == null)
            {
                return Success = false;
            }

            int targetX = robot.CurrentPosition.X;
            int targetY = robot.CurrentPosition.Y;

            switch (robot.Facing)
            {
                case Direction.North:
                    targetY -= steps;
                    break;
                case Direction.East:
                    targetX -= steps;
                    break;
                case Direction.South:
                    targetY += steps;
                    break;
                case Direction.West:
                    targetX += steps;
                    break;
            }

            if (!robot.CurrentMap.IsOnMap(targetX, targetY))
            {
                return Success = false;
            }

            robot.CurrentPosition.X = targetX;
            robot.CurrentPosition.Y = targetY;

            return Success = true;
        }
    }
}