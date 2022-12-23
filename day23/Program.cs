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

var initialState = new HashSet<(long x, long y)>();

for (int y = 0; y < data.Length; ++y)
	for (int x = 0; x < data[y].Length; ++x)
		if (data[y][x] == '#')
			initialState.Add((x, y));

var northMove = new HashSet<string> { "nw", "n", "ne" };
var eastMove = new HashSet<string> { "ne", "e", "se" };
var southMove = new HashSet<string> { "sw", "s", "se" };
var westMove = new HashSet<string> { "nw", "w", "sw" };

void ProposeMove(
	int moveIdx,
	HashSet<(long x, long y)> state,
	(long x, long y) elf,
	Dictionary<(long x, long y), (long x, long y)> proposals,
	Dictionary<(long x, long y), long> proposalCounts)
{
	HashSet<string> occupied = new();

	if (state.Contains((elf.x - 1, elf.y - 1)))
		occupied.Add("nw");
	if (state.Contains((elf.x, elf.y - 1)))
		occupied.Add("n");
	if (state.Contains((elf.x + 1, elf.y - 1)))
		occupied.Add("ne");
	if (state.Contains((elf.x - 1, elf.y)))
		occupied.Add("w");
	if (state.Contains((elf.x + 1, elf.y)))
		occupied.Add("e");
	if (state.Contains((elf.x - 1, elf.y + 1)))
		occupied.Add("sw");
	if (state.Contains((elf.x, elf.y + 1)))
		occupied.Add("s");
	if (state.Contains((elf.x + 1, elf.y + 1)))
		occupied.Add("se");

	(long x, long y)? proposal = null;

	if (occupied.Count > 0)
		for (int move = moveIdx; move < moveIdx + 4; ++move)
		{
			if (move % 4 == 0 && !occupied.Intersect(northMove).Any())
				proposal = (elf.x, elf.y - 1);
			else if (move % 4 == 1 && !occupied.Intersect(southMove).Any())
				proposal = (elf.x, elf.y + 1);
			else if (move % 4 == 2 && !occupied.Intersect(westMove).Any())
				proposal = (elf.x - 1, elf.y);
			else if (move % 4 == 3 && !occupied.Intersect(eastMove).Any())
				proposal = (elf.x + 1, elf.y);

			if (proposal != null)
				break;
		}

	if (proposal != null)
	{
		if (!proposalCounts.ContainsKey(proposal.Value))
			proposalCounts.Add(proposal.Value, 1);
		else
			proposalCounts[proposal.Value]++;

		proposals.Add(elf, proposal.Value);
	}
}

long MoveElves(
	HashSet<(long x, long y)> state,
	long maxRounds = long.MaxValue)
{
	var moveIdx = 0;
	var round = 1L;

	while (true)
	{
		var proposals = new Dictionary<(long x, long y), (long x, long y)>();
		var proposalCounts = new Dictionary<(long x, long y), long>();

		foreach (var elf in state)
			ProposeMove(moveIdx, state, elf, proposals, proposalCounts);

		foreach (var proposal in proposals.Where(p => proposalCounts[p.Value] == 1))
		{
			state.Remove(proposal.Key);
			state.Add(proposal.Value);
		}

		if (proposalCounts.Count == 0 || round == maxRounds)
			return round;

		moveIdx = (moveIdx + 1) % 4;
		++round;
	}
}

long GetPart1()
{
	var state = initialState.ToHashSet();

	MoveElves(state, 10);

	var minX = state.Select(s => s.x).Min();
	var maxX = state.Select(s => s.x).Max();
	var minY = state.Select(s => s.y).Min();
	var maxY = state.Select(s => s.y).Max();
	var totalSpaces = (maxX - minX + 1) * (maxY - minY + 1);

	return totalSpaces - state.Count();
}

long GetPart2() =>
	MoveElves(initialState.ToHashSet());

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
