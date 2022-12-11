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

(List<Monkey> monkeys, long lcm) GetMonkeys()
{
	var idx = 0;
	var monkeys = new List<Monkey>();

	while (idx < data!.Length)
	{
		Func<long, long> operation;
		var items = data[idx + 1][18..].Split(",").Select(long.Parse).ToList();
		var opText = data[idx + 2][23..];
		var divisible = long.Parse(data[idx + 3][21..]);
		var trueMonkey = int.Parse(data[idx + 4][29..]);
		var falseMonkey = int.Parse(data[idx + 5][30..]);

		if (opText[0] == '+')
		{
			if (opText[2..] == "old")
				operation = (x) => x + x;
			else
				operation = (x) => x + long.Parse(opText[2..]);
		}
		else
		{
			if (opText[2..] == "old")
				operation = (x) => x * x;
			else
				operation = (x) => x * long.Parse(opText[2..]);
		}

		var monkey = new Monkey(items, operation, divisible, trueMonkey, falseMonkey);
		monkeys.Add(monkey);
		idx += 6;
	}

	var leastCommonMultiple = 1L;
	foreach (var monkey in monkeys)
		leastCommonMultiple *= monkey.DivisibleTest;

	return (monkeys, leastCommonMultiple);
}

void RunMonkey(List<Monkey> monkeys, Monkey monkey, bool divideBy3, long leastCommonMultiple)
{
	var currentItems = monkey.Items.ToArray();
	monkey.Items.Clear();

	foreach (var item in currentItems)
	{
		monkey.Inspections++;

		var newItem = monkey.Operation(item);
		if (divideBy3)
			newItem /= 3;

		newItem %= leastCommonMultiple;

		if (newItem % monkey.DivisibleTest == 0)
			monkeys[monkey.TrueMonkey].Items.Add(newItem);
		else
			monkeys[monkey.FalseMonkey].Items.Add(newItem);
	}
}

long GetPart1()
{
	var (monkeys, lcm) = GetMonkeys();

	for (var round = 1; round <= 20; ++round)
		for (var idx = 0; idx < monkeys.Count; ++idx)
			RunMonkey(monkeys, monkeys[idx], true, lcm);

	var topTwo = monkeys.OrderByDescending(m => m.Inspections).Take(2).ToArray();
	return topTwo[0].Inspections * topTwo[1].Inspections;
}

long GetPart2()
{
	var (monkeys, lcm) = GetMonkeys();

	for (var round = 1; round <= 10_000; ++round)
		for (var idx = 0; idx < monkeys.Count; ++idx)
			RunMonkey(monkeys, monkeys[idx], false, lcm);

	var topTwo = monkeys.OrderByDescending(m => m.Inspections).Take(2).ToArray();
	return topTwo[0].Inspections * topTwo[1].Inspections;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class Monkey
{
	public Monkey(List<long> items, Func<long, long> operation, long divisibleTest, int trueMonkey, int falseMonkey)
	{
		Items = items;
		Operation = operation;
		DivisibleTest = divisibleTest;
		TrueMonkey = trueMonkey;
		FalseMonkey = falseMonkey;
	}

	public List<long> Items;
	public Func<long, long> Operation;
	public long DivisibleTest;
	public int TrueMonkey;
	public int FalseMonkey;
	public long Inspections = 0L;
}
