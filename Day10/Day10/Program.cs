using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Day10
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<int> adaptors = new List<int>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    adaptors.Add(int.Parse(line));

                }

            }

            adaptors.Add(0);
            adaptors.Add(adaptors.Max() + 3);
            adaptors.Sort();
            int oneVolt = 0;
            int threeVolts = 0; //Last adaptor is always 3
            for(int i=0; i < adaptors.Count-1; ++i)
            {
                var diff = adaptors[i + 1] - adaptors[i];
                if (diff == 1)
                    oneVolt++;
                else if (diff == 3)
                    threeVolts++;
            }

            Console.WriteLine($"Part 1: One V: {oneVolt} Three V: {threeVolts} Answer: {oneVolt * threeVolts}");

            bool capture = false;
            long total = 1;
            int startIndex = 0;
            for(int i=0; i < adaptors.Count-1; ++i)
            {
                int run = 0;
                for (int j = i + 1; j < adaptors.Count; ++j)
                {
                    if (adaptors[j] - adaptors[i] > 3)
                        break;
                    else
                        run++;
                }
                if(run > 1 && !capture)
                {
                    capture = true;
                    startIndex = i;
                }
                else if(run == 1 && capture)
                {
                    long peumenations = Explore(adaptors.GetRange(startIndex, i+2 - startIndex), 0);
                    total *= peumenations;
                    capture = false;
                }
            }

            Console.WriteLine($"Part 2: {total}");



            Console.ReadLine();
        }

        static long Explore(List<int> array, int index)
        {
            if (index == array.Count - 1)
                return 1;
            long count = 0;
            for (int j = index + 1; j < array.Count; ++j)
            {
                if (array[j] - array[index] > 3)
                    break;
                else
                    count += Explore(array, j);
            }
            return count;
        }
    }
}
