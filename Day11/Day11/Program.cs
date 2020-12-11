using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            List<List<char>> mapP1 = new List<List<char>>();
            List<List<char>> mapP2 = new List<List<char>>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    mapP1.Add(new List<char>(line.AsEnumerable()));
                    mapP2.Add(new List<char>(line.AsEnumerable()));
                }
            }

            PrintMap(mapP1);
            while (true)
            {
                List<List<char>> clone = new List<List<char>>();
                for (int y = 0; y < mapP1.Count; ++y)
                {
                    clone.Add(new List<char>());
                    for (int x = 0; x < mapP1[y].Count; ++x)
                        clone[y].Add(ApplyRule1(mapP1, x, y));
                }
                if (clone.SelectMany(t => t).SequenceEqual(mapP1.SelectMany(t => t)))
                {
                    Console.WriteLine($"Part 1 Found: {clone.SelectMany(t => t).Count(r=> r=='#')}");
                    break;
                }
                mapP1 = clone;
            }

            //Part 2
            while (true)
            {
                List<List<char>> clone = new List<List<char>>();
                for (int y = 0; y < mapP2.Count; ++y)
                {
                    clone.Add(new List<char>());
                    for (int x = 0; x < mapP2[y].Count; ++x)
                        clone[y].Add(ApplyRule2(mapP2, x, y));
                }
                if (clone.SelectMany(t => t).SequenceEqual(mapP2.SelectMany(t => t)))
                {
                    Console.WriteLine($"Part 2 Found: {clone.SelectMany(t => t).Count(r => r == '#')}");
                    break;
                }
                mapP2 = clone;
            }

            Console.ReadLine();

        }
        static void PrintMap(List<List<char>> map)
        {
            for (int y = 0; y < map.Count; ++y)
            {
                for (int x = 0; x < map[y].Count; ++x)
                    Console.Write(map[y][x]);
                Console.WriteLine();
            }
            Console.WriteLine("---");
        }
        static char ApplyRule1(List<List<char>> map, int xpos, int ypos)
        {
            switch (map[ypos][xpos])
            {
                case 'L':
                    if (NumOccupiedAjacent(map, xpos, ypos) == 0)
                        return '#';
                    break;
                case '#':
                    if (NumOccupiedAjacent(map, xpos, ypos) >= 4)
                        return 'L';
                    break;
            }
            return map[ypos][xpos];
        }
        static char ApplyRule2(List<List<char>> map, int xpos, int ypos)
        {
            switch (map[ypos][xpos])
            {
                case 'L':
                    if (NumOccupiedLos(map, xpos, ypos) == 0)
                        return '#';
                    break;
                case '#':
                    if (NumOccupiedLos(map, xpos, ypos) >= 5)
                        return 'L';
                    break;
            }
            return map[ypos][xpos];
        }

        private static int NumOccupiedAjacent(List<List<char>> map, int xpos, int ypos)
        {
            int occupiedSeats = 0;
            for (int tY = Math.Max(0, ypos - 1); tY < Math.Min(ypos + 2, map.Count); ++tY)
            {
                for (int tX = Math.Max(0, xpos - 1); tX < Math.Min(xpos + 2, map[tY].Count); ++tX)
                {
                    if ((tX != xpos || tY != ypos) && map[tY][tX] == '#')
                    {
                        occupiedSeats++;
                    }
                }
            }
            return occupiedSeats;
        }
        private static int NumOccupiedLos(List<List<char>> map, int xpos, int ypos)
        {
            int occupiedSeats = 0;

            //Cardinal directions
            for (int xStep = -1; xStep < 2; ++xStep)
            {
                for (int yStep = -1; yStep < 2; ++yStep)
                {
                    if (xStep == 0 && yStep == 0)
                        continue;

                    int tX = xpos + xStep;
                    int tY = ypos + yStep;
                    while (InRange(map, tY, tX))
                    {
                        if (map[tY][tX] != '.')
                        {
                            if (map[tY][tX] == '#')
                                occupiedSeats++;
                            break;
                        }
                        tX += xStep;
                        tY += yStep;
                    }
                }
            }

            return occupiedSeats;
        }

        private static bool InRange(List<List<char>> map, int ypos, int xpos)
        {
            return ypos >= 0 && xpos >= 0 && ypos < map.Count() && xpos < map[ypos].Count;
        }
    }
}
