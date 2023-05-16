using System.Globalization;

namespace AdventOfCode22;

public class Day8
{
    [Fact]
    public async Task Part1()
    {
        var forest = ParseForest(await File.ReadAllLinesAsync("day8.txt"));

        // unnecessarily "optimized" approach that only requires tracing each row/column once per direction rather than "raytracing" each individual tree.
        foreach (IList<Tree> row in forest)
        {
            CalculateViewMinimumHeight(row, (t, h) => t.Left = h);
            CalculateViewMinimumHeight(row.Reverse(), (t, h) => t.Right = h);
        }

        forest = Rotate(forest).ToList();

        foreach (var row in forest)
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

    [Fact]
    public async Task Part2()
    {
        var forest = ParseForest(await File.ReadAllLinesAsync("day8.txt"));

        foreach (var row in forest)
        {
            CalculateMaxViewDistance(row, (t, v) => t.Left = v);
            CalculateMaxViewDistance(row.Reverse(), (t, v) => t.Right = v);
        }

        forest = Rotate(forest).ToList();

        foreach (var row in forest)
        {
            CalculateMaxViewDistance(row, (t, v) => t.Top = v);
            CalculateMaxViewDistance(row.Reverse(), (t, v) => t.Bottom = v);
        }

        var bestScore = forest.Aggregate(0, (current, line) => 
            line.Select(lineElement => lineElement.Top * lineElement.Right * lineElement.Bottom * lineElement.Left)
                .Prepend(current)
                .Max());

        Assert.Equal(327180, bestScore);
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

    static void CalculateMaxViewDistance(IEnumerable<Tree> row, Action<Tree, int> assign)
    {
        var treesInView = new int[10];
        foreach (var tree in row)
        {
            var height = tree.Height;
            assign(tree, treesInView[height]);
            // track view distance for each potential tree height
            for (int h = 0; h < 10; h++)
            {
                treesInView[h] = h > height ? treesInView[h] + 1 : 1;
            }
        }
    }

    IEnumerable<IList<Tree>> Rotate(List<IList<Tree>> input)
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

    static List<IList<Tree>> ParseForest(string[] lines)
    {
        List<IList<Tree>> grid = new();
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

    record Tree
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Height { get; set; }
    }
}