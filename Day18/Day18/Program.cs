using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day18
{
    class Program
    {

        enum Operator
        {
            None,
            Set,
            Add,
            Multiply
        }
        class OperationStack
        {
            public long lhs;
            public Operator oper; 
        }

        static void Main(string[] args)
        {
            long part1Total = 0;
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    part1Total += CalculateString(line);
                }
            }
            Console.WriteLine($"Part 1: {part1Total}");

            long part2Total = 0;
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = Reprecedence(line);
                    part2Total += CalculateString(line);
                }
            }
            Console.WriteLine($"Part 2: {part2Total}");

            Console.ReadLine();
        }
        static string Reprecedence(string input)
        {
            for(int i=0; i < input.Length; ++i)
            {
                if(input[i] == '+')
                {
                    //seek left
                    int commaCache = 0;
                    int l = i - 2;
                    for(; l >= 0; --l)
                    {
                        if (input[l] == ')')
                            commaCache++;
                        else if (input[l] == '(')
                        {
                            commaCache--;
                            if (commaCache <= 0)
                                break;
                        }
                        else if (input[l] == ' ' && commaCache == 0)
                            break;
                    }
                    //Seek right
                    commaCache = 0;
                    int r = i + 2;
                    for (; r < input.Length; ++r)
                    {
                        if (input[r] == ')')
                        {
                            commaCache--;
                            if (commaCache <= 0)
                                break;
                        }
                        else if (input[r] == '(')
                            commaCache++;
                        if (input[r] == ' ' && commaCache == 0)
                            break;
                    }

                    input = input.Insert(l+1, "(");
                    input = input.Insert(r+1, ")");
                    i++;
                }
            }
            return input;
        }

        static long CalculateString(string input)
        {

            StringBuilder builder = new StringBuilder();
            Stack<OperationStack> depth = new Stack<OperationStack>();
            long cache = 0;
            Operator currentOperator = Operator.Set;
            foreach (var c in input)
            {
                if (char.IsNumber(c))
                    builder.Append(c);
                else if (c == '+')
                {
                    PerformOperation(ref cache, builder.ToString(), currentOperator);
                    currentOperator = Operator.Add;
                    builder.Clear();
                }
                else if (c == '*')
                {
                    PerformOperation(ref cache, builder.ToString(), currentOperator);
                    currentOperator = Operator.Multiply;
                    builder.Clear();
                }
                else if (c == '(')
                {
                    depth.Push(new OperationStack { lhs = cache, oper = currentOperator });
                    currentOperator = Operator.Set;
                    cache = 0;
                    builder.Clear();
                }
                else if (c == ')')
                {
                    PerformOperation(ref cache, builder.ToString(), currentOperator);
                    var toRestore = depth.Pop();
                    switch (toRestore.oper)
                    {
                        case Operator.Add:
                            cache = toRestore.lhs + cache;
                            break;
                        case Operator.Multiply:
                            cache = toRestore.lhs * cache;
                            break;
                    }
                    currentOperator = Operator.None;
                    builder.Clear();
                }
            }
            PerformOperation(ref cache, builder.ToString(), currentOperator);
            return cache;
        }

        static void PerformOperation(ref long cache, string currentString, Operator operation)
        {
            switch (operation)
            {
                case Operator.Set:
                    cache = long.Parse(currentString);
                    break;
                case Operator.Add:
                    cache += long.Parse(currentString);
                    break;
                case Operator.Multiply:
                    cache *= long.Parse(currentString);
                    break;
            }
        }
    }
}
