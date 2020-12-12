using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Regex policyRegex = new Regex(@"^(\d+)-(\d+)\s(\w):\s(\w*)$");
            //Part 1
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int validCount = 0;
                int invalidCount = 0;
                while((line = sr.ReadLine()) != null)
                {
                    var m = policyRegex.Match(line);
                    if (!m.Success)
                        Console.WriteLine($"Could not undestand {line}");

                    var min = int.Parse(m.Groups[1].Value);
                    var max = int.Parse(m.Groups[2].Value);
                    var c = m.Groups[3].Value[0];
                    var password = m.Groups[4].Value;

                    if (password.Count(t => t == c) < min || password.Count(t => t == c) > max)
                    {
                        invalidCount++;
                        Console.WriteLine($"Invalid password: {password} - from: {line}");
                    }
                    else
                    {
                        validCount++;
                    }
                }
                Console.WriteLine($"Part 1: Invalid {invalidCount} Valid {validCount}");
                Console.ReadLine();
            }
            //part 2
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int validCount = 0;
                int invalidCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    var m = policyRegex.Match(line);
                    if (!m.Success)
                        Console.WriteLine($"Could not undestand {line}");

                    var pos1 = int.Parse(m.Groups[1].Value) -1;
                    var pos2 = int.Parse(m.Groups[2].Value) - 1;
                    var c = m.Groups[3].Value[0];
                    var password = m.Groups[4].Value;

                    int matchCount = 0;
                    if (pos1 < password.Length && password[pos1] == c)
                        matchCount++;
                    if (pos2 < password.Length && password[pos2] == c)
                        matchCount++;

                    if(matchCount == 1)
                        validCount++;
                    else
                    {
                        invalidCount++;
                        Console.WriteLine($"Invalid password: {password} - from: {line}");
                    }
                }
                Console.WriteLine($"Part 2:  Invalid {invalidCount} Valid {validCount}");
                Console.ReadLine();
            }
        }
    }
}
