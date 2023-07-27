namespace AdventOfCode22;

public class Day12
{
    [Fact]
    public async void Part1()
    {
        var input = await File.ReadAllLinesAsync("day12.txt");
        var heightMap = HeightMap.Parse(input);

        var result = heightMap.FindShortestPath();

        Assert.Equal(408, result);
    }

    [Fact]
    public async void Part2()
    {
        var input = await File.ReadAllLinesAsync("day12.txt");
        var heightMap = HeightMap.Parse(input);

        var results = new List<int>();
        for (int y = 0; y < heightMap.Count; y++)
        {
            for (int x = 0; x < heightMap[0].Length; x++)
            {
                if (heightMap[y][x] == 0)
                {
                    results.Add(heightMap.FindShortestPath(new Pos(y, x)));
                }
            }
        }

        Assert.Equal(399, results.Min());
    }

    record Pos(int Y, int X);

    class HeightMap : List<int[]>
    {
        public static HeightMap Parse(string[] input)
        {
            var grid = new HeightMap();
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

        Pos Goal { get; set; } = null!;
        Pos Start { get; set; } = null!;

        public int FindShortestPath(Pos? startPosition = null)
        {
            int[][] distanceMap = Enumerable.Range(0, Count)
                .Select(_ => Enumerable.Repeat(int.MaxValue, this[0].Length).ToArray()).ToArray();

            var visitedNodes = new HashSet<Pos>();
            var nextNodes = new List<(int distance, Pos node)>
            {
                (0, startPosition ?? Start)
            };

            while (nextNodes.Any())
            {
                var currentNode = nextNodes.MinBy(tuple => tuple.distance);
                var currentPos = currentNode.node;
                var currentDistance = currentNode.distance;
                var currentLevel = Math.Max(0, this[currentPos.Y][currentPos.X]);
                nextNodes.Remove(currentNode);

                foreach (var neighbor in GetNeighbors(currentPos, this.Count - 1, this[0].Length - 1))
                {
                    //check if not visited already
                    if (visitedNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    var neighborLevel = this[neighbor.Y][neighbor.X];
                    // check if neighbor is reachable
                    if (neighborLevel <= currentLevel + 1)
                    {
                        if (neighbor == Goal)
                        {
                            // no need to complete other paths since all node distances are equal
                            return currentDistance + 1;
                        }

                        // node reachable
                        var distanceToNode = currentDistance + 1;
                        if (distanceToNode < distanceMap[neighbor.Y][neighbor.X])
                        {
                            // shorter path found
                            distanceMap[neighbor.Y][neighbor.X] = distanceToNode;
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

        static IEnumerable<Pos> GetNeighbors(Pos currentPos, int maxY, int maxX)
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
    }
}