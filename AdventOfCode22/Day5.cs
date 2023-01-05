using System.Text.RegularExpressions;

namespace AdventOfCode22;

public class Day5
{
    [Fact]
    void Part1()
    {
        var input = File.ReadAllLines("day5.txt");

        var stacks = ReadStacks(input);
        var instructions = ReadInstructions(input);

        foreach (var instruction in instructions)
        {
            for (int i = 0; i < instruction.items; i++)
            {
                stacks[instruction.to].Push(stacks[instruction.from].Pop());
            }
        }

        var result = string.Join("", stacks.Select(stack => stack.Pop()));

        Assert.Equal("RFFFWBPNS", result);
    }

    [Fact]
    void Part2()
    {
        var input = File.ReadAllLines("day5.txt");

        var stacks = ReadStacks(input);
        var instructions = ReadInstructions(input);

        var intermediate = new Stack<char>();
        foreach (var instruction in instructions)
        {
            for (var i = 0; i < instruction.items; i++)
            {
                var crate = stacks[instruction.from].Pop();
                intermediate.Push(crate);
            }

            while (intermediate.TryPop(out var crate))
            {
                stacks[instruction.to].Push(crate);
            }
        }

        var result = string.Join("", stacks.Select(stack => stack.Pop()));

        Assert.Equal("CQQBBJFCS", result);
    }

    static IEnumerable<(int items, int from, int to)> ReadInstructions(string[] input)
    {
        var part2 = input.SkipWhile(line => !line.StartsWith("move"));
        return part2
            .Select(line => Regex.Match(line, @"move (.*) from (.*) to (.*)"))
            .Select(match => (items: int.Parse(match.Groups[1].Value), from: int.Parse(match.Groups[2].Value) - 1, to: int.Parse(match.Groups[3].Value) - 1));
    }

    static List<Stack<char>> ReadStacks(string[] input)
    {
        var part1 = input.TakeWhile(line => line != string.Empty);
        var stacks = new List<Stack<char>>(part1.Last().Chunk(4).Select(_ => new Stack<char>()));

        foreach (var line in part1.Reverse().Skip(1))
        {
            var index = 0;
            foreach (var crateChars in line.Chunk(4))
            {
                if (crateChars[1] != ' ')
                {
                    stacks[index].Push(crateChars[1]);
                }

                index++;
            }
        }

        return stacks;
    }
}