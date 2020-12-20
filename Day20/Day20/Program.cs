using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
    class Coord {
        public int row = 0;
        public int col = 0;
    }

    class Program
    {
        const int TOP = 0;
        const int RIGHT = 1;
        const int BOTTOM = 2;
        const int LEFT = 3;

        class Tile
        {
            public int TileId { get; set; }
            public List<string> tilePattern = new List<string>();
            public List<string> sides = new List<string>();
            public List<string> sideReset = new List<string>();
            public int orentation = 0;

            public Tile(int tileId)
            {
                TileId = tileId;
            }

            public void TakeRow(string row)
            {
                tilePattern.Add(row);
            }

            public void FinaliseImport()
            {
                sides.Add(tilePattern.First());
                sides.Add(new string(tilePattern.Select(t => t.Last()).ToArray()));
                sides.Add(tilePattern.Last());
                sides.Add(new string(tilePattern.Select(t => t[0]).ToArray()));
                sideReset = sides.ToList();
            }

            public void ResetOrientation()
            {
                orentation = 0;
                sides = sideReset.ToList();
            }

            public bool IncrementOrientation()
            {
                if (orentation == 8)
                    return false;

                var back = sides[3];
                sides[3] = sides[2];
                sides[2] = Reverse(sides[1]);
                sides[1] = sides[0];
                sides[0] = Reverse(back);

                if (orentation == 3)
                {
                    var backup = sides[TOP];
                    sides[TOP] = sides[BOTTOM];
                    sides[RIGHT] = Reverse(sides[RIGHT]);
                    sides[BOTTOM] = backup;
                    sides[LEFT] = Reverse(sides[LEFT]);
                }
                orentation++;
                return true;
            }

            public void ApplyOrentationToPixles()
            {
                var orentationToApply = orentation;
                if (orentationToApply >= 4)
                {
                    tilePattern.Reverse();
                    orentationToApply -= 4;
                }

                while (orentationToApply > 0)
                {
                    tilePattern = RotateRight(tilePattern);
                    orentationToApply--;
                }

            }
        }

        static List<string> RotateRight(List<string> str)
        {
            List<string> output = new List<string>();
            for (int column = 0; column < str[0].Length; ++column)
            {
                StringBuilder sb = new StringBuilder();
                for (int row = str.Count - 1; row >= 0; --row)
                {
                    sb.Append(str[row][column]);
                }
                output.Add(sb.ToString());
            }
            return output;
        }

        static string Reverse(string str)
        {
            return new string(str.Reverse().ToArray());
        }

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("puzzleinput.txt"))
            {
                Stack<Tile> availableTiles = new Stack<Tile>();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("Tile "))
                    {
                        var TileSet = new Tile(int.Parse(line.Split(' ')[1].TrimEnd(':')));
                        availableTiles.Push(TileSet);
                    }
                    else if (!string.IsNullOrEmpty(line))
                    {
                        availableTiles.Peek().TakeRow(line);
                    }
                }
                foreach (var tile in availableTiles)
                    tile.FinaliseImport();

                int gridSize = (int)Math.Sqrt(availableTiles.Count);
                Tile[,] placementGrid = new Tile[gridSize, gridSize];

                int row = 0;
                int column = 0;

                Queue<Tile> puzzlePeices = new Queue<Tile>(availableTiles);
                Stack<int> levelTriesLeft = new Stack<int>();
                int currentLevelTries = puzzlePeices.Count;
                while (true)
                {
                    var tileToTry = puzzlePeices.Dequeue();
                    bool succesfullyPlaced = false;

                    do
                    {
                        if (CanTileGoOnGridAt(tileToTry, placementGrid, row, column))
                        {
                            succesfullyPlaced = true;
                            break;
                        }
                    }
                    while (tileToTry.IncrementOrientation());

                    if (succesfullyPlaced)
                    {
                        placementGrid[row, column] = tileToTry;
                        column++;
                        if (column >= gridSize)
                        {
                            column = 0;
                            row++;
                            if (row >= gridSize)
                                break;
                        }
                        levelTriesLeft.Push(currentLevelTries);
                        currentLevelTries = puzzlePeices.Count;
                        Console.SetCursorPosition(0, 0);
                        PrintTiles(placementGrid);
                    }
                    else
                    {
                        tileToTry.ResetOrientation();
                        puzzlePeices.Enqueue(tileToTry);
                        currentLevelTries--;
                        if (currentLevelTries == 0)
                        {
                            column--;   //Recurse backward
                            if (column < 0)
                            {
                                column = gridSize-1;
                                row--;
                                if (row < 0)
                                    break;
                            }
                            puzzlePeices.Enqueue(placementGrid[row, column]);
                            placementGrid[row, column] = null;
                            currentLevelTries = levelTriesLeft.Pop();
                        }
                    }
                }

                Console.WriteLine($"{placementGrid[0, 0].TileId}, {placementGrid[gridSize-1, 0].TileId}, {placementGrid[0, gridSize-1].TileId}, {placementGrid[gridSize-1, gridSize-1].TileId} ");
                Console.WriteLine($"Part 1: {(long)placementGrid[0, 0].TileId * (long)placementGrid[gridSize-1, 0].TileId * (long)placementGrid[0, gridSize-1].TileId * (long)placementGrid[gridSize-1, gridSize-1].TileId} ");

                //Now we found our placements and orientations apply them to the images we are seeing:
                foreach (var t in placementGrid)
                    t.ApplyOrentationToPixles();

                List<string> Image = RasteriseSides(placementGrid, true);
                PrintImage(Image, null);

                Image = Rasterise(placementGrid, false);

                var monster = GenerateMonster();

                for (int r = 0; r < 6; ++r)
                {
                    bool foundSomething = false;
                    bool[,] touched = new bool[Image.Count, Image[0].Length];
                    for (row = 0; row < Image.Count - monster.Max(m => m.row); ++row)
                    {
                        for (column = 0; column < Image[0].Length - monster.Max(m => m.col); ++column)
                        {
                            if (monster.All(m => Image[m.row + row][m.col + column] == '#'))
                            {
                                Console.WriteLine($"{r}: Found monster at {row}, {column}");
                                foreach(var m in monster)
                                {
                                    touched[m.row + row, m.col + column] = true;
                                }
                                foundSomething = true;
                            }
                        }
                    }
                    if (foundSomething)
                    {
                        int roughSeaCount = 0;
                        for (row = 0; row < Image.Count; ++row)
                        {
                            for (column = 0; column < Image[0].Length; ++column)
                            {
                                if(Image[row][column] == '#' && !touched[row,column])
                                {
                                    roughSeaCount++;
                                }
                            }
                        }
                        PrintImage(Image, touched);
                        Console.WriteLine($"Part 2: {roughSeaCount}");
                        break;
                    }

                    Image = RotateRight(Image);
                    if (r == 4) //Flip after 4 rotations
                        Image.Reverse();
                }
                Console.ReadLine();
            }
        }

        private static List<Coord> GenerateMonster()
        {
            var Monster = new List<string>();
            Monster.Add("                  # ");
            Monster.Add("#    ##    ##    ###");
            Monster.Add(" #  #  #  #  #  #   ");

            var monsterOffsets = new List<Coord>();

            for (int row = 0; row < Monster.Count; ++row)
            {
                for (int col = 0; col < Monster[0].Length; ++col)
                {
                    if(Monster[row][col] == '#')
                        monsterOffsets.Add(new Coord { row = row, col = col });
                }
            }
            return monsterOffsets;
        }

        private static List<string> Rasterise(Tile[,] placementGrid, bool debug = false)
        {
            List<string> raster = new List<string>();
            for (int row = 0; row < placementGrid.GetLength(0); ++row)
            {
                for (int pixleRow = 1; pixleRow < placementGrid[0, 0].tilePattern.Count - 1; ++pixleRow)
                {
                    StringBuilder pixleDataForRow = new StringBuilder();
                    for (int column = 0; column < placementGrid.GetLength(1); ++column)
                    {
                        var data = placementGrid[row, column].tilePattern[pixleRow];
                        pixleDataForRow.Append(data.Substring(1, data.Length - 2));
                    }

                    raster.Add(pixleDataForRow.ToString());
                }
            }
            return raster;
        }

        private static bool CanTileGoOnGridAt(Tile tileToTry, Tile[,] grid, int row, int column)
        {
            //Left of us (match our right with their left)
            var up = GetGridCell(grid, row - 1, column);
            var down = GetGridCell(grid, row + 1, column);
            var left = GetGridCell(grid, row, column - 1);
            var right = GetGridCell(grid, row, column + 1);

            if (left != null && tileToTry.sides[LEFT] != left.sides[RIGHT])
                return false;
            if (right != null && tileToTry.sides[RIGHT] != right.sides[LEFT])
                return false;
            if (up != null && tileToTry.sides[TOP] != up.sides[BOTTOM])
                return false;
            if (down != null && tileToTry.sides[BOTTOM] != down.sides[TOP])
                return false;
            return true;
        }


        static Tile GetGridCell(Tile[,] grid, int row, int column)
        {
            if ((row >= 0 && column >= 0 && row < grid.GetLength(0) && column < grid.GetLength(1)))
                return grid[row, column];

            return null;
        }

        #region Debug
        private static List<string> RasteriseSides(Tile[,] placementGrid, bool debug = false)
        {
            List<string> raster = new List<string>();
            if (debug)
            {
                for (int row = 0; row < placementGrid.GetLength(0); ++row)
                {
                    for (int pixleRow = 0; pixleRow < placementGrid[0, 0].tilePattern.Count; ++pixleRow)
                    {
                        StringBuilder pixleDataForRow = new StringBuilder();
                        for (int column = 0; column < placementGrid.GetLength(1); ++column)
                        {
                            for (int pixleColumn = 0; pixleColumn < placementGrid[0, 0].tilePattern[0].Length; ++pixleColumn)
                            {
                                char c = ' ';
                                if (pixleRow == 0)
                                {
                                    c = placementGrid[row, column].sides[TOP][pixleColumn];
                                }
                                else if (pixleRow == 9)
                                {
                                    c = placementGrid[row, column].sides[BOTTOM][pixleColumn];
                                }
                                else if (pixleColumn == 0)
                                {
                                    c = placementGrid[row, column].sides[LEFT][pixleRow];
                                }
                                else if (pixleColumn == 9)
                                {
                                    c = placementGrid[row, column].sides[RIGHT][pixleRow];
                                }
                                else if (pixleRow == 4 && pixleColumn == 4)
                                {
                                    c = placementGrid[row, column].orentation.ToString()[0];
                                }
                                pixleDataForRow.Append(c);
                            }

                            pixleDataForRow.Append(" ");
                        }

                        raster.Add(pixleDataForRow.ToString());
                    }
                    raster.Add(" ");
                }

            }
            return raster;
        }

        private static void PrintTiles(Tile[,] placementGrid)
        {
            for (int i = 0; i < placementGrid.GetLength(0); ++i)
            {
                for (int j = 0; j < placementGrid.GetLength(1); ++j)
                {

                    Console.Write((placementGrid[i, j] != null) ? '#' : ' ');
                }
                Console.WriteLine();
            }
        }

        private static void PrintImage(List<string> image, bool[,] bold)
        {
            for (int i = 0; i < image.Count; ++i)
            {
                for (int j = 0; j < image[i].Length; ++j)
                {
                    if (bold != null && bold[i, j])
                        Console.BackgroundColor = ConsoleColor.Red;
                    Console.Write(image[i][j]);
                    if (bold != null && bold[i, j])
                        Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
            }
        }
        #endregion
    }
}
