using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line.Select(c => c - '0').ToArray()).ToArray();

long GetPart1()
{
	var visibleTrees = 0L;

	for (int x = 0; x < data.Length; x++)
		for (int y = 0; y < data[0].Length; y++)
		{
			if (x == 0 || x == data.Length - 1 || y == 0 || y == data[0].Length)
			{
				visibleTrees++;
				continue;
			}

			var us = data[x][y];
			var visibleEdges = 4;

			for (var cx = 0; cx < x; ++cx)
				if (data[cx][y] >= us)
				{
					visibleEdges--;
					break;
				}

			for (var cx = x + 1; cx < data.Length; ++cx)
				if (data[cx][y] >= us)
				{
					visibleEdges--;
					break;
				}

			for (var cy = 0; cy < y; ++cy)
				if (data[x][cy] >= us)
				{
					visibleEdges--;
					break;
				}

			for (var cy = y + 1; cy < data[0].Length; ++cy)
				if (data[x][cy] >= us)
				{
					visibleEdges--;
					break;
				}

			if (visibleEdges != 0)
				visibleTrees++;
		}

	return visibleTrees;
}

long GetPart2()
{
	var bestScenicScore = 0L;

	for (int x = 0; x < data.Length; x++)
		for (int y = 0; y < data[0].Length; y++)
		{
			if (x == 0 || x == data.Length - 1 || y == 0 || y == data[0].Length - 1)
				continue;

			var us = data[x][y];
			var scenicScore = 1;
			var cx = 0;
			var cy = 0;

			for (cx = x - 1; cx > 0; --cx)
				if (data[cx][y] >= us)
					break;

			scenicScore *= x - cx;

			for (cx = x + 1; cx < data.Length - 1; ++cx)
				if (data[cx][y] >= us)
					break;

			scenicScore *= cx - x;

			for (cy = y - 1; cy > 0; --cy)
				if (data[x][cy] >= us)
					break;

			scenicScore *= y - cy;

			for (cy = y + 1; cy < data[0].Length - 1; ++cy)
				if (data[x][cy] >= us)
					break;

			scenicScore *= cy - y;

			if (scenicScore > bestScenicScore)
				bestScenicScore = scenicScore;
		}

	return bestScenicScore;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
