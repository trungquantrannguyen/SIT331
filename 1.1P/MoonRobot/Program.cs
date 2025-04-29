using System;
using System.Text.RegularExpressions;

namespace MoonRobot
{
    enum Directions
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
        WEST = 4
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the map size: ");
            string input = Console.ReadLine();
            Regex regex = new Regex(@"\b([2-9]|[1-9][0-9]|100)\b");
            if (regex.IsMatch(input))
            {
                int map = int.Parse(input);
                Robot robot = new Robot(0, 0, Directions.NORTH, map);
                Console.WriteLine("Commands: W - Move, A - Turn Left, D - Turn Right, S - Land");
                while (true)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    robot.Command(key);
                    if (!robot.CheckIsLanded())
                    {
                        Console.WriteLine("Please land the robot");
                        continue;
                    }
                    robot.Logging();
                }
            }
            else
            {
                Console.WriteLine("Input must be a number between 2 and 100");
            }
        }
    }

    internal class Robot
    {
        int x;
        int y;
        Directions direction;

        bool isLanded;
        int map;

        public Robot(int x, int y, Directions direction, int map)
        {
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.isLanded = false;
            this.map = map;
        }

        public void Command(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.W:
                    Move();
                    break;
                case ConsoleKey.A:
                    TurnLeft();
                    break;
                case ConsoleKey.D:
                    TurnRight();
                    break;
                case ConsoleKey.S:
                    Land();
                    break;
            }
        }

        public void Move()
        {
            switch (this.direction)
            {
                case Directions.NORTH:
                    if (this.y == this.map)
                    {
                        return;
                    }
                    this.y++;
                    break;
                case Directions.EAST:
                    if (this.x == this.map)
                    {
                        return;
                    }
                    this.x++;
                    break;
                case Directions.SOUTH:
                    if (this.y == 0)
                    {
                        return;
                    }
                    this.y--;
                    break;
                case Directions.WEST:
                    if (this.x == 0)
                    {
                        return;
                    }
                    this.x--;
                    break;
            }
        }

        public void TurnLeft()
        {
            this.direction = (Directions)((int)this.direction - 1);
            if ((int)this.direction < 1)
            {
                this.direction = Directions.WEST;
            }
        }
        public void TurnRight()
        {
            this.direction = (Directions)((int)this.direction + 1);
            if ((int)this.direction > 4)
            {
                this.direction = Directions.NORTH;
            }
        }
        public void Land()
        {
            if (!this.isLanded)
            {
                Console.WriteLine("Enter the landing coordinate...");
                Console.WriteLine("Enter the X coordinate: ");
                string x = Console.ReadLine();
                while (int.Parse(x) > map)
                {
                    Console.WriteLine("X coordinate must be in the map, please enter: ");
                    x = Console.ReadLine();
                }
                Console.WriteLine("Enter the Y coordinate: ");
                string y = Console.ReadLine();
                while (int.Parse(y) > map)
                {
                    Console.WriteLine("Y coordinate must be in the map, please enter: ");
                    y = Console.ReadLine();
                }
                Console.WriteLine("Enter the direction N for North, E for East, W for West, S for South: ");
                string direction = Console.ReadLine();
                this.x = int.Parse(x);
                this.y = int.Parse(y);
                switch (direction.ToUpper())
                {
                    case "N":
                        this.direction = Directions.NORTH;
                        break;
                    case "E":
                        this.direction = Directions.EAST;
                        break;
                    case "W":
                        this.direction = Directions.WEST;
                        break;
                    case "S":
                        this.direction = Directions.SOUTH;
                        break;
                }
                this.isLanded = true;
            }
            return;
        }
        public void Logging()
        {
            Console.WriteLine($"Robot posistion: X: {this.x}, Y: {this.y}, Direction: {this.direction}");
        }

        public bool CheckIsLanded()
        {
            return this.isLanded;
        }
    }
}