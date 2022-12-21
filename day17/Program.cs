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

var air = data[0].ToArray();

long GetPart1() =>
	Runner.Run(air, 2022);

long GetPart2() =>
	Runner.Run(air, 1_000_000_000_000L);

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class Pattern : IEquatable<Pattern>
{
	static IEqualityComparer<HashSet<(int, long)>> setComparer = HashSet<(int, long)>.CreateSetComparer();

	public Pattern(int airIndex, HashSet<(int, long)> stones)
	{
		AirIndex = airIndex;
		Stones = stones;
	}

	public int AirIndex { get; }

	public HashSet<(int, long)> Stones { get; }

	public bool Equals(Pattern? other) =>
		other != null && AirIndex == other.AirIndex && setComparer.Equals(Stones, other.Stones);

	public override bool Equals(object? obj) =>
		Equals(obj as Pattern);

	public override int GetHashCode() =>
		HashCode.Combine(AirIndex, setComparer.GetHashCode(Stones));
}

static class Runner
{
	static List<HashSet<(int x, long y)>> stones = new()
	{
		new() { (0, 0), (1, 0), (2, 0), (3, 0) },
		new() { (1, 2), (0, 1), (1, 1), (2, 1), (1, 0) },
		new() { (2, 2), (2, 1), (0, 0), (1, 0), (2, 0) },
		new() { (0, 3), (0, 2), (0, 1), (0, 0) },
		new() { (0, 1), (1, 1), (0, 0), (1, 0) }
	};

	static Pattern GetPattern(int airIndex, HashSet<(int x, long y)> stones, long maxHeight)
	{
		var maxY = stones.Select(c => c.y).Max();
		var patternStones = stones.Where(c => maxY - c.y < maxHeight).Select(c => (c.x, maxY - c.y)).ToHashSet();
		return new Pattern(airIndex, patternStones);
	}

	static HashSet<(int x, long y)> GetRock(long rockIndex, long bottomY) =>
		stones[(int)(rockIndex % 5)].Select(c => (c.x + 2, c.y + bottomY)).ToHashSet();

	public static long Run(char[] air, long rockCount)
	{
		var stones = new HashSet<(int x, long y)>() { (0, 0), (1, 0), (2, 0), (3, 0), (4, 0), (5, 0), (6, 0) };
		var patterns = new Dictionary<Pattern, (long rockIndex, long top)>();
		var rockIndex = 0L;
		var top = 0L;
		var airIndex = 0;
		var patternAdded = 0L;

		while (rockIndex < rockCount)
		{
			var rock = GetRock(rockIndex, top + 4);

			while (true)
			{
				if (air[airIndex] == '>')
				{
					rock = ShiftRight(rock);
					if (stones.Intersect(rock).Any())
						rock = ShiftLeft(rock);
				}
				else
				{
					rock = ShiftLeft(rock);
					if (stones.Intersect(rock).Any())
						rock = ShiftRight(rock);
				}

				airIndex = (airIndex + 1) % air.Length;

				rock = ShiftDown(rock);
				if (stones.Intersect(rock).Any())
				{
					rock = ShiftUp(rock);
					foreach (var coordinate in rock)
						stones.Add(coordinate);

					top = stones.Select(c => c.y).Max();

					if (top >= 15)
					{
						var pattern = GetPattern(airIndex, stones, 15);

						if (patterns.TryGetValue(pattern, out var result))
						{
							var distanceY = top - result.top;
							var numRocks = rockIndex - result.rockIndex;
							var multiple = (rockCount - rockIndex) / numRocks;
							patternAdded += distanceY * multiple;
							rockIndex += numRocks * multiple;
						}

						patterns[pattern] = (rockIndex, top);
					}

					++rockIndex;
					break;
				}
			}
		}

		return top + patternAdded;
	}

	static HashSet<(int x, long y)> ShiftDown(HashSet<(int x, long y)> rock) =>
		rock.Select(c => (c.x, c.y - 1)).ToHashSet();

	static HashSet<(int x, long y)> ShiftLeft(HashSet<(int x, long y)> rock)
	{
		if (rock.Any(c => c.x == 0))
			return rock;

		return rock.Select(c => (c.x - 1, c.y)).ToHashSet();
	}

	static HashSet<(int x, long y)> ShiftRight(HashSet<(int x, long y)> rock)
	{
		if (rock.Any(c => c.x == 6))
			return rock;

		return rock.Select(c => (c.x + 1, c.y)).ToHashSet();
	}

	static HashSet<(int x, long y)> ShiftUp(HashSet<(int x, long y)> rock) =>
		rock.Select(c => (c.x, c.y + 1)).ToHashSet();
}
