using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select long.Parse(line)).ToArray();

long ComputeResult(LinkedListNode<long>[] nodes, LinkedList<long> linkedList)
{
	var result = 0L;
	var targetNode = nodes.Where(n => n.Value == 0L).Single();

	for (int i = 0; i < 3; ++i)
	{
		var moveCount = 1000 % linkedList.Count;
		while (moveCount-- > 0)
			targetNode = targetNode!.Next ?? linkedList.First;
		result += targetNode!.Value;
	}

	return result;
}

void Mix(LinkedListNode<long>[] nodes, LinkedList<long> linkedList)
{
	foreach (var item in nodes)
	{
		var moveQuantity = item.Value % (nodes.Length - 1);

		if (moveQuantity > 0)
		{
			var after = item.Next ?? linkedList.First;
			linkedList.Remove(item);

			while (moveQuantity-- > 0)
				after = after!.Next ?? linkedList.First;

			linkedList.AddBefore(after!, item);
		}
		else if (moveQuantity < 0)
		{
			var before = item.Previous ?? linkedList.Last;
			linkedList.Remove(item);

			while (moveQuantity++ < 0)
				before = before!.Previous ?? linkedList.Last;

			linkedList.AddAfter(before!, item);
		}
	}
}

long GetPart1()
{
	var nodes = data.Select(d => new LinkedListNode<long>(d)).ToArray();
	var linkedList = new LinkedList<long>();
	foreach (var item in nodes)
		linkedList.AddLast(item);

	Mix(nodes, linkedList);

	return ComputeResult(nodes, linkedList);
}

long GetPart2()
{
	var nodes = data.Select(d => new LinkedListNode<long>(d * 811589153)).ToArray();
	var linkedList = new LinkedList<long>();
	foreach (var item in nodes)
		linkedList.AddLast(item);

	for (int i = 0; i < 10; ++i)
		Mix(nodes, linkedList);

	return ComputeResult(nodes, linkedList);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
