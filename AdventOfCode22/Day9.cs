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
                    (2, 0) => (1, 0),
                    (-2, 0) => (-1, 0),
                    (0, 2) => (0, 1),
                    (0, -2) => (0, -1),
                    (>= 1, >= 1) => (1, 1),
                    (<= -1, >= 1) => (-1, 1),
                    (<= -1, <= -1) => (-1, -1),
                    (>= 1, <= -1) => (1, -1),
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
}