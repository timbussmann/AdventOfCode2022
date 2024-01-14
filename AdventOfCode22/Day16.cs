using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCode22;

public class Day16
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day16.txt");
        
        var regex = new Regex("Valve (..) has flow rate=(\\d+); tunnels? leads? to valves? (.+)");
        var valves = input.Select(line =>
        {
            var matches = regex.Matches(line);
            
            var valve = matches[0].Groups[1].Value;
            var flowRate = int.Parse(matches[0].Groups[2].ValueSpan);
            var adjacent = matches[0].Groups[3].Value.Split(',').Select(a => a.Trim()).ToImmutableArray();
            return new
            {
                Valve = valve,
                FlowRate = flowRate,
                Adjecent = adjacent
            };

        }).ToDictionary(x => x.Valve, x => x);
        
        Dictionary<string, Dictionary<string, (int, FlowRate)>> allPaths = new();
        foreach (var valve in valves.Values)
        {
            Dictionary<string, (int, FlowRate)> paths = new();
            allPaths.Add(valve.Valve, paths);
            
            var adj = new List<string>(){ valve.Valve };
            var next = new List<string>();
            int distance = 0;
            while (adj.Any())
            {
                foreach (string adjacent in adj)
                {
                    if (!paths.ContainsKey(adjacent))
                    {
                        var node = valves[adjacent];
                        paths.Add(node.Valve, (distance, new FlowRate(node.FlowRate)));
                        next.AddRange(node.Adjecent);
                    }
                }

                (next, adj) = (adj, next);
                next.Clear();
                distance++;
            }
        }
        
        var filteredPaths = allPaths
            //.Where(p => valves[p.Key].FlowRate > 0)
            .Select(p => (p.Key, p.Value.Where(kvp => kvp.Value.Item2.value > 0).ToDictionary())).ToDictionary();
        List<(string pos, int total, int timeUsed, HashSet<string> path)> current = new();
        List<(string pos, int total, int timeUsed, HashSet<string> path)> nextNodes = new();
        List<(string pos, int total, int timeUsed, HashSet<string> path)> results = new();
        current.Add(("AA", 0, 0, new HashSet<string>() { "AA" }));
        while (current.Any())
        {
            foreach (var path in current)
            {
                if (path.path.Count == filteredPaths[path.pos].Count + 1) // +1 because of AA starting point
                {
                    results.Add(path);
                    continue;
                }
                foreach (var target in filteredPaths[path.pos])
                {
                    if (!path.path.Contains(target.Key))
                    {
                        var remainingTime = (30 - path.timeUsed - target.Value.Item1 - 1);
                        if (remainingTime < 0)
                        {
                            results.Add(path);
                            continue;
                        }
                        var released = remainingTime * target.Value.Item2.value;
                        (string pos, int total, int timeUsed, HashSet<string> path) x = (target.Key, path.total + released, path.timeUsed + target.Value.Item1 + 1, [..path.path, target.Key]);
                        // only add if no better path is already in there?
                        nextNodes.Add(x);
                    }
                }
            }

            (current, nextNodes) = (nextNodes, current);
            nextNodes.Clear();
        }
        
        var result = results.Max(r => r.total);
        Assert.Equal(1754, result);
    }
}

readonly record struct FlowRate(int value);