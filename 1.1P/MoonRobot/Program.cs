using System;
using System.Text.RegularExpressions;

namespace MoonRobot
{
    enum Directions{
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
            Regex regex = new Regex(@"[\d]");
            if(regex.IsMatch(input)){
                int map = int.Parse(input);
                Robot robot = new Robot(0, 0, Directions.NORTH, map);
                Console.WriteLine("Commands: W - Move, A - Turn Left, D - Turn Right, S - Land");
                while(true){
                    ConsoleKey key = Console.ReadKey(true).Key;
                    robot.Command(key);
                    if(!robot.CheckIsLanded()){
                        Console.WriteLine("Please land the robot");
                        continue;
                    }
                    robot.Logging();
                }
            }
            else{
                Console.WriteLine("Invalid input");
            }
        }
    }

    internal class Robot {
        int x;
        int y;
        Directions  direction;

        bool isLanded;
        int map;

        public Robot(int x, int y, Directions direction, int map){
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.isLanded = false;
            this.map = map;
        }

        public void Command(ConsoleKey key){
            switch (key){
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
        
        public void Move(){
            switch (this.direction){
                case Directions.NORTH:
                    if(this.y == this.map){
                        return;
                    }
                    this.y++;
                    break;
                case Directions.EAST:
                    if(this.x == this.map){
                        return;
                    }
                    this.x++;
                    break;
                case Directions.SOUTH:
                    if(this.y == 0){
                        return;
                    }
                    this.y--;
                    break;
                case Directions.WEST:
                    if(this.x == 0){
                        return;
                    }
                    this.x--;
                    break;
            }
        }

        public void TurnLeft(){
            this.direction = (Directions)((int)this.direction - 1);
            if((int)this.direction < 1){
                this.direction = Directions.WEST;
            }
        }
        public void TurnRight(){
            this.direction = (Directions)((int)this.direction + 1);
            if((int)this.direction > 4){
                this.direction = Directions.NORTH;
            }
        }
        public void Land(){
            if(!this.isLanded){
                this.isLanded = true;
            }
            return;
        }
        public void Logging(){
            Console.WriteLine($"Robot posistion: X: {this.x}, Y: {this.y}, Direction: {this.direction}");
        }

        public bool CheckIsLanded(){
            return this.isLanded;
        }
    }
}