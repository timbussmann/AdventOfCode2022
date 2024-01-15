using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCode22;

using Valve = (string Name, int FlowRate, ImmutableArray<string> Adjecent);
using Path = (string CurrentValve, int TotalReleased, int TimeUsed, HashSet<string> OpenedValves);
public class Day16
{
    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day16.txt");
        
        var valves = Parse(input);
        var allPaths = CalculateValveDistances(valves);

        var filteredPaths = allPaths
            .Select(p => (p.Key, p.Value
                .Where(kvp => kvp.Value.Item2.value > 0)
                .ToDictionary()))
            .ToDictionary(); // only filter valves that are meaningful to open to drastically reduce search space
        
        List<Path> current = new();
        List<Path> nextNodes = new();
        List<Path> results = new();
        current.Add(("AA", 0, 0, ["AA"]));
        while (current.Any())
        {
            foreach (var path in current)
            {
                if (path.OpenedValves.Count == filteredPaths[path.CurrentValve].Count + 1) // +1 because of AA starting point
                {
                    // all valves (that have a flow rate) have been visited, can't continue
                    results.Add(path);
                    continue;
                }
                
                foreach (var target in filteredPaths[path.CurrentValve])
                {
                    if (!path.OpenedValves.Contains(target.Key))
                    {
                        var remainingTime = 30 - path.TimeUsed - target.Value.Item1 - 1;
                        if (remainingTime < 0)
                        {
                            results.Add(path);
                            continue;
                        }
                        var released = remainingTime * target.Value.Item2.value;
                        var x = new Path(target.Key, path.TotalReleased + released, path.TimeUsed + target.Value.Item1 + 1, [..path.OpenedValves, target.Key]);
                        // only add if no better path is already in there?
                        nextNodes.Add(x);
                    }
                }
            }

            (current, nextNodes) = (nextNodes, current);
            nextNodes.Clear();
        }
        
        var result = results.Max(r => r.TotalReleased);
        Assert.Equal(1754, result);
    }

    static Dictionary<string, Dictionary<string, (int, FlowRate)>> CalculateValveDistances(Dictionary<string, Valve> valves)
    {
        Dictionary<string, Dictionary<string, (int, FlowRate)>> allPaths = new();
        foreach (var valve in valves.Values)
        {
            Dictionary<string, (int, FlowRate)> pathsFromCurrentValve = new();
            allPaths.Add(valve.Name, pathsFromCurrentValve);
            
            var adj = new List<string>{ valve.Name };
            var next = new List<string>();
            int distance = 0;
            while (adj.Any())
            {
                foreach (string adjacent in adj)
                {
                    if (!pathsFromCurrentValve.ContainsKey(adjacent))
                    {
                        var adjacentValve = valves[adjacent];
                        pathsFromCurrentValve.Add(adjacentValve.Name, (distance, new FlowRate(adjacentValve.FlowRate)));
                        next.AddRange(adjacentValve.Adjecent);
                    }
                }

                (next, adj) = (adj, next);
                next.Clear();
                distance++;
            }
        }

        return allPaths;
    }

    static Dictionary<string, Valve> Parse(string[] input)
    {
        var regex = new Regex("Valve (..) has flow rate=(\\d+); tunnels? leads? to valves? (.+)");
        var valves = input.Select(line =>
        {
            var matches = regex.Matches(line);
            
            var valve = matches[0].Groups[1].Value;
            var flowRate = int.Parse(matches[0].Groups[2].ValueSpan);
            var adjacent = matches[0].Groups[3].Value.Split(',').Select(a => a.Trim()).ToImmutableArray();
            return new Valve(valve, flowRate, adjacent);

        }).ToDictionary(x => x.Name, x => x);
        return valves;
    }
}

readonly record struct FlowRate(int value);