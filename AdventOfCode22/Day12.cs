namespace AdventOfCode22;

public class Day12
{
    [Fact]
    public async void Part1()
    {
        var input = await File.ReadAllLinesAsync("day12.txt");
        var grid = Grid.Parse(input);

        var result =  FindShortestPath(grid, grid.Start);
        
        Assert.Equal(408, result);
    }
    
    [Fact]
    public async void Part2()
    {
        var input = await File.ReadAllLinesAsync("day12.txt");
        var grid = Grid.Parse(input);

        var results = new List<int>();
        for (int y = 0; y < grid.Count; y++)
        {
            for (int x = 0; x < grid[0].Length; x++)
            {
                if (grid[y][x] == 0)
                {
                    results.Add(FindShortestPath(grid, new Pos(y, x)));
                }
            }
        }

        var result = results.Min();
        
        Assert.Equal(29, result);
    }

    int FindShortestPath(Grid levelMap, Pos startPosition)
    {
        int[][] distanceMap = Enumerable.Range(0, levelMap.Count)
            .Select(_ => Enumerable.Repeat(int.MaxValue, levelMap[0].Length).ToArray()).ToArray();
        distanceMap[0][0] = 0;

        var visitedNodes = new HashSet<Pos>();
        var nextNodes = new List<(int distance, Pos node)>()
        {
            (0, startPosition)
        };

        while (nextNodes.Any())
        {
            var currentNode = nextNodes.MinBy(tuple => tuple.distance);
            var currentPos = currentNode.node;
            var currentDistance = currentNode.distance;
            var currentLevel = Math.Max(0, levelMap[currentPos.y][currentPos.x]);
            nextNodes.Remove(currentNode);

            foreach (var neighbor in GetNeighbors(currentPos, levelMap.Count - 1, levelMap[0].Length - 1))
            {
                //check if not visited already
                if (visitedNodes.Contains(neighbor))
                {
                    continue;
                }
                
                var neighborLevel = levelMap[neighbor.y][neighbor.x];
                // check if neighbor is reachable
                if (neighborLevel <= currentLevel + 1)
                {
                    if (neighbor == levelMap.Goal)
                    {
                        // no need to complete other paths since all node distances are equal
                        return currentDistance + 1;
                    }
                    
                    // node reachable
                    var distanceToNode = currentDistance + 1;
                    if (distanceToNode < distanceMap[neighbor.y][neighbor.x])
                    {
                        // shorter path found
                        distanceMap[neighbor.y][neighbor.x] = distanceToNode;
                        // valid next node
                        nextNodes.Add((distanceToNode, neighbor));
                    }
                }
            }

            // mark current node as visited
            visitedNodes.Add(currentPos);
            nextNodes.RemoveAll(tuple => tuple.node == currentPos);
        }

        return int.MaxValue;
    }

    readonly record struct Pos(int y, int x);

    //TODO make local function?
    IEnumerable<Pos> GetNeighbors(Pos currentPos, int maxY, int maxX)
    {
        (int y, int x) = currentPos;

        if (y < maxY)
        {
            yield return new Pos(y + 1, x);
        }
        if (y > 0)
        {
            yield return new Pos(y - 1, x);
        }
        if (x > 0)
        {
            yield return new Pos(y, x - 1);
        }
        if (x < maxX)
        {
            yield return new Pos(y, x + 1);
        }
    }

    class Grid : List<int[]>
    {
        public static Grid Parse(string[] input)
        {
            var grid = new Grid();
            for (int y = 0; y < input.Length; y++)
            {
                grid.Add(input[y].Select((c, x) =>
                {
                    if (c == 'S')
                    {
                        grid.Start = new Pos(y, x);
                        return 0;
                    }

                    if (c == 'E')
                    {
                        grid.Goal = new Pos(y, x);
                        return 'z' - 'a';
                    }
                
                    return c - 'a';
                }).ToArray());
            }

            return grid;
        }

        public Pos Goal { get; set; }

        public Pos Start { get; set; }
    }
}