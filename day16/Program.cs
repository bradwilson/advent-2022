using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var stopwatch = Stopwatch.StartNew();

var regex = new Regex("Valve ([A-Z][A-Z]) has flow rate=(\\d+); tunnels? leads? to valves? (.*)");

var valves =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 let match = regex.Match(line)
	 select new Valve(match.Groups[1].Value, long.Parse(match.Groups[2].Value), match.Groups[3].Value.Split(",").Select(x => x.Trim()).ToArray())).ToDictionary(v => v.Id, v => v);

var pathCostCache = new Dictionary<(string from, string to), int>();

int? GetPathCost(string from, string to, HashSet<string>? visited = null)
{
	if (pathCostCache.TryGetValue((from, to), out var cached))
		return cached;

	int? best = null;
	visited ??= new HashSet<string>();

	if (from == to)
		return null;

	visited.Add(from);

	var fromValve = valves[from];

	if (fromValve.Neighbors.Contains(to))
		best = 1;
	else
		foreach (var neighbor in fromValve.Neighbors)
		{
			if (!visited.Contains(neighbor))
			{
				var possibility = GetPathCost(neighbor, to, visited);
				if (possibility != null)
					if (best == null || possibility.Value + 1 < best.Value)
						best = possibility.Value + 1;
			}
		}

	visited.Remove(from);

	if (best == null)
		return null;

	pathCostCache[(from, to)] = best.Value;
	return best.Value;
}

long GetOptimumPressure(
	HashSet<string> openableValves,
	Valve valve,
	long pressure = 0L,
	int minute = 0)
{
	if (minute == 30 || openableValves.Count == 0)
		return pressure;

	var best = pressure;
	var minutesLeft = 30 - minute;

	foreach (var targetValveId in openableValves.ToArray())
	{
		var targetValve = valves[targetValveId];
		var targetPathCost = GetPathCost(valve.Id, targetValveId);

		if (targetPathCost != null && minutesLeft > targetPathCost.Value)
		{
			openableValves.Remove(targetValveId);

			var newMinute = minute + targetPathCost.Value + 1;
			var possibility = GetOptimumPressure(
				openableValves,
				targetValve,
				pressure + (30 - newMinute) * targetValve.FlowRate,
				newMinute
			);

			openableValves.Add(targetValveId);

			if (possibility > best)
				best = possibility;
		}
	}

	return best;
}

long GetOptimumPressurePair(
	HashSet<string> openableValves,
	Valve valve1,
	Valve valve2,
	long pressure = 0,
	int minute1 = 4,
	int minute2 = 4)
{
	if (minute1 == 30 || openableValves.Count == 0)
		return pressure;

	var best = pressure;
	var minutes1Left = 30 - minute1;
	var minutes2Left = 30 - minute2;

	foreach (var targetValveId1 in openableValves.ToArray())
	{
		var targetValve1 = valves[targetValveId1];
		var targetPathCost1 = GetPathCost(valve1.Id, targetValveId1);

		if (targetPathCost1 != null && minutes1Left > targetPathCost1.Value)
		{
			openableValves.Remove(targetValveId1);
			var newMinute1 = minute1 + targetPathCost1.Value + 1;
			var newPressure1 = pressure + (30 - newMinute1) * targetValve1.FlowRate;

			foreach (var targetValveId2 in openableValves.ToArray())
			{
				var targetValve2 = valves[targetValveId2];
				var targetPathCost2 = GetPathCost(valve2.Id, targetValveId2);

				if (targetPathCost2 != null && minutes2Left > targetPathCost2.Value)
				{
					openableValves.Remove(targetValveId2);
					var newMinute2 = minute2 + targetPathCost2.Value + 1;
					var newPressure2 = newPressure1 + (30 - newMinute2) * targetValve2.FlowRate;

					var potentialWith2 = GetOptimumPressurePair(openableValves, targetValve1, targetValve2, newPressure2, newMinute1, newMinute2);
					if (potentialWith2 > best)
						best = potentialWith2;

					openableValves.Add(targetValveId2);
				}
			}

			var potentialWithout2 = GetOptimumPressurePair(openableValves, targetValve1, valve2, newPressure1, newMinute1, minute2);
			if (potentialWithout2 > best)
				best = potentialWithout2;

			openableValves.Add(targetValveId1);
		}
	}

	return best;
}

long GetPart1()
{
	var openableValves = valves.Where(v => v.Value.FlowRate > 0).Select(v => v.Key).ToHashSet();
	return GetOptimumPressure(openableValves, valves["AA"]);
}

long GetPart2()
{
	var openableValves = valves.Where(v => v.Value.FlowRate > 0).Select(v => v.Key).ToHashSet();
	var aa = valves["AA"];
	return GetOptimumPressurePair(openableValves, aa, aa);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class Valve
{
	public Valve(string id, long flowRate, string[] neighbors)
	{
		Id = id;
		FlowRate = flowRate;
		Neighbors = neighbors;
	}

	public string Id { get; }
	public long FlowRate { get; }
	public string[] Neighbors { get; }
}
