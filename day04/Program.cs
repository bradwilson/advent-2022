using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var stopwatch = Stopwatch.StartNew();

var regex = new Regex("(\\d+)-(\\d+),(\\d+)-(\\d+)");

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 let match = regex.Match(line)
	 select (
		left: (
			low: long.Parse(match.Groups[1].Value),
			high: long.Parse(match.Groups[2].Value)
		),
		right: (
			low: long.Parse(match.Groups[3].Value),
			high: long.Parse(match.Groups[4].Value)
		)
	 )
	).ToArray();


long GetPart1()
{
	var result = 0L;

	foreach (var (left, right) in data)
	{
		if (left.low <= right.low && left.high >= right.high)
			result++;
		else if (right.low <= left.low && right.high >= left.high)
			result++;
	}

	return result;
}

long GetPart2()
{
	var result = 0L;

	foreach (var (left, right) in data)
	{
		if (left.low >= right.low && left.low <= right.high)
			result++;
		else if (left.high >= right.low && left.high <= right.high)
			result++;
		else if (right.low >= left.low && right.low <= left.high)
			result++;
		else if (right.high >= left.low && right.high <= left.high)
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
