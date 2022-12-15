using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var targetY = int.Parse(data[0]);
var regex = new Regex("Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)");

long GetPart1()
{
	var hits = new Dictionary<long, char>();

	foreach (var line in data.Skip(1))
	{
		var match = regex.Match(line);
		var sensor = (x: long.Parse(match.Groups[1].Value), y: long.Parse(match.Groups[2].Value));
		var beacon = (x: long.Parse(match.Groups[3].Value), y: long.Parse(match.Groups[4].Value));
		var maxDistance = Math.Abs(beacon.x - sensor.x) + Math.Abs(beacon.y - sensor.y);

		if (sensor.y - maxDistance <= targetY && sensor.y + maxDistance >= targetY)
		{
			var distanceToY = Math.Abs(sensor.y - targetY);
			var residualLeft = maxDistance - distanceToY;

			for (long x = sensor.x - residualLeft; x <= sensor.x + residualLeft; ++x)
				if (sensor.y == targetY && x == sensor.x)
					hits[x] = 'S';
				else if (beacon.y == targetY && x == beacon.x)
					hits[x] = 'B';
				else if (!hits.ContainsKey(x))
					hits[x] = '#';
		}
	}

	return hits.Values.Where(c => c == '#').Count();
}

long GetPart2()
{
	var sensors = new List<Sensor>();

	foreach (var line in data.Skip(1))
	{
		var match = regex.Match(line);
		var sensor = (x: long.Parse(match.Groups[1].Value), y: long.Parse(match.Groups[2].Value));
		var beacon = (x: long.Parse(match.Groups[3].Value), y: long.Parse(match.Groups[4].Value));
		sensors.Add(new Sensor(sensor, beacon));
	}

	var maxDimension = targetY * 2;

	for (var y = 0L; y <= maxDimension; ++y)
	{
		var x = 0L;

		var intersections = sensors.Select(s => s.IntersetX(y)).Where(i => i != null).Select(i => i!.Value).OrderBy(i => i.min).ToArray();

		foreach (var intersection in intersections)
		{
			if (intersection.min > x)
				return x * 4000000 + y;
			else if (x <= intersection.max)
				x = intersection.max + 1;
		}

		if (x < maxDimension)
			return x * 4000000 + y;
	}

	throw new InvalidOperationException();
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class Sensor
{
	readonly (long x, long y) beacon;
	readonly long maxDistance;
	readonly (long x, long y) sensor;

	public Sensor((long x, long y) sensor, (long x, long y) beacon)
	{
		this.sensor = sensor;
		this.beacon = beacon;

		maxDistance = Math.Abs(beacon.x - sensor.x) + Math.Abs(beacon.y - sensor.y);
	}

	public (long min, long max)? IntersetX(long y)
	{
		var distanceToY = Math.Abs(y - sensor.y);
		if (distanceToY > maxDistance)
			return null;

		var residual = maxDistance - distanceToY;
		return (sensor.x - residual, sensor.x + residual);
	}

	public bool IsOccupied(long x, long y) =>
		Math.Abs(sensor.x - x) + Math.Abs(sensor.y - y) <= maxDistance;

	public long? MaximumIntersectX(long x, long y)
	{
		if (!IsOccupied(x, y))
			return null;

		var distanceToY = Math.Abs(sensor.y - y);
		var residualLeft = maxDistance - distanceToY;
		return x + residualLeft;
	}

	public override string ToString()
	{
		return $"{{ s={sensor}, b={beacon}, md={maxDistance} }}";
	}
}
