using System.Globalization;

namespace AdventOfCode22;

public class Day8
{
    [Fact]
    public async Task Part1()
    {
        var lines = await File.ReadAllLinesAsync("day8.txt");
        var forest = ParseForest(lines);

        foreach (IList<Tree> row in forest)
        {
            CalculateViewMinimumHeight(row, (t, h) => t.Left = h);
            CalculateViewMinimumHeight(row.Reverse(), (t, h) => t.Right = h);
        }

        forest = Rotate(forest).ToList();

        foreach (IList<Tree> row in forest)
        {
            CalculateViewMinimumHeight(row, (t, h) => t.Top = h);
            CalculateViewMinimumHeight(row.Reverse(), (t, h) => t.Bottom = h);
        }

        var counter = forest.SelectMany(row => row)
            .Count(tree => tree.Height > tree.Top
                           || tree.Height > tree.Right
                           || tree.Height > tree.Bottom
                           || tree.Height > tree.Left);

        Assert.Equal(1672, counter);
    }

    IEnumerable<List<Tree>> Rotate(List<List<Tree>> input)
    {
        for (int x = 0; x < input[0].Count; x++)
        {
            yield return Column(x).ToList();
        }

        IEnumerable<Tree> Column(int x)
        {
            for (int y = 0; y < input.Count; y++)
            {
                yield return input[y][x];
            }
        }
    }

    static void CalculateViewMinimumHeight(IEnumerable<Tree> row, Action<Tree, int> assign)
    {
        var currentMaxHeight = -1;
        foreach (var tree in row)
        {
            var height = tree.Height;
            assign(tree, currentMaxHeight);

            currentMaxHeight = Math.Max(currentMaxHeight, height);
        }
    }

    static List<List<Tree>> ParseForest(string[] lines)
    {
        List<List<Tree>> grid = new();
        foreach (var line in lines)
        {
            var currentLine = new List<Tree>();
            foreach (var c in line.ToCharArray())
            {
                currentLine.Add(new Tree()
                {
                    Height = c - '0'
                });
            }

            grid.Add(currentLine);
        }

        return grid;
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
        var resultGrid = new List<List<Tree>>();
        for (int y = 0; y < grid.Count; y++)
        {
            var curentLine = new List<Tree>();
            resultGrid.Add(curentLine);

            treesInView = new int[10];
            for (int x = 0; x < grid[0].Count; x++)
            {
                var height = grid[y][x];

                curentLine.Add(new Tree()
                {
                    Left = treesInView[height],
                    Height = height
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

    record Tree
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Height { get; set; }
        public bool Visible { get; set; }
    }
}