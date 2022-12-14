using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line.Split(" -> ").Select(x => x.Split(",").Select(int.Parse).ToArray()).ToArray()).ToArray();

var maxY1 = 0;
var minX1 = int.MaxValue;
var maxX1 = int.MinValue;

foreach (var coords in data)
{
	foreach (var coord in coords)
	{
		if (coord[0] < minX1)
			minX1 = coord[0];
		if (coord[0] > maxX1)
			maxX1 = coord[0];
		if (coord[1] > maxY1)
			maxY1 = coord[1];
	}
}

var cave1 = new Occupant[maxX1 - minX1 + 1, maxY1 + 1];

var maxY2 = maxY1 + 2;
var minX2 = Math.Min(minX1, 500 - maxY2);
var maxX2 = Math.Max(maxX1, 500 + maxY2);
var cave2 = new Occupant[maxX2 - minX2 + 1, maxY2 + 1];

foreach (var coords in data)
{
	var current = (coords[0][0], coords[0][1]);
	cave1[current.Item1 - minX1, current.Item2] = Occupant.Rock;
	cave2[current.Item1 - minX2, current.Item2] = Occupant.Rock;

	for (var idx = 1; idx < coords.Length; ++idx)
	{
		var target = (coords[idx][0], coords[idx][1]);
		var dirX = Math.Min(1, Math.Max(-1, target.Item1 - current.Item1));
		var dirY = Math.Min(1, Math.Max(-1, target.Item2 - current.Item2));

		while (current != target)
		{
			current.Item1 += dirX;
			current.Item2 += dirY;
			cave1[current.Item1 - minX1, current.Item2] = Occupant.Rock;
			cave2[current.Item1 - minX2, current.Item2] = Occupant.Rock;
		}
	}
}

for (var x = minX2; x <= maxX2; ++x)
	cave2[x - minX2, maxY2] = Occupant.Rock;

long FillCave(Occupant[,] cave, int minX, int maxX, int maxY)
{
	var result = 0L;
	bool finished = false;

	while (!finished)
	{
		var currentSand = (500, 0);

		while (true)
		{
			// Sand filled (for part 2)?
			if (cave[currentSand.Item1 - minX, currentSand.Item2] == Occupant.Sand)
			{
				finished = true;
				break;
			}

			// Fall off the bottom?
			if (currentSand.Item2 >= maxY)
			{
				finished = true;
				break;
			}

			// Try moving down first...
			if (cave[currentSand.Item1 - minX, currentSand.Item2 + 1] == Occupant.Empty)
				currentSand.Item2++;
			else
			{
				// Fall off the left edge?
				if (currentSand.Item1 <= minX)
				{
					finished = true;
					break;
				}

				// Trying moving down & left next...
				if (cave[currentSand.Item1 - minX - 1, currentSand.Item2 + 1] == Occupant.Empty)
				{
					currentSand.Item2++;
					currentSand.Item1--;
				}
				else
				{
					// Fall off the right edge?
					if (currentSand.Item1 >= maxX)
					{
						finished = true;
						break;
					}

					// Last, try moving down & right
					if (cave[currentSand.Item1 - minX + 1, currentSand.Item2 + 1] == Occupant.Empty)
					{
						currentSand.Item2++;
						currentSand.Item1++;
					}
					else
						break;
				}
			}
		}

		if (!finished)
		{
			cave[currentSand.Item1 - minX, currentSand.Item2] = Occupant.Sand;
			result++;
		}
	}

	return result;
}

long GetPart1() =>
	FillCave(cave1, minX1, maxX1, maxY1);

long GetPart2() =>
	FillCave(cave2, minX2, maxX2, maxY2);

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

enum Occupant { Empty = 0, Rock, Sand };
