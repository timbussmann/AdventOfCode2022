using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode22.Utils;
using Coordinate = (int X, int Y);

namespace AdventOfCode22;

public class Day15
{
    [Theory]
    [InlineData("day15.txt", 2000000, 4793062)]
    [InlineData("day15.test.txt", 10, 26)]
    public async Task Part1(string fileName, int requestedRowNumber, int expectedResult)
    {
        var input = await File.ReadAllLinesAsync(fileName);
        var readings = ParseSensorReadings(input);

        // somewhat tricky because the coordinates can be negative
        var minX = readings.Min(r => r.MinX);
        int rowWidth = Math.Abs(readings.Max(r => r.MaxX) - minX);
        var row = new int[rowWidth + 1];
        foreach (var reading in readings)
        {
            reading.AddToCoordinateSystem(requestedRowNumber, row, -minX);
        }
        
        var total = row.Where(x => x == 1).Sum();
        Assert.Equal(expectedResult, total);
    }

    [Theory]
    [InlineData("day15.test.txt", 20, 56000011)]
    [InlineData("day15.txt", 4000000, 10826395253551)]
    public async Task Part2(string fileName, int max, long expectedResult)
    {
        var input = await File.ReadAllLinesAsync(fileName);
        var readings = ParseSensorReadings(input);
        
        for (int y = 0; y <= max; y++)
        {
            var sensorRanges = GetSensorRangesByRow(y, 0, max, readings);

            if (TryFindSignalGap(max, sensorRanges, y, out Coordinate signalGap))
            {
                long frequency = (signalGap.X * 4000000l) + signalGap.Y; 
                Assert.Equal(expectedResult, frequency);
                return;
            }
        }
        
        Assert.Fail("Did not find a signal gap");
    }

    /// <summary>
    /// Try to merge signal ranges from the left and right. If there is an uncovered spot, the left and right ranges do not have an overlap
    /// </summary>
    static bool TryFindSignalGap(int max, List<(int start, int end)> sensorRanges, int y, out Coordinate signal)
    {
        var sortedByStart = sensorRanges.OrderBy(t => t.start);
        var sortedByEnd = sensorRanges.OrderByDescending(t => t.end);

        var lowestBlock = sortedByStart.First();
        if (lowestBlock.start != 0)
        {
            signal = new Coordinate(0, y);
            return true;
        }

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
        if (upperBlock.end != max)
        {
            signal = new Coordinate(max, y);
            return true;
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
            return true;
        }

        signal = default;
        return false;
    }

    static List<(int start, int end)> GetSensorRangesByRow(int y, int from, int to, Reading[] readings)
    {
        var ranges = new List<(int start, int end)>();
        foreach (var reading in readings)
        {
            // skip sensors outside of the requested row
            if (reading.MinY > y || reading.MaxY < y) continue;
            
            // reading has data within the given y row
            var sensor = reading.Sensor;
            var yDistance = Math.Abs(sensor.Y - y);
            var xWidth = reading.BeaconDistance - yDistance;
            var xStart = Math.Max(sensor.X - xWidth, from);
            var xEnd = Math.Min(sensor.X + xWidth, to);
            ranges.Add((xStart, xEnd));
        }

        return ranges;
    }

    static Reading[] ParseSensorReadings(string[] input)
    {
        var regex = new Regex("x=(-?\\d+), y=(-?\\d+)");
        var readings = input.Select(line =>
        {
            var matches = regex.Matches(line);
            var sensor = new Coordinate(int.Parse(matches[0].Groups[1].ValueSpan), int.Parse(matches[0].Groups[2].ValueSpan));
            var beacon = new Coordinate(int.Parse(matches[1].Groups[1].ValueSpan), int.Parse(matches[1].Groups[2].ValueSpan));
            return new Reading(sensor, beacon);
        }).ToArray();
        return readings;
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

        public void AddToCoordinateSystem(int rowNumber, int[] rowArray, int offset)
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
                var index = x + offset;
                // make sure to not overwrite sensor/beacon values
                if (rowArray[index] == 0)
                {
                    rowArray[index] = 1;
                }
            }
        
            // mark sensors/beacons separately as they don't count towards the expected result
            // We don't need to check if we're overwriting something because sensor and beacon readings are guaranteed to not overlap
            if (Sensor.Y == rowNumber)
            {
                rowArray[Sensor.X + offset] = 9;
            }

            if (Beacon.Y == rowNumber)
            {
                rowArray[Beacon.X + offset] = 8;
            }
        }
    }
}