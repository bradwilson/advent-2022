using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var comparer = new SpecialSorter();

long GetPart1()
{
	var result = 0L;

	for (var idx = 0; idx < data.Length; idx += 2)
	{
		var compare = comparer.Compare(data[idx], data[idx + 1]);
		if (compare == -1)
			result += idx / 2 + 1;
	}

	return result;
}

long GetPart2()
{
	var newData = new List<string>(data) { "[[2]]", "[[6]]" };
	newData.Sort(comparer);

	return (newData.IndexOf("[[2]]") + 1) * (newData.IndexOf("[[6]]") + 1);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class SpecialSorter : IComparer<string>
{
	public int Compare(string? left, string? right)
	{
		if (left == null || right == null)
			throw new InvalidOperationException();

		if (left[0] == '[' && right[0] != '[')
			return Compare(left, $"[{right}]");

		if (left[0] != '[' && right[0] == '[')
			return Compare($"[{left}]", right);

		if (left[0] != '[' && right[0] != '[')
		{
			var leftInt = int.Parse(left);
			var rightInt = int.Parse(right);

			return leftInt < rightInt ? -1 : (leftInt > rightInt ? 1 : 0);
		}

		var leftItems = Unwrap(left[1..(left.Length - 1)]);
		var rightItems = Unwrap(right[1..(right.Length - 1)]);

		for (var idx = 0; ; ++idx)
		{
			if (leftItems.Length == idx)
				return rightItems.Length == idx ? 0 : -1;
			if (rightItems.Length == idx)
				return 1;

			var itemResult = Compare(leftItems[idx], rightItems[idx]);
			if (itemResult != 0)
				return itemResult;
		}
	}

	string[] Unwrap(string value)
	{
		var result = new List<string>();

		for (var start = 0; start < value.Length; ++start)
		{
			var innerCount = 0;
			var end = start;

			while (true)
			{
				if (end == value.Length || (value[end] == ',' && innerCount == 0))
				{
					result.Add(value[start..end]);
					start = end;
					break;
				}

				if (value[end] == '[')
					innerCount++;

				if (value[end] == ']')
					innerCount--;

				end++;
			}
		}

		return result.ToArray();
	}
}
