using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

using Coordinate = (int X, int Y);

namespace AdventOfCode22;

public class Day15
{
    [Theory]
    [InlineData("day15.txt", 2000000, 4793062)]
    [InlineData("day15.test.txt", 10, 26)]
    public async Task Part1(string fileName, int row, int expectedResult)
    {
        var input = await File.ReadAllLinesAsync(fileName);
        var regex = new Regex("x=(-?\\d+), y=(-?\\d+)");
        
        var readings = input.Select(line =>
        {
            var matches = regex.Matches(line);
            var sensor = new Coordinate(int.Parse(matches[0].Groups[1].ValueSpan), int.Parse(matches[0].Groups[2].ValueSpan));
            var beacon = new Coordinate(int.Parse(matches[1].Groups[1].ValueSpan), int.Parse(matches[1].Groups[2].ValueSpan));
            return new Reading(sensor, beacon);
        }).ToArray();

        var coordinateSystem = new CoordinateSystem(readings.Min(r => r.MinX), readings.Max(r => r.MaxX) + 1, row, row);
        foreach (var reading in readings)
        {
            reading.AddToCoordinateSystem(row, coordinateSystem);
        }
        
        var total = coordinateSystem.GetRowValues(row).Where(x => x == 1).Sum();
        
        Assert.Equal(expectedResult, total);
    }

    [Theory]
    [InlineData("day15.test.txt", 20, 56000011)]
    [InlineData("day15.txt", 4000000, 10826395253551)]
    public async Task Part2(string fileName, int max, long expectedResult)
    {
        var input = await File.ReadAllLinesAsync(fileName);
        var regex = new Regex("x=(-?\\d+), y=(-?\\d+)");
        
        var readings = input.Select(line =>
        {
            var matches = regex.Matches(line);
            var sensor = new Coordinate(int.Parse(matches[0].Groups[1].ValueSpan), int.Parse(matches[0].Groups[2].ValueSpan));
            var beacon = new Coordinate(int.Parse(matches[1].Groups[1].ValueSpan), int.Parse(matches[1].Groups[2].ValueSpan));
            return new Reading(sensor, beacon);
        }).ToArray();
        
        int counter = 0;
        Coordinate signal = default;

        for (int y = 0; y <= max; y++)
        {
            var ranges = new List<(int start, int end)>();
            foreach (var reading in readings)
            {
                if (reading.MinY <= y && reading.MaxY >= y)
                {
                    // reading has data within the given y row
                    var sensor = reading.Sensor;
                    var yDistance = Math.Abs(sensor.Y - y);
                    var xWidth = reading.BeaconDistance - yDistance;
                    var xStart = Math.Max(sensor.X - xWidth, 0);
                    var xMax = Math.Min(sensor.X + xWidth, max);
                    ranges.Add((xStart, xMax));
                }
            }

            var sortedByStart = ranges.OrderBy(t => t.start);
            var sortedByEnd = ranges.OrderByDescending(t => t.end);

            var lowestBlock = sortedByStart.First();
            int lowerStart = lowestBlock.start;
            if (lowerStart != 0)
            {
                signal = new Coordinate(0, y);
                break;
            }
            //TODO if lowerStart != 0, we already found the right position
            int lowerEnd = lowestBlock.end;
            foreach (var block in sortedByStart.Skip(1))
            {
                if (block.start > lowerEnd + 1)
                {
                    // not adjecent
                    break;
                }
                if (block.end > lowerEnd)
                {
                    lowerEnd = block.end;
                }
            }

            var upperBlock = sortedByEnd.First();
            var upperEnd = upperBlock.end;
            if (upperEnd != max)
            {
                signal = new Coordinate(max, y);
                break;
            }
            int upperStart = upperBlock.start;
            foreach (var block in sortedByEnd.Skip(1))
            {
                if (block.end < upperStart - 1)
                {
                    // not adjecent
                    break;
                }

                if (block.start < upperStart)
                {
                    upperStart = block.start;
                }
            }

            if (upperStart > lowerEnd)
            {
                // found a gap
                signal = new Coordinate(lowerEnd + 1, y);
                break;
            }
        }
        

        long frequency = (signal.X * 4000000l) + signal.Y; 
        Assert.Equal(expectedResult, frequency);
    }
    
    public class Reading
    {
        public Reading(Coordinate sensor, Coordinate beacon)
        {
            Sensor = sensor;
            Beacon = beacon;
            BeaconDistance = Math.Abs(sensor.X - beacon.X) + Math.Abs(sensor.Y - beacon.Y);
            MinX = Sensor.X - BeaconDistance;
            MaxX = Sensor.X + BeaconDistance;
            MinY = Sensor.Y - BeaconDistance;
            MaxY = Sensor.Y + BeaconDistance;
        }

        public Coordinate Sensor { get; }
        public Coordinate Beacon { get; }
        public int BeaconDistance { get; }

