using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace AdventOfCode22;

using Point = (int x, int y);

public class Day14
{
    private const int BlockValue = int.MaxValue;

    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day14.txt");
        var lines = ParseLines(input);
        
        var map = new int[1000,1000];

        int globalYMax = 0;
        foreach (var line in lines)
        {
            foreach (var edge in line)
            {
                var xMin = Math.Min(edge.First.Item1, edge.Second.Item1);
                var xMax = Math.Max(edge.First.Item1, edge.Second.Item1);
                var yMin = Math.Min(edge.First.Item2, edge.Second.Item2);
                var yMax = Math.Max(edge.First.Item2, edge.Second.Item2);
                globalYMax = Math.Max(globalYMax, yMax);

                for (int i = xMin; i <= xMax; i++)
                {
                    map[yMin, i] = BlockValue;
                }

                for (int i = yMin; i < yMax; i++)
                {
                    map[i, xMin] = BlockValue;
                }
            }
        }

        int counter = 0;
        Point start = (500, 0);
        do
        {
            if (map[start.y + 1, start.x] <= 0)
            {
                start.y++;
            }
            else
            {
                // found block or other sand
                // try left
                if (map[start.y + 1, start.x - 1] <= 0)
                {
                    start.y++;
                    start.x--;
                }
                else
                {
                    // left blocked
                    // try right
                    if (map[start.y + 1, start.x + 1] <= 0)
                    {
                        start.y++;
                        start.x++;
                    }
                    else
                    {
                        // right blocked too
                        // rest at current pos
                        map[start.y, start.x] = 1;
                        start = (500, 0); // new sand block
                        counter++;
                    }
                }
            }
        } while (start.y <= globalYMax);
        
        Assert.Equal(1003, counter);
    }

    private static IEnumerable<IEnumerable<((int, int) First, (int, int) Second)>> ParseLines(string[] input)
    {
        var lines = input.Select(line =>
        {
            var points = line.Split("->").Select(pointString =>
            {
                var pointElements = pointString.Split(',');
                return (int.Parse(pointElements[0].Trim()), int.Parse(pointElements[1].Trim()));
            }).ToArray();
            return points.Zip(points.Skip(1));
        });
        return lines;
    }

    [Fact]
    public async Task Part2()
    {
        var input = await File.ReadAllLinesAsync("day14.txt");
        var lines = ParseLines(input);

        var allPoints = lines
            .SelectMany(l => l
                .SelectMany(p => new[] { p.First, p.Second }))
            .ToArray();
        var maxY = allPoints.Max(p => p.Item2);
        
        // +1 because of zero index and +2 due to part 2 addition
        var map = new int[maxY + 3, 1000];
        foreach (var line in lines)
        {
            foreach (var edge in line)
            {
                var xMin = Math.Min(edge.First.Item1, edge.Second.Item1);
                var xMax = Math.Max(edge.First.Item1, edge.Second.Item1);
                var yMin = Math.Min(edge.First.Item2, edge.Second.Item2);
                var yMax = Math.Max(edge.First.Item2, edge.Second.Item2);

                for (int i = xMin; i <= xMax; i++)
                {
                    map[yMin, i] = BlockValue;
                }

                for (int i = yMin; i < yMax; i++)
                {
                    map[i, xMin] = BlockValue;
                }
            }
        }

        for (int i = 0; i < map.GetLength(1); i++)
        {
            map[map.GetLength(0) - 1, i] = BlockValue;
        }
        
        int counter = 0;
        Point start = (500, 0);
        do
        {
            if (map[start.y + 1, start.x] <= 0)
            {
                start.y++;
            }
            else
            {
                // found block or other sand
                // try left
                if (map[start.y + 1, start.x - 1] <= 0)
                {
                    start.y++;
                    start.x--;
                }
                else
                {
                    // left blocked
                    // try right
                    if (map[start.y + 1, start.x + 1] <= 0)
                    {
                        start.y++;
                        start.x++;
                    }
                    else
                    {
                        // right blocked too
                        // rest at current pos
                        map[start.y, start.x] = 1;
                        start = (500, 0); // new sand block
                        counter++;
                    }
                }
            }
        } while (map[0, 500] <= 0);
        
        Assert.Equal(1003, counter);
    }
}