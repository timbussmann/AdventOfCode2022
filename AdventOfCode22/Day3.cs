namespace AdventOfCode22;

public class Day3
{
    [Fact]
    void Part1()
    {
        var input = File.ReadAllLines("day3.txt");
        int prioritySum = 0;
        foreach (var line in input)
        {
            var compartments = line.Chunk(line.Length / 2);
            var shared = compartments
                .First()
                .First(c => compartments.Last().Contains(c));

            prioritySum += GetCharPriority(shared);
        }

        Assert.Equal(7701, prioritySum);
    }

    static int GetCharPriority(char shared)
    {
        var priority = char.IsLower(shared)
            ? shared - 'a' + 1
            : shared - 'A' + 27;
        return priority;
    }

    [Fact]
    void Part2()
    {
        var input = File.ReadAllLines("day3.txt");
        List<char> groups = new();
        for (int i = 0; i < input.Length / 3; i++)
        {
            var groupIndex = i * 3;
            var shared = input[groupIndex].ToHashSet()
                .Join(input[groupIndex + 1].ToHashSet(), c => c, c => c, (c, _) => c)
                .Join(input[groupIndex + 2].ToHashSet(), c => c, c => c, (c, _) => c);

            groups.Add(shared.Single());
        }

        var result = groups.Select(c => GetCharPriority(c)).Sum();

        Assert.Equal(2644, result);
    }
}