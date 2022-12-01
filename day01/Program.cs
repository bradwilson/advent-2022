using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line.Trim()).ToArray();

var elves = new List<long>();
var currentElf = 0L;

for (int idx = 0; idx < data.Length; ++idx)
{
	if (data[idx] == "")
	{
		elves.Add(currentElf);
		currentElf = 0L;
	}
	else
		currentElf += long.Parse(data[idx]);
}

if (currentElf > 0)
	elves.Add(currentElf);

long GetPart1() =>
	elves.Max();

long GetPart2() =>
	elves.OrderByDescending(x => x).Take(3).Sum();

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
