using System.Globalization;

namespace AdventOfCode22;

public class Day8
{
    [Fact]
    public async Task Part1()
    {
        var lines = await File.ReadAllLinesAsync("day8.txt");
        List<List<int>> grid = new List<List<int>>();
        foreach (var line in lines)
        {
            var currentLine = new List<int>();
            foreach (var c in line.ToCharArray())
            {
                currentLine.Add(c - '0');
            }
            grid.Add(currentLine);
        }

        var resultGrid = new List<List<GridElement>>();
        for (int y = 0; y < grid.Count; y++)
        {
            var curentLine = new List<GridElement>();
            resultGrid.Add(curentLine);
            var leftHeight = -1;
            for (int x = 0; x < grid[0].Count; x++)
            {
                var height = grid[y][x];
                
                curentLine.Add(new GridElement()
                {
                    Left = leftHeight,
                    CurrentHeight = height
                });

                leftHeight = Math.Max(leftHeight, height);
            }

            var rightHeight = -1;
            for (int x = curentLine.Count - 1; x >= 0; x--)
            {
                var height = grid[y][x];
                resultGrid[y][x].Right = rightHeight;
                rightHeight = Math.Max(rightHeight, height);
            }
        }

        for (int x = 0; x < grid[0].Count; x++)
        {
            var topHeight = -1;
            for (int y = 0; y < grid.Count; y++)
            {
                var height = grid[y][x];
                resultGrid[y][x].Top = topHeight;
                topHeight = Math.Max(topHeight, height);
            }

            var bottomHeight = -1;
            for (int y = grid.Count - 1; y >= 0; y--)
            {
                var height = grid[y][x];
                resultGrid[y][x].Bottom = bottomHeight;
                bottomHeight = Math.Max(bottomHeight, height);
            }
        }

        int counter = 0;
        foreach (var line in resultGrid)
        {
            foreach (var lineElement in line)
            {
                if (lineElement.CurrentHeight <= lineElement.Top 
                    && lineElement.CurrentHeight <= lineElement.Right 
                    && lineElement.CurrentHeight <= lineElement.Bottom 
                    && lineElement.CurrentHeight <= lineElement.Left)
                {
                    // not visible
                }
                else
                {
                    counter++;
                    lineElement.Visible = true;
                }
            }
        }

        Assert.Equal(1672, counter);
    }

    [Fact]
    public async Task Part2()
    {
        var lines = await File.ReadAllLinesAsync("day8.txt");
        List<List<int>> grid = new List<List<int>>();
        foreach (var line in lines)
        {
            var currentLine = new List<int>();
            foreach (var c in line.ToCharArray())
            {
                currentLine.Add(c - '0');
            }
            grid.Add(currentLine);
        }

        int[] treesInView;
        var resultGrid = new List<List<GridElement>>();
        for (int y = 0; y < grid.Count; y++)
        {
            var curentLine = new List<GridElement>();
            resultGrid.Add(curentLine);

            treesInView = new int[10];
            for (int x = 0; x < grid[0].Count; x++)
            {
                var height = grid[y][x];

                curentLine.Add(new GridElement()
                {
                    Left = treesInView[height],
                    CurrentHeight = height
                });
                for (int h = 0; h < 10; h++)
                {
                    treesInView[h] = h > height ? treesInView[h] + 1 : 1;
                }
            }

            treesInView = new int[10];
            for (int x = curentLine.Count - 1; x >= 0; x--)
            {
                var height = grid[y][x];
                resultGrid[y][x].Right = treesInView[height];
                for (int h = 0; h < 10; h++)
                {
                    treesInView[h] = h > height ? treesInView[h] + 1 : 1;
                }
            }
        }

        for (int x = 0; x < grid[0].Count; x++)
        {
            treesInView = new int[10];
            for (int y = 0; y < grid.Count; y++)
            {
                var height = grid[y][x];
                resultGrid[y][x].Top = treesInView[height];
                for (int h = 0; h < 10; h++)
                {
                    treesInView[h] = h > height ? treesInView[h] + 1 : 1;
                }
            }

            treesInView = new int[10];
            for (int y = grid.Count - 1; y >= 0; y--)
            {
                var height = grid[y][x];

                resultGrid[y][x].Bottom = treesInView[height];
                for (int h = 0; h < 10; h++)
                {
                    treesInView[h] = h > height ? treesInView[h] + 1 : 1;
                }
            }
        }

        int bestScore = 0;
        foreach (var line in resultGrid)
        {
            foreach (var lineElement in line)
            {
                var score = lineElement.Top * lineElement.Right * lineElement.Bottom * lineElement.Left;
                if (score > bestScore)
                {
                    bestScore = score;
                }
            }
        }

        Assert.Equal(327180, bestScore);
    }

    record GridElement
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int CurrentHeight { get; set; }
        public bool Visible { get; set; }
    }
}