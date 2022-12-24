using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nito.Collections;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var maxX = data[0].Length - 1;
var maxY = data.Length - 1;

var blizzards = new List<(int x, int y, char direction)>();
for (int y = 1; y < maxY; ++y)
	for (int x = 1; x < maxX; ++x)
		if (data[y][x] != '.')
			blizzards.Add((x, y, data[y][x]));

var maxBoards = (maxX - 1) * (maxY - 1);
var occupiedSpotsByRound = new List<HashSet<(int x, int y)>>();
var currentBlizzards = blizzards;
for (int boardNum = 0; boardNum < maxBoards; ++boardNum)
{
	var newBlizzards = new List<(int x, int y, char direction)>();
	var occupiedSpots = new HashSet<(int x, int y)>();

	foreach (var blizzard in currentBlizzards)
	{
		occupiedSpots.Add((blizzard.x, blizzard.y));

		var newBlizzard = blizzard;
		if (blizzard.direction == '<')
		{
			if (--newBlizzard.x < 1)
				newBlizzard.x = maxX - 1;
		}
		else if (blizzard.direction == 'v')
		{
			if (++newBlizzard.y > maxY - 1)
				newBlizzard.y = 1;
		}
		else if (blizzard.direction == '>')
		{
			if (++newBlizzard.x > maxX - 1)
				newBlizzard.x = 1;
		}
		else if (blizzard.direction == '^')
		{
			if (--newBlizzard.y < 1)
				newBlizzard.y = maxY - 1;
		}

		newBlizzards.Add(newBlizzard);
	}

	occupiedSpotsByRound.Add(occupiedSpots);
	currentBlizzards = newBlizzards;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");
stopwatch = Stopwatch.StartNew();

var visited = new HashSet<(int x, int y, int step, short stage)>();
var toRun = new Deque<(int x, int y, int step, short stage)>();
toRun.AddToBack((1, 0, 0, 0));
var foundPart1 = false;

while (toRun.Count != 0)
{
	var current = toRun.RemoveFromFront();
	var (x, y, step, stage) = current;
	if (x < 0 || x > maxX || y < 0 || y > maxY || data[y][x] == '#')
		continue;

	if (stage == 1 && y == 0)
		stage = 2;

	if (y == maxY)
	{
		if (stage == 0)
		{
			if (!foundPart1)
			{
				Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {step - 1}");
				stopwatch = Stopwatch.StartNew();
				foundPart1 = true;
			}
			stage = 1;
		}
		else if (stage == 2)
		{
			Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {step - 1}");
			break;
		}
	}

	if (visited.Contains(current))
		continue;

	visited.Add(current);

	var obstacles = occupiedSpotsByRound[step % occupiedSpotsByRound.Count];
	if (!obstacles.Contains((x, y)))
		toRun.AddToBack((x, y, step + 1, stage));
	if (!obstacles.Contains((x + 1, y)))
		toRun.AddToBack((x + 1, y, step + 1, stage));
	if (!obstacles.Contains((x - 1, y)))
		toRun.AddToBack((x - 1, y, step + 1, stage));
	if (!obstacles.Contains((x, y + 1)))
		toRun.AddToBack((x, y + 1, step + 1, stage));
	if (!obstacles.Contains((x, y - 1)))
		toRun.AddToBack((x, y - 1, step + 1, stage));
}

void PrintSpots(HashSet<(int x, int y)> occupiedSpots)
{
	for (int y = 0; y < data.Length; ++y)
	{
		for (int x = 0; x < data[0].Length; ++x)
			Console.Write(occupiedSpots.Contains((x, y)) ? '*' : data[y][x] == '#' ? '#' : '.');

		Console.WriteLine();
	}

	Console.WriteLine();
}
