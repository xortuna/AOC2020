using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day16
{
    class Program
    {
        class MinMaxValidator : IValidationRule
        {
            public int Min;
            public int Max;

            public bool Validate(int n)
            {
                return (n >= Min && n <= Max);
            }
        }
        class StackedMinMax : IValidationRule
        {
            public string FieldName;
            public List<MinMaxValidator> List = new List<MinMaxValidator>();
            public bool Validate(int input)
            {
                return List.Any(t => t.Validate(input));
            }
        }

        interface IValidationRule
        {
            bool Validate(int input);
        }

        static void Main(string[] args)
        {
            Regex ruleParser = new Regex(@"^(.+):\s(\d+)-(\d+)\sor\s(\d+)-(\d+)$");
            Console.WriteLine("Hello World!");
            List<StackedMinMax> requirements = new List<StackedMinMax>();

            List<int> myTicket = new List<int>();
            List<List<int>> nearbyTickets = new List<List<int>>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                bool captureMyTicket = false;
                bool captureNearbyTickets = false;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if(captureMyTicket)
                        {
                            myTicket = line.Split(',').Select(t => int.Parse(t)).ToList();
                            captureMyTicket = false;
                        }
                        else if(captureNearbyTickets)
                        {
                            nearbyTickets.Add(line.Split(',').Select(t => int.Parse(t)).ToList());
                        }
                        else if(line.StartsWith("your ticket:"))
                        {
                            captureMyTicket = true;
                        }
                        else if(line.StartsWith("nearby tickets:"))
                        {
                            captureNearbyTickets = true;
                        }
                        else
                        {
                            var l = ruleParser.Match(line);
                            if (!l.Success)
                                throw new Exception($"Failed to parse {line}");

                            StackedMinMax smm = new StackedMinMax()
                            {
                                FieldName = l.Groups[1].Value,
                                List = new List<MinMaxValidator>()
                                {
                                    new MinMaxValidator(){Min = int.Parse(l.Groups[2].Value), Max = int.Parse(l.Groups[3].Value)},
                                    new MinMaxValidator(){Min = int.Parse(l.Groups[4].Value), Max = int.Parse(l.Groups[5].Value)}
                                }
                            };
                            requirements.Add(smm);
                        }
                    }
                }

                int part1Total = 0;
                List<List<int>> ticketsToDiscard = new List<List<int>>();
                foreach(var ticket in nearbyTickets)
                {
                    foreach(var v in ticket)
                    {
                        if(requirements.All(t=> !t.Validate(v)))
                        {
                            Console.WriteLine($"Number: {v} is invalid for all rules");
                            part1Total += v;
                            ticketsToDiscard.Add(ticket);
                            break;
                        }
                    }
                }
                nearbyTickets.RemoveAll(t=> ticketsToDiscard.Contains(t));
                ticketsToDiscard.Clear();
                Console.WriteLine($"Part1: {part1Total}");


                //Part 2
                List<List<StackedMinMax>> fieldMap = new List<List<StackedMinMax>>();
                for (int i = 0; i < myTicket.Count; ++i)
                {
                    fieldMap.Add(requirements.ToList());    //Add all possible mappings
                }


                while (fieldMap.Any(t=>t.Count != 1))
                {
                    for (int i = 0; i < myTicket.Count; ++i)
                    {
                        foreach (var ticket in nearbyTickets.Concat(new List<List<int>> { myTicket }))
                        {
                            fieldMap[i].RemoveAll(t => !t.Validate(ticket[i]));
                        }
                        if (fieldMap[i].Count == 1)
                        {
                            var toDeleteFromEveryone = fieldMap[i].First();
                            foreach(var spreadDelete in fieldMap.Where(t=>t != fieldMap[i]))
                            {
                                spreadDelete.Remove(toDeleteFromEveryone);  //knock out possible mappings from neighbors
                            }
                        }
                    }
                }
                Console.WriteLine(string.Join(",", fieldMap.Select(t=>t.First().FieldName)));

                long total = 1;
                for(int i=0; i< myTicket.Count; ++i)
                {
                    if(fieldMap[i].First().FieldName.StartsWith("departure"))
                    {
                        total *= myTicket[i];
                        Console.WriteLine($"{fieldMap[i].First().FieldName} = {myTicket[i]}");
                    }
                }
                Console.WriteLine($"Part 2: {total}");
                Console.ReadLine();
            }
        }
    }
}
