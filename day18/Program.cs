using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 let cube = line.Split(",").Select(int.Parse).ToArray()
	 select (x: cube[0], y: cube[1], z: cube[2])).ToHashSet();

var minX = data.Select(p => p.x).Min();
var maxX = data.Select(p => p.x).Max();
var minY = data.Select(p => p.y).Min();
var maxY = data.Select(p => p.y).Max();
var minZ = data.Select(p => p.z).Min();
var maxZ = data.Select(p => p.z).Max();

var outsideAirCache = new Dictionary<(int x, int y, int z), bool>();

bool CanFindOutsideAir((int x, int y, int z) pixel, HashSet<(int x, int y, int z)>? visited = null)
{
	visited ??= new();

	if (outsideAirCache.TryGetValue(pixel, out var result))
		return result;
	if (visited.Contains(pixel))
		return false;

	visited.Add(pixel);

	if (data.Contains(pixel))
		result = false;
	else if (pixel.x <= minX || pixel.x >= maxX ||
			pixel.y <= minY || pixel.y >= maxY ||
			pixel.z <= minZ || pixel.z >= maxZ)
		result = true;
	else
		result =
			CanFindOutsideAir((pixel.x - 1, pixel.y, pixel.z), visited) ||
			CanFindOutsideAir((pixel.x + 1, pixel.y, pixel.z), visited) ||
			CanFindOutsideAir((pixel.x, pixel.y - 1, pixel.z), visited) ||
			CanFindOutsideAir((pixel.x, pixel.y + 1, pixel.z), visited) ||
			CanFindOutsideAir((pixel.x, pixel.y, pixel.z - 1), visited) ||
			CanFindOutsideAir((pixel.x, pixel.y, pixel.z + 1), visited);

	visited.Remove(pixel);

	outsideAirCache[pixel] = result;
	return result;
}

long GetPart1()
{
	var result = 0L;

	foreach (var pixel in data)
	{
		if (!data.Contains((pixel.x - 1, pixel.y, pixel.z)))
			result++;
		if (!data.Contains((pixel.x + 1, pixel.y, pixel.z)))
			result++;
		if (!data.Contains((pixel.x, pixel.y - 1, pixel.z)))
			result++;
		if (!data.Contains((pixel.x, pixel.y + 1, pixel.z)))
			result++;
		if (!data.Contains((pixel.x, pixel.y, pixel.z - 1)))
			result++;
		if (!data.Contains((pixel.x, pixel.y, pixel.z + 1)))
			result++;
	}

	return result;
}

long GetPart2()
{
	var result = 0L;

	for (var x = minX; x <= maxX; ++x)
		for (var y = minY; y <= maxY; ++y)
			for (var z = minZ; z <= maxZ; ++z)
				if (data.Contains((x, y, z)))
				{
					if (CanFindOutsideAir((x - 1, y, z)))
						result++;
					if (CanFindOutsideAir((x + 1, y, z)))
						result++;
					if (CanFindOutsideAir((x, y - 1, z)))
						result++;
					if (CanFindOutsideAir((x, y + 1, z)))
						result++;
					if (CanFindOutsideAir((x, y, z - 1)))
						result++;
					if (CanFindOutsideAir((x, y, z + 1)))
						result++;
				}

	return result;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
