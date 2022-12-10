using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

void ProcessData(Action<long, long> callback)
{
	var register = 1L;
	var cycle = 0L;

	foreach (var line in data)
	{
		var addend = 0L;
		var cycleCount = 1L;

		if (line != "noop")
		{
			cycleCount = 2L;
			addend = long.Parse(line[5..]);
		}

		while (cycleCount > 0L)
		{
			cycle++;
			callback(register, cycle);
			cycleCount--;
		}

		register += addend;
	}
}

long GetPart1()
{
	var result = 0L;
	var nextInteresting = 20L;

	ProcessData((long register, long cycle) =>
	{
		if (cycle == nextInteresting)
		{
			result += register * nextInteresting;
			nextInteresting += 40;
		}
	});

	return result;
}

string GetPart2()
{
	StringBuilder result = new(Environment.NewLine);

	ProcessData((long register, long cycle) =>
	{
		var target = (cycle - 1) % 40;

		if (Math.Abs(register - target) < 2)
			result.Append('#');
		else
			result.Append('.');

		if (target == 39)
			result.Append(Environment.NewLine);
	});

	return result.ToString();
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
