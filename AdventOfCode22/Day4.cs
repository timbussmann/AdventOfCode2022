namespace AdventOfCode22;

public class Day4
{
    [Fact]
    void Part1()
    {
        var input = File.ReadAllLines("day4.txt");
        int counter = input
            .Select(line => ParseGroup(line)
                .OrderBy(tuple => tuple.end - tuple.start))
            .Count(ranges => ranges.First().start >= ranges.Last().start && ranges.First().end <= ranges.Last().end);

        Assert.Equal(487, counter);
    }

    [Fact]
    void Part2()
    {
        var input = File.ReadAllLines("day4.txt");
        int counter = input
            .Select(line => ParseGroup(line)
                .OrderBy(t => t.start))
            .Count(ranges => ranges.First().end >= ranges.Last().start);

        Assert.Equal(849, counter);
    }

    static IEnumerable<(int start, int end)> ParseGroup(string line) =>
        line
            .Split(",")
            .Select(r =>
            {
                var parts = r.Split("-");
                return (start: int.Parse(parts[0]), end: int.Parse(parts[1]));
            });
}