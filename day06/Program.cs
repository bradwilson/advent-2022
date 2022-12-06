using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).First();

int FindUnique(string text, int length)
{
	for (var idx = 0; idx < text.Length - length; ++idx)
		if (text.Skip(idx).Take(length).Distinct().Count() == length)
			return idx + length;

	throw new InvalidOperationException();
}

int GetPart1() => FindUnique(data, 4);

int GetPart2() => FindUnique(data, 14);

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
