using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select (
		all: line.ToCharArray(),
		firstHalf: line.Substring(0, line.Length / 2).ToCharArray(),
		secondHalf: line.Substring(line.Length / 2).ToCharArray()
	 )
	).ToArray();

long getValue(char c) =>
	c >= 'a' && c <= 'z'
		? c - 'a' + 1
		: c - 'A' + 27;

long GetPart1()
{
	var score = 0L;

	foreach (var sack in data)
	{
		var dup = sack.firstHalf.Intersect(sack.secondHalf).Single();
		score += getValue(dup);
	}

	return score;
}

long GetPart2()
{
	var score = 0L;

	var current = 0;
	while (current < data.Length)
	{
		var id = data[current].all.Intersect(data[current + 1].all).Intersect(data[current + 2].all).Single();
		score += getValue(id);
		current += 3;
	}

	return score;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
