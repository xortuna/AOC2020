using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong thirtySixBitMask = 68719476735;
            ulong currentMask = 0x00;    //1100
            ulong activeMask = 0x00; //0110 parts of the current mask to apply

            ulong[] ram = new ulong[100000];
            using(StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    if(line.StartsWith("mask = "))
                    {
                        foreach (var c in line.Substring("mask = ".Length))
                        {
                            activeMask = activeMask << 1;
                            if(c != 'X')    //C# wont let me |= for some reason
                                activeMask |= 1;
                            activeMask = activeMask & thirtySixBitMask; // trim
                            currentMask = currentMask << 1;
                            if (c == '1')
                                currentMask |= 1;
                            currentMask = currentMask & thirtySixBitMask; // trim
                        }
                    }
                    else if(line.StartsWith("mem["))
                    {
                        var eb = line.IndexOf("]");
                        int memoryIndex = int.Parse(line.Substring(4, eb - 4));
                        ulong value = ulong.Parse(line.Substring(eb+4));

                        //Postive combine - every 1 and applied gets forced to 1
                        var postiveMask = currentMask & activeMask;
                        value |= postiveMask;

                        //Negative override - everything 0 and active gets forced to 0
                        var negativeMask = ~((~currentMask) & activeMask);
                        value &= negativeMask;
                        ram[memoryIndex] = value;
                    }
                }
            }
            ulong total = 0;
            foreach(var l in ram)   //Linq Sum does not work with ulong :(
                total += l;

            Console.WriteLine($"Part 1: {total}");

            currentMask = 0x00;
            ulong floatingMask = 0x00;

            Dictionary<string, long> ramP2 = new Dictionary<string, long>();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("mask = "))
                    {
                        foreach (var c in line.Substring("mask = ".Length))
                        {
                            floatingMask = floatingMask << 1;
                            if (c == 'X')
                                floatingMask |= 1;
                            floatingMask = floatingMask & thirtySixBitMask; // trim to 36bits
                            currentMask = currentMask << 1;
                            if (c == '1')
                                currentMask |= 1;
                            currentMask = currentMask & thirtySixBitMask; // trim to 36bits
                        }
                    }
                    else if (line.StartsWith("mem["))
                    {
                        var eb = line.IndexOf("]");
                        ulong memoryIndex = ulong.Parse(line.Substring(4, eb - 4));
                        long value = long.Parse(line.Substring(eb + 4));

                        memoryIndex = memoryIndex | currentMask;
                        foreach(ulong memoryAddress in StartFindPermutations(memoryIndex, floatingMask))
                        {
                            var dictionaryKey = Convert.ToString((long)memoryAddress, 2);
                            ramP2[dictionaryKey] = value;
                        }
                    }
                }
            }
            Console.WriteLine($"Part 2: {ramP2.Sum(t => t.Value)}");
        }

        static IEnumerable<ulong> StartFindPermutations(ulong value, ulong mask)
        {
            for (int i = 0; i < 36; i++)
            {
                if (((mask >> i) & 0x01) == 1)
                {
                    foreach (var p in FindPermutations(value, mask, i))
                        yield return p;
                    break;
                }
            }
        }
        static IEnumerable<ulong> FindPermutations(ulong value, ulong mask, int pointer)
        {
            ulong switchMask = 0x01UL << pointer;
            ulong path1 = value | switchMask;
            ulong path2 = value & (~switchMask);

            //are there any children?
            bool isLastNode = true;
            for (int i = pointer+1; i < 36; i++)
            {
                if(((mask >> i) & 0x01) == 1)
                {
                    foreach (var p in FindPermutations(path1, mask, i))
                        yield return p;

                    foreach (var p in FindPermutations(path2, mask, i))
                        yield return p;
                    isLastNode = false;
                    break;
                }
            }
            if(isLastNode)
            {
                yield return path1;
                yield return path2;
            }
        }
    }
}
