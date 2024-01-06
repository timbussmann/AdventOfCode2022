using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

using Coordinate = (int X, int Y);

namespace AdventOfCode22;

public class Day15
{
    readonly ITestOutputHelper testOutputHelper;

    public Day15(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Part1()
    {
        var input = await File.ReadAllLinesAsync("day15.txt");
        var regex = new Regex("x=(-?\\d+), y=(-?\\d+)");
        
        var readings = input.Select(line =>
        {
            var matches = regex.Matches(line);
            var sensor = new Coordinate(int.Parse(matches[0].Groups[1].ValueSpan), int.Parse(matches[0].Groups[2].ValueSpan));
            var beacon = new Coordinate(int.Parse(matches[1].Groups[1].ValueSpan), int.Parse(matches[1].Groups[2].ValueSpan));
            var distance = Math.Abs(sensor.X - beacon.X) + Math.Abs(sensor.Y - beacon.Y);
            return (Sensor: sensor, Beacon: beacon, Distance: distance);
        }).ToArray();

        var allPoints = readings.Select(reading => reading.Beacon).Union(readings.Select(reading => reading.Sensor)).ToArray();
        var maxDistance = readings.Max(reading => reading.Distance);
        var coords = new CoordinateSystem(allPoints.Min(c => c.X) - maxDistance, allPoints.Max(c => c.X) + maxDistance, allPoints.Min(c => c.Y) - maxDistance, allPoints.Max(c => c.Y) + maxDistance);
        foreach (var reading in readings)
        {
            coords[reading.Sensor.X, reading.Sensor.Y] = 9;
            coords[reading.Beacon.X, reading.Beacon.Y] = 8;
            
            var baseX = reading.Sensor.X;
            var baseY = reading.Sensor.Y;
            
            for (int y = 0; y <= reading.Distance; y++)
            {
                for (int x = 0; x <= (reading.Distance - y); x++)
                {
                    coords.TrySet(baseX + x, baseY + y, 1);
                    coords.TrySet(baseX - x, baseY + y, 1);
                    coords.TrySet(baseX + x, baseY - y, 1);
                    coords.TrySet(baseX - x, baseY - y, 1);
                }
            }
        }

        var row = coords.GetRow(10);
        var total = row.Where(x => x == 1).Sum();
        
        Assert.Equal(26, total);
        coords.Render(testOutputHelper);
    }
}

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
        
        //var xWidth = xMin < 0 ? xMax + Math.Abs(xMin) + 1 : xMax - xMin + 1;
        var xWidth = Math.Abs(xMax - xMin) + 1;
        //var yWidth = yMin < 0 ? yMax + Math.Abs(yMin) + 1 : yMax - yMin + 1;
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
        int numCols = array.Length;
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