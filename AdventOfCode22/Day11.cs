namespace AdventOfCode22;

public class Day11
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day11.txt");

        var monkeys = ParseMonkeys(input).ToArray();

        for (int i = 0; i < 20; i++)
        {
            MonkeyRound(monkeys, level => level / 3);
        }

        var result = monkeys
            .Select(m => m.InspectedItems)
            .OrderByDescending(i => i)
            .Take(2)
            .Aggregate(1L, (result, itemsInspected) => result * itemsInspected);
        Assert.Equal(120756, result);
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
        
        var result = monkeys
            .Select(m => m.InspectedItems)
            .OrderByDescending(i => i)
            .Take(2)
            .Aggregate(1L, (result, itemsInspected) => result * itemsInspected);
        Assert.Equal(39109444654, result);
    }

    static void MonkeyRound(Monkey[] monkeys, Func<long, long> afterInspection)
    {
        foreach (var monkey in monkeys)
        {
            while (monkey.Items.TryDequeue(out var item))
            {
                monkey.InspectedItems++;
                long worryLevel = monkey.Operation.GetResult(item);
                worryLevel = afterInspection(worryLevel);
                var test = worryLevel % monkey.Test == 0;
                if (test)
                {
                    monkeys[monkey.ThrowToIfTrue].Items.Enqueue(worryLevel);
                }
                else
                {
                    monkeys[monkey.ThrowToIfFalse].Items.Enqueue(worryLevel);
                }
            }
        }
    }

    IEnumerable<Monkey> ParseMonkeys(IEnumerable<string> input)
    {
        using var enumerator = input.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var monkey = new Monkey();

            enumerator.MoveNext();
            monkey.Items = new Queue<long>(enumerator.Current.Substring("  Starting items: ".Length).Split(",").Select(s => long.Parse(s.Trim())));
            
            enumerator.MoveNext();
            monkey.Operation = enumerator.Current.Substring("  Operation: new = ".Length).Split(' ', StringSplitOptions.RemoveEmptyEntries) switch
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
    public Queue<long> Items { get; set; } = null!;
    public Operation Operation { get; set; } = null!;
    public int Test { get; set; }
    public int ThrowToIfTrue { get; set; }
    public int ThrowToIfFalse { get; set; }
    public long InspectedItems { get; set; }
}

abstract class Operation
{
    protected string A { get; set; }
    protected string B { get; set; }

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