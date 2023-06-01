namespace AdventOfCode22;

public class Day9
{
    [Fact]
    public async Task Part1()
    {
        var steps = await File.ReadAllLinesAsync("day9.txt");

        var visited = new HashSet<(int, int)> ();
        
        (int x, int y) tailPos = (0, 0);
        visited.Add(tailPos);
        (int xd, int yd) headDelta = (0, 0);

        foreach (var line in steps.Select(s => s.Split(" ")))
        {
            Func<(int xd, int yd), (int xd, int yd)> deltaChange = line[0] switch
            {
                "R" => d => d with { xd = d.xd + 1},
                "L" => d => d with { xd = d.xd - 1 },
                "U" => d => d with { yd = d.yd + 1 },
                "D" => d => d with { yd = d.yd - 1 },
            };

            for (int i = 0; i < int.Parse(line[1]); i++)
            {
                headDelta = deltaChange(headDelta);

                var move = headDelta switch
                {
                    (>= -1 and <= 1, >= -1 and <= 1) => (0, 0), // do not move
                    var (dx, dy) => (Math.Sign(dx), Math.Sign(dy)),
                };

                tailPos.x += move.Item1;
                tailPos.y += move.Item2;
                visited.Add(tailPos);
                headDelta.xd -= move.Item1;
                headDelta.yd -= move.Item2;
            }
        }

        Assert.Equal(6503, visited.Count);
    }

    [Fact]
    public async Task Part2()
    {
        // change to 2 to use this approach to solve Part 1
        int numberOfKnots = 10;
        var steps = await File.ReadAllLinesAsync("day9.txt");
        var knots = Enumerable.Repeat(new Knot(0, 0), numberOfKnots).ToList();

        var visited = new HashSet<Knot> { knots[^1] };

        foreach (var line in steps.Select(s => s.Split(" ")))
        {
            Func<Knot, Knot> moveHead = line[0] switch
            {
                "R" => d => d with { x = d.x + 1 },
                "L" => d => d with { x = d.x - 1 },
                "U" => d => d with { y = d.y + 1 },
                "D" => d => d with { y = d.y - 1 },
            };

            for (int i = 0; i < int.Parse(line[1]); i++)
            {
                knots[0] = moveHead(knots[0]);

                for (int j = 1; j < knots.Count; j++)
                {
                    var dx = knots[j - 1].x - knots[j].x;
                    var dy = knots[j - 1].y - knots[j].y;

                    knots[j] = (dx, dy) switch
                    {
                        ( >= -1 and <= 1, >= -1 and <= 1) => knots[j], // do not move
                        var t => new Knot(x: knots[j].x + Math.Sign(t.dx), y: knots[j].y + Math.Sign(t.dy))
                    };
                }

                visited.Add(knots[^1]);
            }
        }

        Assert.Equal(6503, visited.Count);
    }

    record struct Knot(int x, int y);
}