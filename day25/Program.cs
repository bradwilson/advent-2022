using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var mapping = new Dictionary<char, long>
{
	{ '=', -2 },
	{ '-', -1 },
	{ '0', 0 },
	{ '1', 1 },
	{ '2', 2 }
};

var reverseMapping = mapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

long FromSnafu(string value)
{
	var place = 1L;
	var result = 0L;

	foreach (var digit in value.Reverse())
	{
		result += (place * mapping[digit]);
		place *= 5L;
	}

	return result;
}

string ToSnafu(long value)
{
	var digitStack = new Stack<char>();

	while (value > 0)
	{
		digitStack.Push(reverseMapping[((value + 2) % 5) - 2]);
		value = (value + 2) / 5;
	}

	var result = new StringBuilder();
	foreach (var c in digitStack)
		result.Append(c);

	return result.ToString();
}

string GetPart1()
{
	var result = 0L;

	foreach (var line in data)
	{
		var value = FromSnafu(line);
		result += value;
	}

	return ToSnafu(result);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");
