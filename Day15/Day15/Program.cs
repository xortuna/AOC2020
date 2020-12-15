using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> input = new List<int> { 0, 14, 1, 3, 7, 9 };

            for (int i = input.Count; i < 2020; ++i)
            {
                var lastNo = input[i - 1];
                var lastSpoken = input.LastIndexOf(lastNo, input.Count - 2);

                if (lastSpoken == -1)
                    input.Add(0);
                else
                    input.Add((i - 1) - lastSpoken);
            }

            Console.WriteLine($"Part 1: {input.Last()}");

            Dictionary<int, int> inputD = new Dictionary<int, int> { { 0, 0 }, { 14, 1 }, { 1, 2 },{ 3, 3}, { 7, 4 }, };
            int lastSpoke = 9;
            for (int i = inputD.Count+1; i < 30000000; ++i)
            {
                if (!inputD.ContainsKey(lastSpoke))
                {
                    inputD[lastSpoke] = i-1;
                    lastSpoke = 0;
                }
                else
                {
                    var toSpeak = (i - 1) - inputD[lastSpoke];
                    inputD[lastSpoke] = i-1;
                    lastSpoke = toSpeak;
                }
            }
            Console.WriteLine($"Part 2: {lastSpoke}");
            Console.ReadLine();
        }
    }
}
