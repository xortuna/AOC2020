using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#define PART2 1
namespace Day4
{
    class MinMaxValidator : IValidationRule
    {
        public int Min;
        public int Max;

        public bool Validate(string input)
        {
            if (int.TryParse(input, out int n))
                if (n >= Min && n <= Max)
                    return true;
            return false;
        }
    }

    class ListValidator : IValidationRule
    {
        public List<string> ValidItems;
        public bool Validate(string input)
        {
            if (ValidItems.Contains(input))
                return true;
            return false;
        }
    }

    class RegexValidator : IValidationRule
    {
        public Regex FieldRegex;
        public bool Validate(string input)
        {
            return FieldRegex.IsMatch(input);
        }
    }

    class HeightValidator : IValidationRule
    {
        public int MinCm;
        public int MaxCm;
        public int MinIn;
        public int MaxIn;
        public bool Validate(string input)
        {
            if (int.TryParse(input.Substring(0, input.Length - 2), out int n))
            {
                return ((input.EndsWith("cm") && n >= MinCm && n <= MaxCm) || (input.EndsWith("in") && n >= MinIn && n <= MaxIn));
            }
            return false;
        }
    }

    interface IValidationRule
    {
        bool Validate(string input);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Dictionary<string, IValidationRule> requirements = new Dictionary<string, IValidationRule>
            {
                { "byr", new MinMaxValidator{Min =1920, Max=2002} },
                { "iyr", new MinMaxValidator{Min=2010, Max=2020} },
                { "eyr", new MinMaxValidator{Min=2020, Max=2030} },
                { "hgt", new HeightValidator{MinCm=150, MaxCm=193, MinIn = 59, MaxIn=76} },
                { "hcl", new RegexValidator{ FieldRegex = new Regex(@"^#[0-9a-f]{6}$")} },
                { "ecl", new ListValidator{ValidItems = "amb blu brn gry grn hzl oth".Split(' ').ToList()} },
                { "pid", new RegexValidator{ FieldRegex = new Regex(@"^[0-9]{9}$")} }
                //"cid"
            };

            int validCount = 0;
            int invalidCount = 0;
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                Dictionary<string, bool> elementsNeeded = GenerateValidationDictionary(requirements.Keys);
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        //validate passport
                        if (elementsNeeded.All(t => t.Value))
                            validCount++;
                        else
                            invalidCount++;
                        elementsNeeded = GenerateValidationDictionary(requirements.Keys);
                    }
                    else
                    {
                        var elements = line.Split(' ');
                        foreach (var element in elements)
                        {

                            var t = element.IndexOf(':');
                            var field = element.Substring(0, t);
                            var value = element.Substring(t + 1);

                            if (elementsNeeded.ContainsKey(field))
                            {
#if PART2
                                if (requirements[field].Validate(value))
#endif
                                    elementsNeeded[field] = true;
                            }
                        }
                    }
                }
                if (elementsNeeded.All(t => t.Value))
                    validCount++;
                else
                    invalidCount++;

                Console.WriteLine($"Valid {validCount} Invalid {invalidCount}");
                Console.ReadLine();
            }
        }

        static Dictionary<string, bool> GenerateValidationDictionary(IEnumerable<string> requirements)
        {
            return new Dictionary<string, bool>(requirements.Select(t=> new KeyValuePair<string, bool>(t, false)));
        }
    }
}


