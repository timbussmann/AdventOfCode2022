using Xunit.Abstractions;

namespace AdventOfCode22;


public class Day10
{
    readonly ITestOutputHelper testOutputHelper;

    public Day10(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    async Task Part1()
    {
        var lines = await File.ReadAllLinesAsync("day10.txt");

        int currentValue = 1;
        var cycleValues = new List<int> { currentValue }; // value of register X at the beginning of the cycle n
        foreach (var line in lines)
        {
            var lineParts = line.Split(' ');
            switch (lineParts)
            {
                case ["noop"]:
                    cycleValues.Add(currentValue);
                    break;
                case ["addx", var value]:
                    cycleValues.Add(currentValue);
                    cycleValues.Add(currentValue);
                    currentValue += int.Parse(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineParts));
            }
        }

        var signal = 0;
        int index = 20;
        do
        {
            var cycleValue = index * cycleValues[index];
            signal += cycleValue;
            index += 40;
        } while (index < cycleValues.Count);

        Assert.Equal(17020, signal);
    }

    [Fact]
    async Task Part2()
    {
        var lines = await File.ReadAllLinesAsync("day10.txt");

        int currentValue = 1;
        var cycleValues = new List<int> { currentValue }; // value of register X at the beginning of the cycle n
        foreach (var line in lines)
        {
            var lineParts = line.Split(' ');
            switch (lineParts)
            {
                case ["noop"]:
                    cycleValues.Add(currentValue);
                    break;
                case ["addx", var value]:
                    cycleValues.Add(currentValue);
                    cycleValues.Add(currentValue);
                    currentValue += int.Parse(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineParts));
            }
        }

        var pixel = new List<char>();
        for (int i = 1; i < cycleValues.Count; i++)
        {
            var spritePosition = cycleValues[i];
            var crtPos = (i-1) % 40;
            if (spritePosition >= crtPos - 1 && spritePosition <= crtPos + 1)
            {
                pixel.Add('#');
            }
            else
            {
                pixel.Add('.');
            }
        }

        var renderLines = pixel.Chunk(40).Select(chunk => string.Concat(chunk));
        foreach (var renderLine in renderLines)
        {
            testOutputHelper.WriteLine(renderLine);
        }

        // result is RLEZFLGE
    }
}