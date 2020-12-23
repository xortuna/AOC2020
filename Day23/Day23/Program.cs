using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day23
{
    class Program
    {
        static void Main(string[] args)
        {

            LinkedList<int> cups = new LinkedList<int>(new List<int> { 4, 6, 3, 5, 2, 8, 1, 7, 9 });
            DoCrabGame(cups, 100, true);
            Console.WriteLine($"Part 1:");
            var one = cups.Find(1);
            var n = one.Next;
            while(n != null)
            {
                Console.Write(n.Value);
                n = n.Next;
            }
            n = cups.First;
            while (n != one)
            {
                Console.Write(n.Value);
                n = n.Next;
            }
            cups = new LinkedList<int>(new List<int>() { 4, 6, 3, 5, 2, 8, 1, 7, 9 }.Concat(Enumerable.Range(10, 1000000 - 9)));
            DoCrabGame(cups, 10000000);

            Console.WriteLine($"Part 2: {cups.Find(1).Next.Value} * {cups.Find(1).Next.Next.Value} {cups.Find(1).Next.Value* (long)cups.Find(1).Next.Next.Value}");
            Console.ReadLine();
        }

        static void DoCrabGame(LinkedList<int> cups, int moves, bool debug = false)
        {
            int max = cups.Max();
            int min = cups.Min();

            LinkedListNode<int>[] nodeMap = new LinkedListNode<int>[max+1];
            LinkedListNode<int> lln = cups.First;
            while(lln != null)
            {
                nodeMap[lln.Value] = lln;
                lln = lln.Next;
            }
            for (int i = 0; i < moves; ++i)
            {
                if (debug)
                {
                    Console.WriteLine($"Move {i + 1}");
                    Console.WriteLine($"cups: {string.Join(",", cups)}");
                }
                var curerntCup = cups.First;

                var placementCups = new List<LinkedListNode<int>>{curerntCup.Next, curerntCup.Next.Next, curerntCup.Next.Next.Next };
                if (debug)
                {
                    Console.WriteLine($"pickup: {string.Join(",", placementCups)}");
                }
                foreach (var c in placementCups)
                    cups.Remove(c);
                int bestCup = curerntCup.Value;
                do
                {
                    bestCup--;
                    if (bestCup < min)
                        bestCup = max;

                }
                while (placementCups.Any(t=>t.Value == bestCup));

                if (debug)
                {
                    Console.WriteLine($"destination: {bestCup}");
                }
                LinkedListNode<int> n = nodeMap[bestCup];
                foreach (var c in placementCups)
                {
                    cups.AddAfter(n, c);
                    n = c;
                }

                cups.RemoveFirst();
                cups.AddLast(curerntCup);

            }
        }
    }
}
