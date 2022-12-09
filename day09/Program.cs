using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select (dir: line[0], dist: int.Parse(line[2..]))).ToArray();

static (int x, int y) MoveHead((int x, int y) head, char direction)
{
	switch (direction)
	{
		case 'R': head.x++; break;
		case 'L': head.x--; break;
		case 'U': head.y--; break;
		case 'D': head.y++; break;
		default:
			throw new InvalidOperationException();
	}

	return head;
}

(int x, int y) MoveFollower((int x, int y) leader, (int x, int y) follower)
{
	var dx = leader.x - follower.x;
	var dy = leader.y - follower.y;
	var result = follower;

	if (dy == 2)
	{
		result.y++;
		result.x += dx > 0 ? 1 : (dx < 0 ? -1 : 0);
	}
	else if (dy == -2)
	{
		result.y--;
		result.x += dx > 0 ? 1 : (dx < 0 ? -1 : 0);
	}
	else if (dx == 2)
	{
		result.x++;
		result.y += dy > 0 ? 1 : (dy < 0 ? -1 : 0);
	}
	else if (dx == -2)
	{
		result.x--;
		result.y += dy > 0 ? 1 : (dy < 0 ? -1 : 0);
	}

	return result;
}

long GetPart1()
{
	var head = (x: 0, y: 0);
	var tail = (x: 0, y: 0);
	var tailVisited = new HashSet<(int, int)>();

	foreach (var instruction in data)
	{
		var count = instruction.dist;

		while (count > 0)
		{
			head = MoveHead(head, instruction.dir);
			tail = MoveFollower(head, tail);

			tailVisited.Add(tail);

			--count;
		}
	}

	return tailVisited.Count;
}

long GetPart2()
{
	var head = (x: 0, y: 0);
	var tails = new List<(int x, int y)> { (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0) };
	var tailVisited = new HashSet<(int x, int y)>();

	foreach (var instruction in data)
	{
		var count = instruction.dist;

		while (count > 0)
		{
			head = MoveHead(head, instruction.dir);
			tails[0] = MoveFollower(head, tails[0]);
			tails[1] = MoveFollower(tails[0], tails[1]);
			tails[2] = MoveFollower(tails[1], tails[2]);
			tails[3] = MoveFollower(tails[2], tails[3]);
			tails[4] = MoveFollower(tails[3], tails[4]);
			tails[5] = MoveFollower(tails[4], tails[5]);
			tails[6] = MoveFollower(tails[5], tails[6]);
			tails[7] = MoveFollower(tails[6], tails[7]);
			tails[8] = MoveFollower(tails[7], tails[8]);

			tailVisited.Add(tails[8]);

			--count;
		}
	}

	return tailVisited.Count;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
