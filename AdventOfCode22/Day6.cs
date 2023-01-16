namespace AdventOfCode22;

public class Day6
{
    [Fact]
    public void Part1()
    {
        int GetStartIndex(string input, int distinctChars)
        {
            // requires less allocations as we can reuse the set
            HashSet<char> set = new();
            for (int i = distinctChars; i < input.Length; i++)
            {
                set.Clear();
                for (int j = 0; j < distinctChars; j++)
                {
                    set.Add(input[i - 1 - j]);
                }

                if (set.Count == distinctChars)
                {
                    return i;
                }
            }

            return -1;
        }

        var input = File.ReadAllText("day6.txt");

        var result = GetStartIndex(input, 4);

        Assert.Equal(1542, result);
    }

    [Fact]
    public void Part2()
    {
        int GetStartIndex(string input, int distinctChars)
        {
            for (int i = distinctChars; i < input.Length; i++)
            {
                if (input.Substring(i - distinctChars, distinctChars).Distinct().Count() == distinctChars)
                {
                    return i;
                }
            }

            return -1;
        }

        var input = File.ReadAllText("day6.txt");

        var result = GetStartIndex(input, 14);

        Assert.Equal(3153, result);
    }
}