using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string target = sr.ReadLine();
                string buses = sr.ReadLine();

                int iTarget = int.Parse(target);
                int bestBus = 0;
                int bestRemainder = int.MaxValue;
                foreach (var bus in buses.Split(',').Where(t=>t != "x"))
                {
                    int increment = int.Parse(bus);

                    var remainder = increment - (iTarget % increment); //need to catch the 'next' bus - not the nearest bus, so calculate the shortest wait to next bus
                    if (remainder == increment) //Fix for the above formula's edge case when a bus arrives at the right time
                        remainder = 0;

                    if (remainder < bestRemainder)  //Calculate best time
                    {
                        bestRemainder = remainder;
                        bestBus = increment;
                    }
                }

                Console.WriteLine($"Part1: Wait: {bestRemainder} Bus {bestBus} Answer {bestRemainder * bestBus}");


                //Part two
                List<int[]> busList = new List<int[]>();
                var busarray = buses.Split(',');
                for (int i = 1; i < busarray.Count(); ++i)
                {
                    if (busarray[i] != "x")
                    {
                        var v = int.Parse(busarray[i]);
                        busList.Add(new int[] { i, v });
                    }
                }

                busList = busList.OrderByDescending(t => t[1]).ToList();
                long step = long.Parse(busarray[0]);
                long currentDepartTime = 0;
                int matching = 1;
                long? firstMatch = null;
                while (true)
                {
                    //Do we have a match?
                    if (busList.All(t => ((currentDepartTime + t[0]) % t[1] == 0)))
                    {
                        Console.WriteLine($"Part2: Timestamp: {currentDepartTime}");
                        break;
                    }
                    //Else try and improve the stepage
                    if (busList.GetRange(0,matching).All(t => ((currentDepartTime + t[0]) % t[1] == 0)))
                    {
                        if(!firstMatch.HasValue)
                        {
                            firstMatch = currentDepartTime;
                        }
                        else
                        {
                            step = currentDepartTime - firstMatch.Value;    //The occurrence will only appear at this minimal interval 
                            firstMatch = null;
                            matching++;
                            continue; //we may as well re-test our current step if it matches+1
                        }

                    }
                    currentDepartTime += step;

                }
                Console.ReadLine();
            }
        }
    }
}
