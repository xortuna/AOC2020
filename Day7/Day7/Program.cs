using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day7
{
    class Program
    {
        public class Bag
        {
            public bool Visited = false;
            public string Color = null;
            public List<KeyValuePair<string, int>> CanHold = new List<KeyValuePair<string, int>>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Regex containRegex = new Regex(@"^([\w|\s]+) bags contain");
            Regex noOfBags = new Regex(@"(\d+)\s([\s\w]+)\sbag(s?)");
            List<Bag> bagSet = new List<Bag>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                while((line = sr.ReadLine()) != null)
                {
                    var match = containRegex.Match(line);
                    if (match.Success)
                    {
                        Bag newBag = new Bag();
                        newBag.Color = match.Groups[1].Value;

                        line = line.Substring(match.Length);
                        foreach (var subElement in line.Split(','))
                        {
                            var subMatch = noOfBags.Match(subElement);
                            if (subMatch.Success)
                            {
                                newBag.CanHold.Add(new KeyValuePair<string, int>(subMatch.Groups[2].Value, int.Parse(subMatch.Groups[1].Value)));
                            }
                        }
                        bagSet.Add(newBag);

                    }
                    else
                        Console.WriteLine($"Could not parse {line}");

                }


                var shinyGoldBags = StartPackColor("shiny gold", bagSet);
                var totalBags = RecursiveCount("shiny gold", bagSet);
                Console.WriteLine($"Part 1: {shinyGoldBags}");
                Console.WriteLine($"Part 2: {totalBags}");
                Console.ReadLine();
            }
        }

        public static int StartPackColor(string color, List<Bag> bagset)
        {
            var b = bagset.Where(t => t.Color != color).ToList();
            PackColor(color, b);
            return b.Count(t => t.Visited);
        }

        public static void PackColor(string color, List<Bag> bagset)
        {
            foreach(var b in bagset)
            {
                if(!b.Visited && b.CanHold.Any(t=>t.Key == color))
                {
                    b.Visited = true;
                    PackColor(b.Color, bagset);
                }
            }
        }

        public static int RecursiveCount(string color, List<Bag> bagset)
        {
            var bag = bagset.FirstOrDefault(t => t.Color == color);

            int contains = 0;
            foreach(var subBag in bag.CanHold)
            {
                //This bag
                contains += 1 * subBag.Value;
                //This bags contents
                contains += RecursiveCount(subBag.Key, bagset) * subBag.Value; 
            }
            return contains;
        }
    }

}
