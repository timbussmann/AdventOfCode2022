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
        
        var maxY = FindMaximumY(lines);
        var map = BuildMap(maxY, lines);

        int counter = 0;
        Point current = (500, 0);
        do
        {
            var next = FindNextPosition(map, current, ref counter);
            if (current == next)
            {
                map[current.y, current.x] = 1;
                current = (500, 0); // new sand block
                counter++;
            }
            else
            {
                current = next;
            }

        } while (current.y <= maxY);
        
        Assert.Equal(1003, counter);
    }

    [Fact]
    public async Task Part2()
    {
        var input = await File.ReadAllLinesAsync("day14.txt");
        var lines = ParseLines(input);

        var maxY = FindMaximumY(lines);
        var map = BuildMap(maxY, lines);

        int counter = 0;
        Point current = (500, 0);
        do
        {
            var next = FindNextPosition(map, current, ref counter);
            if (current == next)
            {
                map[current.y, current.x] = 1;
                current = (500, 0); // new sand block
                counter++;
            }
            else
            {
                current = next;
            }

        } while (map[0, 500] <= 0);
        
        Assert.Equal(25771, counter);
    }

    private static int[,] BuildMap(int maxY, IEnumerable<IEnumerable<(Point First, Point Second)>> lines)
    {
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

                for (int i1 = xMin; i1 <= xMax; i1++)
                {
                    map[yMin, i1] = BlockValue;
                }

                for (int i2 = yMin; i2 < yMax; i2++)
                {
                    map[i2, xMin] = BlockValue;
                }
            }
        }

        // add bottom line for part 2
        for (int i = 0; i < map.GetLength(1); i++)
        {
            map[map.GetLength(0) - 1, i] = BlockValue;
        }

        return map;
    }

    private static IEnumerable<IEnumerable<(Point First, Point Second)>> ParseLines(string[] input)
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

    private static Point FindNextPosition(int[,] map, Point position, ref int counter)
    {
        if (map[position.y + 1, position.x] <= 0)
        {
            position.y++;
        }
        else
        {
            // found block or other sand
            // try left
            if (map[position.y + 1, position.x - 1] <= 0)
            {
                position.y++;
                position.x--;
            }
            else
            {
                // left blocked
                // try right
                if (map[position.y + 1, position.x + 1] <= 0)
                {
                    position.y++;
                    position.x++;
                }
                else
                {
                    // right blocked too
                    // rest at current pos
                }
            }
        }

        return position;
    }

    private static int FindMaximumY(IEnumerable<IEnumerable<(Point First, Point Second)>> lines)
    {
        var allPoints = lines
            .SelectMany(l => l
                .SelectMany(p => new[] { p.First, p.Second }))
            .ToArray();
        var maxY = allPoints.Max(p => p.y);
        return maxY;
    }
}