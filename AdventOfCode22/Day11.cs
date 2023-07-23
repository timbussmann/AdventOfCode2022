namespace AdventOfCode22;

public class Day11
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day11.txt");

        var monkeys = ParseMonkeys(input).ToArray();

        var allMods = monkeys.Aggregate(1, (res, monkey) => res * monkey.Test);
        
        for (int i = 0; i < 20; i++)
        {
            MonkeyRound(monkeys, level => level / 3);
        }

        var orderByDescending = monkeys.Select(m => m.InspectedItems).OrderByDescending(i => i);
        var top2 = orderByDescending.Take(2).ToArray();
        Assert.Equal(120756, top2[0] * top2[1]);
    }
    
    [Fact]
    public async Task Part2()
    {
        var input = await File.ReadAllLinesAsync("day11.txt");

        var monkeys = ParseMonkeys(input).ToArray();

        var allMods = monkeys.Aggregate(1, (res, monkey) => res * monkey.Test);
        for (int i = 0; i < 10_000; i++)
        {
            MonkeyRound(monkeys, level => level % allMods);
        }

        var orderByDescending = monkeys.Select(m => m.InspectedItems).OrderByDescending(i => i);
        var top2 = orderByDescending.Take(2).ToArray();
        Assert.Equal(39109444654, top2[0] * top2[1]);
    }
    

    static void MonkeyRound(Monkey[] monkeys, Func<long, long> afterInspection)
    {
        for (int i = 0; i < monkeys.Length; i++)
        {
            var monkey = monkeys[i];
            foreach (var item in monkey.Items)
            {
                monkey.InspectedItems++;
                long worryLevel = monkey.Operation.GetResult(item);
                worryLevel = afterInspection(worryLevel);
                var test = worryLevel % monkey.Test == 0;
                if (test)
                {
                    monkeys[monkey.ThrowToIfTrue].Items.Add(worryLevel);
                }
                else
                {
                    monkeys[monkey.ThrowToIfFalse].Items.Add(worryLevel);
                }
            }

            monkey.Items = new List<long>();
        }
    }

    IEnumerable<Monkey> ParseMonkeys(IEnumerable<string> input)
    {
        var enumerator = input.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var monkey = new Monkey();
            enumerator.MoveNext();
            var itemsText = enumerator.Current;
            monkey.Items = itemsText.Substring("  Starting items: ".Length).Split(",").Select(s => long.Parse(s.Trim())).ToList();
            enumerator.MoveNext();
            var operationText = enumerator.Current.Substring("  Operation: new = ".Length);
            monkey.Operation = operationText.Split(' ', StringSplitOptions.RemoveEmptyEntries) switch
            {
                [var a, "*", var b] => new Multiply(a, b),
                [var a, "+", var b] => new Sum(a, b),
                _ => throw new Exception("unexpected operation")
            };
            enumerator.MoveNext();
            var testText = enumerator.Current.Substring("  Test: divisible by ".Length);
            monkey.Test = int.Parse(testText);
            enumerator.MoveNext();
            var trueText = enumerator.Current.Substring("    if true: throw to monkey ".Length);
            monkey.ThrowToIfTrue = int.Parse(trueText);
            enumerator.MoveNext();
            var falseText = enumerator.Current.Substring("    if false: throw to monkey ".Length);
            monkey.ThrowToIfFalse = int.Parse(falseText);
            enumerator.MoveNext();
            yield return monkey;
        }
    }
}

class Monkey
{
    public List<long> Items { get; set; }
    public Operation Operation { get; set; }
    public int Test { get; set; }
    public int ThrowToIfTrue { get; set; }
    public int ThrowToIfFalse { get; set; }
    public long InspectedItems { get; set; }
}

abstract class Operation
{
    public string A { get; set; }
    public string B { get; set; }

    protected Operation(string a, string b)
    {
        A = a;
        B = b;
    }

    public abstract long GetResult(long currentValue);
}

class Multiply : Operation
{
    public Multiply(string a, string b) : base(a, b)
    {
    }

    public override long GetResult(long currentValue)
    {
        return (A == "old" ? currentValue : long.Parse(A)) * (B == "old" ? currentValue : long.Parse(B));
    }
}

class Sum : Operation
{
    public Sum(string a, string b) : base(a, b)
    {
    }

    public override long GetResult(long currentValue)
    {
        return (A == "old" ? currentValue : long.Parse(A)) + (B == "old" ? currentValue : long.Parse(B));
    }
}