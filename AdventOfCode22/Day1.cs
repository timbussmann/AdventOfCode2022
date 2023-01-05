namespace AdventOfCode22;

using Xunit;

public class Day1
{
    [Fact]
    void Part1()
    {
        var calories = CaloriesByElf();

        Assert.Equal(67016, calories.First());
    }

    [Fact]
    void Part2()
    {
        var calories = CaloriesByElf();
        Assert.Equal(200116, calories.Take(3).Sum());
    }

    static IOrderedEnumerable<int> CaloriesByElf()
    {
        var input = File.ReadAllText("day1.txt");
        var elves = input.Split($"{Environment.NewLine}{Environment.NewLine}");
        var calories = elves
            .Select(e => e
                .Split(Environment.NewLine)
                .Select(l => int.Parse(l))
                .Sum())
            .OrderByDescending(c => c);
        return calories;
    }
}