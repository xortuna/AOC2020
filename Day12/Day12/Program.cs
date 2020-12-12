using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    class Program
    {
        class Waypoint
        {
            public int x = 10;
            public int y = -1;

            public void Move(Direction direction, int count)
            {
                switch (direction)
                {
                    case Direction.N:
                        y -= count;
                        break;
                    case Direction.E:
                        x += count;
                        break;
                    case Direction.W:
                        x -= count;
                        break;
                    case Direction.S:
                        y += count;
                        break;

                }

            }
            public void Turn(int amount)
            {
                while (amount < 0)
                    amount += 360;
                while (amount > 0)
                {
                    int cacheX = x;
                    this.x = -this.y;
                    this.y = cacheX;
                    amount -= 90;
                }
            }
        }

        class Ship
        {
            public int x = 0;
            public int y = 0;
            public Direction Orientation = Direction.E;

            public void Move(Direction direction, int count)
            {
                switch (direction)
                {
                    case Direction.N:
                        y -= count;
                        break;
                    case Direction.E:
                        x += count;
                        break;
                    case Direction.S:
                        y += count;
                        break;
                    case Direction.W:
                        x -= count;
                        break;
                }

            }
            public void Turn(int amount)
            {
                while (amount < 0)
                    amount += 360;
                while (amount > 0)
                {
                    Orientation = (Direction)((((int)Orientation) + 1) % 4);
                    amount -= 90;
                }
            }
        }
        enum Direction
        {
            N,
            E,
            S,
            W
        }

        static void Main(string[] args)
        {
            Ship p1Ship = new Ship();
            Ship p2Ship = new Ship();
            Waypoint waypoint = new Waypoint();

            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    int amount = int.Parse(line.Substring(1));
                    switch (line[0])
                    {
                        case 'N':
                            p1Ship.Move(Direction.N, amount);
                            waypoint.Move(Direction.N, amount);
                            break;
                        case 'S':
                            p1Ship.Move(Direction.S, amount);
                            waypoint.Move(Direction.S, amount);
                            break;
                        case 'E':
                            p1Ship.Move(Direction.E, amount);
                            waypoint.Move(Direction.E, amount);
                            break;
                        case 'W':
                            p1Ship.Move(Direction.W, amount);
                            waypoint.Move(Direction.W, amount);
                            break;
                        case 'F':
                            p1Ship.Move(p1Ship.Orientation, amount);
                            p2Ship.x += waypoint.x *amount ;
                            p2Ship.y += waypoint.y * amount;
                            break;
                        case 'L':
                            p1Ship.Turn(-amount);
                            waypoint.Turn(-amount);
                            break;
                        case 'R':
                            p1Ship.Turn(amount);
                            waypoint.Turn(amount);
                            break;

                    }
                }
            }

            Console.WriteLine($"{p1Ship.x} {p1Ship.y} Part1: {Math.Abs(p1Ship.x) + Math.Abs(p1Ship.y)}");
            Console.WriteLine($"{p2Ship.x} {p2Ship.y} Part2: {Math.Abs(p2Ship.x) + Math.Abs(p2Ship.y)}");
            Console.ReadLine();
        }
    }
}
