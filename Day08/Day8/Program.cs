using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        class Instruction
        {
            public string Command;
            public int Paramater;
            public bool Visited = false;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            List<Instruction> instructions = new List<Instruction>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    var elements = line.Split(' ');
                    Instruction instruction = new Instruction()
                    {
                        Command = elements[0],
                        Paramater = int.Parse(elements[1])
                    };
                    instructions.Add(instruction);
                }
            }
            int part1 = RunOs(instructions, out bool throwaway);
            Console.WriteLine($"Part 1: {part1}");

            for(int i =0; i < instructions.Count; ++i)
            {
                //Flip
                if (instructions[i].Command == "jmp")
                    instructions[i].Command = "nop";
                else if (instructions[i].Command == "nop")
                    instructions[i].Command = "jmp";

                //Test
                int part2 = RunOs(instructions, out bool infinateLoop);
                if (!infinateLoop)
                {
                    Console.WriteLine($"Part 2: {part2} ");
                    break;
                }

                //Flip
                if (instructions[i].Command == "jmp")
                    instructions[i].Command = "nop";
                else if (instructions[i].Command == "nop")
                    instructions[i].Command = "jmp";
            }


            Console.ReadLine();
        }


        static int RunOs(List<Instruction> instructions, out bool looped)
        {
            int accumulator = 0;
            int instructionPointer = 0;
            instructions.ForEach(t => t.Visited = false);
            while (instructionPointer < instructions.Count && !instructions[instructionPointer].Visited)
            {
                instructions[instructionPointer].Visited = true;
                switch (instructions[instructionPointer].Command)
                {
                    case "acc":
                        accumulator += instructions[instructionPointer].Paramater;
                        break;
                    case "jmp":
                        instructionPointer += instructions[instructionPointer].Paramater;
                        continue;
                }


                instructionPointer++;
            }
            looped = (instructionPointer >= 0 && instructionPointer < instructions.Count);
            return accumulator;
        }
    }
}
