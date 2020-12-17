using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string, bool> Space = new Dictionary<string, bool>();

            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int y = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    for (int x = 0;  x < line.Length; ++x)
                    {
                        if (line[x] == '#')
                            Space[CoordToString(x, y, 0)] = true;
                    }
                    y++;
                }
            }


            for (int i = 0; i < 6; ++i)
            {
                Dictionary<string, bool> cloned = new Dictionary<string, bool>();

                foreach (var d in Space)
                {
                    int activeNeighbours = 0;
                    foreach (var neighbour in GetNeightbours(d.Key))
                    {
                        //Does this neighbor exist in our array, and can we affect it?
                        if (!Space.ContainsKey(neighbour))
                        {
                            if (d.Value)    //(Dumass optimisation) This node cannot contribute to the neighbours so ignore it
                            {
                                int subactive = 0;
                                foreach (var subNeighbour in GetNeightbours(neighbour))
                                {
                                    if (Space.ContainsKey(subNeighbour) && Space[subNeighbour])
                                        subactive++;
                                }
                                if (subactive == 3)
                                    cloned[neighbour] = true;
                            }
                        }
                        else
                        {
                            if(Space[neighbour])
                                activeNeighbours++;
                        }
                    }
                    if(d.Value)
                    {
                        if (activeNeighbours == 2 || activeNeighbours == 3)
                            cloned[d.Key] = true;
                    }
                    else
                        if(activeNeighbours == 3)
                            cloned[d.Key] = true;
                }
                Space = cloned;

                Console.WriteLine($"Cycle {i}: {Space.Count(t => t.Value)}");
            }
            Console.WriteLine($"Part 1: {Space.Count(t => t.Value)}");

            Space.Clear();
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                string line = null;
                int y = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    for (int x = 0; x < line.Length; ++x)
                    {
                        if (line[x] == '#')
                            Space[CoordToString4(x, y, 0, 0)] = true;
                    }
                    y++;
                }
            }


            for (int i = 0; i < 6; ++i)
            {
                Dictionary<string, bool> cloned = new Dictionary<string, bool>();

                foreach (var d in Space)
                {
                    int activeNeighbours = 0;
                    foreach (var neighbour in GetNeightbours4(d.Key))
                    {
                        //Does this neighbor exist in our array, and can we affect it?
                        if (!Space.ContainsKey(neighbour))
                        {
                            if (d.Value)    //(Dumass optimisation) This node cannot contribute to the neighbours so ignore it
                            {
                                int subactive = 0;
                                foreach (var subNeighbour in GetNeightbours4(neighbour))
                                {
                                    if (Space.ContainsKey(subNeighbour) && Space[subNeighbour])
                                        subactive++;
                                }
                                if (subactive == 3)
                                    cloned[neighbour] = true;
                            }
                        }
                        else
                        {
                            if (Space[neighbour])
                                activeNeighbours++;
                        }
                    }
                    if (d.Value)
                    {
                        if (activeNeighbours == 2 || activeNeighbours == 3)
                            cloned[d.Key] = true;
                    }
                    else
                        if (activeNeighbours == 3)
                            cloned[d.Key] = true;
                }
                Space = cloned;

                Console.WriteLine($"Cycle {i}: {Space.Count(t => t.Value)}");
            }
            Console.WriteLine($"Part 2: {Space.Count(t => t.Value)}");
            Console.ReadLine();
        }


        static string CoordToString(int x, int y, int z)
        {
            return x + "," + y + "," + z;
        }

        static IEnumerable<string> GetNeightbours(string coord)
        {
            var co = coord.Split(',').Select(t=> int.Parse(t)).ToList();

            for(int z = -1; z < 2; ++z)
            {
                for (int y = -1; y < 2; ++y)
                {
                    for (int x = -1; x < 2; ++x)
                    {
                        if (x == 0 && y == 0 && z == 0)
                            continue;

                        yield return CoordToString(co[0] + x, co[1] + y, co[2] + z);
                    }
                }
            }
        }
        static string CoordToString4(int x, int y, int z, int w)
        {
            return x + "," + y + "," + z + "," + w;
        }

        static IEnumerable<string> GetNeightbours4(string coord)
        {
            var co = coord.Split(',').Select(t => int.Parse(t)).ToList();
            for (int w = -1; w < 2; ++w)
            {
                for (int z = -1; z < 2; ++z)
                {
                    for (int y = -1; y < 2; ++y)
                    {
                        for (int x = -1; x < 2; ++x)
                        {
                            if (x == 0 && y == 0 && z == 0 && w == 0)
                                continue;

                            yield return CoordToString4(co[0] + x, co[1] + y, co[2] + z, co[3] + w);
                        }
                    }
                }

            }
        }
    }
}
