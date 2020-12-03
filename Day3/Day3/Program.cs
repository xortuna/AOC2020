using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<List<char>> terrain = new List<List<char>>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    terrain.Add(new List<char>(line.AsEnumerable()));
                }
            }

            //Part 1
            int tree = TraverseSlope(terrain, 3, 1);
            Console.WriteLine($"Part 1: Height {terrain.Count} Width {terrain.First().Count} Trees {tree}");
            Console.ReadLine();

            //Part 2
            int trees1 = TraverseSlope(terrain, 1, 1);
            int trees2 = TraverseSlope(terrain, 3, 1);
            int trees3 = TraverseSlope(terrain, 5, 1);
            int trees4 = TraverseSlope(terrain, 7, 1);
            int trees5 = TraverseSlope(terrain, 1, 2);

            long total = (long)trees1 * (long)trees2 * (long)trees3 * (long)trees4 * (long)trees5;
            Console.WriteLine($"Part 2: 1:{trees1} 2:{trees2} 3:{trees3} 4:{trees4} 5:{trees5}");
            Console.WriteLine($"Part 2: Answer:{total}");
            Console.ReadLine();
        }

        static int TraverseSlope(List<List<char>> terrain, int moveRight, int moveDown)
        {
            int posX = 0;
            int posY = 0;
            int tree = 0;
            do
            {
                posX = (posX + moveRight) % terrain.First().Count;
                posY = posY + moveDown;

                if (posY < terrain.Count)
                {
                    var c = terrain[posY][posX];
                    if (c == '#')
                        tree++;
                }
            }
            while (posY < terrain.Count);
            return tree;

        }
    }
}
