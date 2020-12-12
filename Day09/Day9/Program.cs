using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            List<long> uncullednumbers= new List<long>();
            List<long> numbers = new List<long>();
            const int PreambleLength = 25;
            long weakNo = 0;
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;

                while((line = sr.ReadLine()) != null)
                {
                    var target = long.Parse(line);

                    if (numbers.Count == PreambleLength)
                    {
                        if(!MakeTo(numbers, target))
                        {
                            Console.WriteLine($"Part 1: Could not make {target}");
                            weakNo = target;
                            break;
                        }
                        numbers.RemoveAt(0);
                    }
                    numbers.Add(target);
                    uncullednumbers.Add(target);
                }
            }

            var set = ContinuiousSet(uncullednumbers, weakNo);

            Console.WriteLine($"Part 2: Min {set.Min()} Max {set.Max()} Sum: {set.Min() + set.Max()}");
            Console.ReadLine();
        }

        private static bool MakeTo(List<long> numbers , long target)
        {
            for(int i =0; i < numbers.Count; ++i)
            {
                for(int j = i+1; j < numbers.Count; ++j)
                {
                    if (numbers[i] + numbers[j] == target)
                        return true;
                }
            }
            return false;
        }
        private static List<long> ContinuiousSet(List<long> numbers, long target)
        {
            for (int i = 0; i < numbers.Count; ++i)
            {
                long set = 0;
                for (int j = i; j < numbers.Count; ++j)
                {
                    set += numbers[j];
                    if(set == target)
                        return numbers.GetRange(i, j - i);
                    if (set > target)
                        break;
                }
            }
            return Enumerable.Empty<long>().ToList();
        }
    }
}
