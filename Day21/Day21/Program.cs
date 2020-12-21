using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day21
{
    class Program
    {
        class IngredentAllergenPair
        {
            public string Ingredent { get; set; }
            public string Allergen { get; set; }
        }
        static void Main(string[] args)
        {
            Regex parser = new Regex(@"([\w\s]+)\s\(contains ([\w\s,]+)\)");
            Dictionary<string, List<string>> AllergenIngredents = new Dictionary<string, List<string>>();
            foreach(var line in File.ReadAllLines("puzzleinput.txt"))
            {
                var m = parser.Match(line);
                if (m.Success)
                {
                    var ingredents = m.Groups[1].Value.Split(' ');
                    var allergens = m.Groups[2].Value.Split(',').Select(t=>t.TrimStart()).ToArray();

                    Stack<IngredentAllergenPair> ingredentsResolved = new Stack<IngredentAllergenPair>();
                    foreach(var allergen in allergens)
                    {
                        if (AllergenIngredents.ContainsKey(allergen))
                        {
                            //reductive filter
                            AllergenIngredents[allergen] = AllergenIngredents[allergen].Intersect(ingredents).ToList();
                            if(AllergenIngredents[allergen].Count() == 1)
                                ingredentsResolved.Push(new IngredentAllergenPair { Allergen = allergen, Ingredent = AllergenIngredents[allergen].First() });
                        }
                        else //Add all ingredients we know about and arn't already solved
                            AllergenIngredents.Add(allergen, ingredents.Where(i=> !AllergenIngredents.Values.Any(a=> a.Count == 1 && a.Contains(i))).ToList());
                    }

                    while(ingredentsResolved.Any())
                    {
                        var pop = ingredentsResolved.Pop();
                        foreach(var allergen in AllergenIngredents.Where(a => a.Key != pop.Allergen))
                        {
                            if(allergen.Value.Remove(pop.Ingredent) && allergen.Value.Count == 1)
                                ingredentsResolved.Push(new IngredentAllergenPair { Allergen = allergen.Key, Ingredent = AllergenIngredents[allergen.Key].First() });
                        }
                    }
                }
            }
            Debug.Assert(AllergenIngredents.All(t => t.Value.Count() == 1), "Could not solve all ingredents");

            int total = 0;
            var ingredentsThatHaveAllergents = AllergenIngredents.Select(t => t.Value.First()).ToList();
            foreach (var line in File.ReadAllLines("puzzleinput.txt"))
            {
                string ingredentSection = line.Substring(0, line.IndexOf('(') == -1 ? line.Length: line.IndexOf('(')).Trim();
                total += ingredentSection.Split(' ').Count(t => !ingredentsThatHaveAllergents.Contains(t));
            }

            Console.WriteLine($"Part 1: {total}");
            Console.WriteLine($"Part 2: {string.Join(",", AllergenIngredents.OrderBy(t => t.Key).Select(t => t.Value.First()))}");
            Console.ReadLine();
        }
    }
}