        public int MinX { get; }
        public int MaxX { get; }
        public int MinY { get; }
        public int MaxY { get; }

        public void AddToCoordinateSystem(int rowNumber, int[] rowArray)
        {
            if (rowNumber < MinY || rowNumber > MaxY)
            {
                // this reading is not close to the desired row
                return;
            }

            var yDistance = Math.Abs(Sensor.Y - rowNumber);
            var xWidth = BeaconDistance - yDistance;
            var xStart = Math.Max(Sensor.X - xWidth, 0);
            var xMax = Math.Min(Sensor.X + xWidth + 1, rowArray.Length);
            for (int x = xStart; x < xMax; x++)
            {
                rowArray[x] = 1;
            }
        
            // mark sensors/beacons separately as they don't count towards the expected result
            if (Sensor.Y == rowNumber && Sensor.X >= 0 && Sensor.X < rowArray.Length)
            {
                rowArray[Sensor.X] = 9;
            }

            if (Beacon.Y == rowNumber && Beacon.X >= 0 && Beacon.X < rowArray.Length)
            {
                rowArray[Beacon.X] = 8;
            }
        }
        
        public void AddToCoordinateSystem(int rowNumber, CoordinateSystem rowArray)
        {
            if (rowNumber < MinY || rowNumber > MaxY)
            {
                // this reading is not close to the desired row
                return;
            }

            var yDistance = Math.Abs(Sensor.Y - rowNumber);
            var xWidth = BeaconDistance - yDistance;
            for (int x = Sensor.X - xWidth; x <= Sensor.X + xWidth; x++)
            {
                rowArray.TrySet(x, rowNumber, 1);
            }
        
            // mark sensors/beacons separately as they don't count towards the expected result
            if (Sensor.Y == rowNumber)
            {
                rowArray[Sensor.X, rowNumber] = 9;
            }

            if (Beacon.Y == rowNumber)
            {
                rowArray[Beacon.X, rowNumber] = 8;
            }
        }
    }
}


/// <summary>
/// Little helper class to work in a coordinate system more comfortably.
/// Allows using X,Y coordinates without inverting them as typically required in two-dimensional arrays.
/// Handles offsetting desired coordinates with the actual internal array structure.
/// </summary>
public class CoordinateSystem
{
    int[][] array;
    readonly int xMin;
    int yMin;
    readonly bool throwOnOutOfBoundAccess;
    readonly int xMax;
    int yMax;

    public CoordinateSystem(int xMin, int xMax, int yMin, int yMax, bool throwOnOutOfBoundAccess = true)
    {
        if (xMax < 0 || yMax < 0)
        {
            throw new NotSupportedException();
        }
        
        var xWidth = Math.Abs(xMax - xMin) + 1;
        var yWidth = Math.Abs(yMax - yMin) + 1;
        this.xMin = xMin;
        this.yMin = yMin;
        this.xMax = xMax;
        this.yMax = yMax;
        this.throwOnOutOfBoundAccess = throwOnOutOfBoundAccess;

        array = new int[yWidth][];
        for(int i = 0; i < yWidth; i++)
        {
            array[i] = new int[xWidth];
        }
    }

    public void Reset(int newY)
    {
        //TODO: very hack to verify if this really brings the necessary speedup
        for (int i = 0; i < array[0].Length; i++)
        {
            array[0][i] = 0;
        }

        this.yMin = this.yMax = newY;
    }

    bool IsWithinBoundary(int x, int y)
    {
        if (throwOnOutOfBoundAccess)
        {
            return true; // array access itself will throw
        }

        return x >= xMin && x <= xMax && y >= yMin && y <= yMax;
    }
    
    public int this[int x, int y]
    {
        get
        {
            return array[Math.Abs(yMin - y)][Math.Abs(xMin - x)];
        }

        set
        {
            if (IsWithinBoundary(x, y))
            {
                array[Math.Abs(yMin - y)][Math.Abs(xMin - x)] = value;
            }
        }
    }

    public void TrySet(int x, int y, int value)
    {
        if (IsWithinBoundary(x, y))
        {
            var y_ = Math.Abs(yMin - y);
            var x_ = Math.Abs(xMin - x);
            if (array[y_][x_] == 0)
            {
                array[y_][x_] = value;
            }
        }
    }

    public int[] GetRowValues(int y)
    {
        return array[Math.Abs(yMin - y)];
    }

    public void Render(ITestOutputHelper outputHelper)
    {
        var sb = new StringBuilder();
        var rowLength = array[0].Length;
        for (int y = 0; y < array.Length; y++)
        {
            for (int x = 0; x < rowLength; x++)
            {
                sb.Append(array[y][x]);
            }

            sb.AppendLine();
        }

        outputHelper.WriteLine(sb.ToString());
    }
}