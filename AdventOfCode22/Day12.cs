namespace AdventOfCode22;

public class Day12
{
    [Fact]
    public async void Part1()
    {
        int[][] grid = await ParseGrid();

        var result =  FindShortestPath(grid);
        
        Assert.Equal(408, result);
    }

    int FindShortestPath(int[][] levelMap)
    {
        int[][] distanceMap = Enumerable.Range(0, levelMap.Length)
            .Select(_ => Enumerable.Repeat(int.MaxValue, levelMap[0].Length).ToArray()).ToArray();
        distanceMap[0][0] = 0;

        var visitedNodes = new HashSet<Pos>();
        var nextNodes = new List<(int distance, Pos node)>()
        {
            (0, FindStartPos(levelMap))
        };

        while (nextNodes.Any())
        {
            var currentNode = nextNodes.MinBy(tuple => tuple.distance);
            var currentPos = currentNode.node;
            var currentDistance = currentNode.distance;
            var currentLevel = Math.Max(0, levelMap[currentPos.y][currentPos.x]);
            nextNodes.Remove(currentNode);

            foreach (var neighbor in GetNeighbors(currentPos, levelMap.Length - 1, levelMap[0].Length - 1))
            {
                //check if not visited already
                if (visitedNodes.Contains(neighbor))
                {
                    continue;
                }
                
                var neighborLevel = levelMap[neighbor.y][neighbor.x];
                if (neighborLevel == 1000 && currentLevel >= ('y' - 'a'))
                {
                    // no need to complete other paths since all node distances are equal
                    return currentDistance + 1;
                }

                if (neighborLevel <= currentLevel + 1)
                {
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

        throw new Exception("no path found");
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

    async Task<int[][]> ParseGrid()
    {
        var lines = (await File.ReadAllLinesAsync("day12.txt")).ToArray();

        var grid = new int[lines.Length][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = lines[i].Select(c =>
            {
                if (c == 'S')
                {
                    return -1000;
                }

                if (c == 'E')
                {
                    return 1000;
                }
                
                return c - 'a';
            }).ToArray();
        }

        return grid;
    }

    Pos FindStartPos(int[][] heightMap)
    {
        for (int y = 0; y < heightMap.Length; y++)
        {
            for (int x = 0; x < heightMap[0].Length; x++)
            {
                if (heightMap[y][x] == -1000)
                {
                    return new Pos(y, x);
                }
            }
        }

        throw new ArgumentException("No start position on map");
    }
}