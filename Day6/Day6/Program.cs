using System;
using System.Collections.Generic;
using System.IO;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            using(StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int totalPostiveAnswers = 0;
                HashSet<char> positiveAnswers = new HashSet<char>();
                while ((line = sr.ReadLine()) != null)
                {
                    if(string.IsNullOrWhiteSpace(line))
                    {
                        totalPostiveAnswers += positiveAnswers.Count;
                        positiveAnswers.Clear();
                    }
                    else
                    {
                        foreach(var c in line)
                        {
                            positiveAnswers.Add(c);
                        }
                    }
                    
                }
                totalPostiveAnswers += positiveAnswers.Count;

                Console.WriteLine($"Part 1: Total answers {totalPostiveAnswers}");
            }

            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int totalPostiveAnswers = 0;
                HashSet<char> positiveAnswers = new HashSet<char>();
                bool firstPerson = true;
                while ((line = sr.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        totalPostiveAnswers += positiveAnswers.Count;
                        positiveAnswers.Clear();
                        firstPerson = true;
                    }
                    else
                    {
                        if(firstPerson)
                        {
                            foreach (var c in line)
                                positiveAnswers.Add(c);
                            firstPerson = false;
                        }
                        else
                        {
                            positiveAnswers.RemoveWhere(t => !line.Contains(t));
                        }
                    }

                }
                totalPostiveAnswers += positiveAnswers.Count;

                Console.WriteLine($"Part 2 Total answers {totalPostiveAnswers}");
                Console.ReadLine();
            }
        }


    }
}
