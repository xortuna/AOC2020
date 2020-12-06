using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            int planeRows = 128;
            int planeColumns = 8;
            List<int> foundSeats = new List<int>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int highestSeat = -1;
                while((line = sr.ReadLine()) != null)
                {
                    var pos = GetSeatPosition(line, planeRows, planeColumns);
                    int seatCode = (pos.Item1 * 8) + pos.Item2;
                    foundSeats.Add(seatCode);
                    highestSeat = Math.Max(seatCode, highestSeat);
                }
                Console.WriteLine($"Part 1: Highest seat: {highestSeat}");

                foundSeats.Sort();

                var mySeat = foundSeats.FirstOrDefault(t => !foundSeats.Contains(t + 1) && foundSeats.Contains(t + 2)) + 1;

                Console.WriteLine($"Part 2: My seat: {mySeat}");
            }
            Console.ReadLine();
        }

        static Tuple<int,int> GetSeatPosition(string line, int rows, int colums)
        {
            int rowH = rows, rowL = 0;
            int columH = colums, columL = 0;
            for (int i = 0; i < line.Length; ++i)
            {
                if (i < 7) //row
                {
                    var midPoint = (rowH + rowL) / 2;
                    if (line[i] == 'F') //lower
                        rowH = midPoint;
                    else if (line[i] == 'B')
                        rowL = midPoint;
                    else
                        Console.WriteLine($"Unreconised char {line[i]}");
                }
                else //column 
                {
                    var midPoint = (columH + columL) / 2;
                    if (line[i] == 'L') //lower
                        columH = midPoint;
                    else if (line[i] == 'R')
                        columL = midPoint;
                    else
                        Console.WriteLine($"Unreconised char {line[i]}");
                }
            }

            return new Tuple<int, int>(rowL, columL);
        }
    }
}
