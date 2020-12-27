using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day24
{
    class Program
    {
        class Point2D
        {
            public Point2D(int row, int column)
            {
                this.Row = row;
                this.Column = column;
            }
            public int Row;
            public int Column;
            public override int GetHashCode()
            {
                return Row.GetHashCode() + Column.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if(obj is Point2D)
                {
                    return ((Point2D)obj).Column == this.Column && ((Point2D)obj).Row == this.Row;
                }
                return base.Equals(obj);
            }
        }
        enum HexDirection
        {
            East,
            SouthEast,
            SouthWest,
            West,
            NorthWest,
            NorthEast
        }

        class HexTile
        {
            public Point2D Position;
            public bool White = true;
            public HexTile(Point2D point)
            {
                Position = point;
            }

            public Point2D Move(HexDirection direction)
            {
                switch(direction )
                {
                    case HexDirection.East:
                        return new Point2D(Position.Row, Position.Column  + 1);
                    case HexDirection.SouthEast:
                        return new Point2D(Position.Row + 1, Position.Column + 1);
                    case HexDirection.SouthWest:
                        return new Point2D(Position.Row + 1, Position.Column);
                    case HexDirection.West:
                        return new Point2D(Position.Row, Position.Column - 1);
                    case HexDirection.NorthWest:
                        return new Point2D(Position.Row - 1, Position.Column - 1);
                    case HexDirection.NorthEast:
                        return new Point2D(Position.Row  - 1, Position.Column);
                    default:
                        return new Point2D(Position.Row, Position.Column);
                }
            }
        }

        static void Main(string[] args)
        {
            Dictionary<Point2D, HexTile> Tiles = new Dictionary<Point2D, HexTile>();
            Point2D curentPosition = new Point2D(0, 0);
            Tiles.Add(curentPosition, new HexTile(curentPosition));
            foreach(var line in File.ReadAllLines("puzzleinput.txt"))
            {
                curentPosition = new Point2D(0, 0);
                var lastChar = '0';
                HexDirection? dir = null;
                foreach (var c in line)
                {
                    dir = null;
                    if(c == 'n' || c == 's')
                    {
                        lastChar = c;
                    }
                    else
                    {
                        if (lastChar == 's')
                        {
                            if (c == 'e')
                                dir = HexDirection.SouthEast;
                            else
                                dir = HexDirection.SouthWest;
                            lastChar = '0';
                        }
                        else if (lastChar == 'n')
                        {
                            if (c == 'e')
                                dir = HexDirection.NorthEast;
                            else
                                dir = HexDirection.NorthWest;
                            lastChar = '0';
                        }
                        else
                        {
                            if (c == 'e')
                                dir = HexDirection.East;
                            else
                                dir = HexDirection.West;
                        }
                    }

                    if(dir.HasValue)
                    {
                        var ctile = Tiles[curentPosition];
                        curentPosition = ctile.Move(dir.Value);
                        if (!Tiles.ContainsKey(curentPosition))
                        { 
                            Tiles.Add(curentPosition, new HexTile(curentPosition));
                        }
                    }
                }
                Tiles[curentPosition].White = !Tiles[curentPosition].White;
            }

            Console.WriteLine($"Part 1 {Tiles.Count(t=> !t.Value.White)}");


            //Living tile
            for(int i= 0; i < 100; ++i)
            {
                Dictionary<Point2D, HexTile> copy = new Dictionary<Point2D, HexTile>();
                foreach (var tile in Tiles.Values)
                {
                    bool setToWhite = tile.White;
                    if(tile.White)
                    {
                        if (CountAjacentBlacks(tile, Tiles) == 2)
                            setToWhite = false;
                    }
                    else
                    {
                        if (CountAjacentBlacks(tile, Tiles) > 2 || CountAjacentBlacks(tile, Tiles) == 0)
                            setToWhite = true;

                        for(int j=0; j < 6; ++j)
                        {
                            var tPos = tile.Move((HexDirection)j);
                            if(!Tiles.ContainsKey(tPos) && !copy.ContainsKey(tPos))
                            {
                                var subTile = new HexTile(tPos);
                                if(CountAjacentBlacks(subTile, Tiles) == 2)
                                {
                                    subTile.White = false;
                                    copy.Add(subTile.Position, subTile);
                                }
                            }
                        }
                    }
                    copy.Add(tile.Position, new HexTile(tile.Position) { White = setToWhite });
                }
                Console.WriteLine($"{i}: {copy.Count(t => !t.Value.White)}");
                Tiles = copy;
            }
            Console.WriteLine($"Part 2 {Tiles.Count(t => !t.Value.White)}");

            Console.ReadLine();
        }

        private static int CountAjacentBlacks(HexTile tile, Dictionary<Point2D, HexTile> tiles)
        {
            int count = 0;

            for (int j = 0; j < 6; ++j)
                count += IsBlack(tile.Move((HexDirection)j), tiles); 
            return count;
        }

        private static int IsBlack(Point2D point2D, Dictionary<Point2D, HexTile> tiles)
        {
            if (tiles.ContainsKey(point2D) && !tiles[point2D].White)
                return 1;
            return 0;
        }
    }
}
