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

        var rowArray = new CoordinateSystem(readings.Min(r => r.MinX), readings.Max(r => r.MaxX) + 1, row, row);
        foreach (var reading in readings)
        {
            reading.AddToCoordinateSystem(row, rowArray);
        }
        
        var total = rowArray.GetRow(row).Where(x => x == 1).Sum();
        
        Assert.Equal(expectedResult, total);
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

        public void AddToCoordinateSystem(int row, CoordinateSystem rowArray)
        {
            if (row < MinY || row > MaxY)
            {
                // this reading is not close to the desired row
                return;
            }

            var yDistance = Math.Abs(Sensor.Y - row);
            var xWidth = BeaconDistance - yDistance;
            for (int x = Sensor.X - xWidth; x <= Sensor.X + xWidth; x++)
            {
                rowArray.TrySet(x, row, 1);
            }
        
            // mark sensors/beacons separately as they don't count towards the expected result
            if (Sensor.Y == row)
            {
                rowArray[Sensor.X, row] = 9;
            }

            if (Beacon.Y == row)
            {
                rowArray[Beacon.X, row] = 8;
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
    readonly int yMin;

    public CoordinateSystem(int xMin, int xMax, int yMin, int yMax)
    {
        if (xMax < 0 || yMax < 0)
        {
            throw new NotSupportedException();
        }
        
        var xWidth = Math.Abs(xMax - xMin) + 1;
        var yWidth = Math.Abs(yMax - yMin) + 1;
        this.xMin = xMin;
        this.yMin = yMin;
        
        array = new int[yWidth][];
        for(int i = 0; i < yWidth; i++)
        {
            array[i] = new int[xWidth];
        }
    }
    
    public int this[int x, int y]
    {
        get
        {
            //TODO constraint checks
            return array[Math.Abs(yMin - y)][Math.Abs(xMin - x)];
        }

        set
        {
            //TODO constraint checks
            array[Math.Abs(yMin - y)][Math.Abs(xMin - x)] = value;
        }
    }

    public void TrySet(int x, int y, int value)
    {
        try
        {
            var y_ = Math.Abs(yMin - y);
            var x_ = Math.Abs(xMin - x);
            if (array[y_][x_] == 0)
            {
                array[y_][x_] = value;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public int[] GetRow(int y)
    {
        int numCols = array[0].Length;
        int[] rowValues = new int[numCols];

        for (int col = 0; col < numCols; col++)
        {
            rowValues[col] = array[Math.Abs(yMin - y)][col];
        }

        return rowValues;
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