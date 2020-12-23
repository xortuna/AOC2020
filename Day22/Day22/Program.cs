using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22
{
    class Program
    {
        enum Winner
        {
            P1,
            P2
        }

        static void Main(string[] args)
        {
            long score = 0;
            Queue<int> p1 = new Queue<int>(), p2 = new Queue<int>();

            ReadFile(p1, p2);

            while(p1.Any() && p2.Any())
            {
                var p1v = p1.Dequeue();
                var p2v = p2.Dequeue();
                if(p1v > p2v)
                {
                    p1.Enqueue(p1v);
                    p1.Enqueue(p2v);
                }
                else
                {
                    p2.Enqueue(p2v);
                    p2.Enqueue(p1v);
                }
            }
            Queue<int> winner = p1.Any() ? p1 : p2;
            while (winner.Any())
                score += winner.Count * winner.Dequeue();

            Console.WriteLine($"Part 1 {score}");

            //Reset
            p1.Clear(); p2.Clear(); score = 0;
            ReadFile(p1, p2);

            DoGame(p1, p2);
            winner = p1.Any() ? p1 : p2;
            while (winner.Any())
                score += winner.Count * winner.Dequeue();

            Console.WriteLine($"Part 2 {score}");
            Console.ReadLine();
        }
        static void ReadFile(Queue<int> p1, Queue<int> p2)
        {
            Queue<int> filling = null;
            foreach (var lin in File.ReadAllLines("puzzleinput.txt").Where(t=> !string.IsNullOrEmpty(t)))
            {
                if (lin.StartsWith("Player"))
                    filling = (lin == "Player 1:") ? p1 : p2;
                else
                    filling.Enqueue(int.Parse(lin));
            }
        }

        static Winner DoGame(Queue<int> p1, Queue<int> p2)
        {
            HashSet<string> previousRounds = new HashSet<string>();
            while (p1.Any() && p2.Any())
            {
                Winner winner = Winner.P1;
                var encodedP1 = previousRounds.Add("p1" + string.Join(",", p1));
                var encodedP2 = previousRounds.Add("p2" + string.Join(",", p2));
                if (!encodedP2 || !encodedP2)
                    return winner;    //Player1 won

                var p1v = p1.Dequeue();
                var p2v = p2.Dequeue();
                if (p1.Count() >= p1v && p2.Count >= p2v)
                    winner = DoGame(new Queue<int>(p1.Take(p1v)), new Queue<int>(p2.Take(p2v)));
                else if (p1v < p2v)
                    winner = Winner.P2;

                if (winner == Winner.P1)
                {
                    p1.Enqueue(p1v);
                    p1.Enqueue(p2v);
                }
                else
                {
                    p2.Enqueue(p2v);
                    p2.Enqueue(p1v);
                }
            }
            return p1.Any() ? Winner.P1 : Winner.P2;
        }
    }
}