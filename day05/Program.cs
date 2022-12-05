using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

var startingCrates = new List<List<char>>();
var instructionStart = 0;
var instructionRegex = new Regex("move (\\d+) from (\\d+) to (\\d+)");

while (true)
{
	var line = data[instructionStart];

	if (line.IndexOf('[') < 0)
		break;

	var text = line;
	var crateIdx = 0;

	while (text != "")
	{
		List<char> crate;

		if (startingCrates.Count <= crateIdx)
		{
			crate = new List<char>();
			startingCrates.Add(crate);
		}
		else
			crate = startingCrates[crateIdx];

		if (text[1] != ' ')
			crate.Add(text[1]);

		if (text.Length < 4)
			break;

		text = text.Substring(4);
		++crateIdx;
	}

	++instructionStart;
}

instructionStart += 2;

List<Stack<char>> GetCrates()
{
	var crates = new List<Stack<char>>();

	foreach (var crate in startingCrates!)
		crates.Add(new Stack<char>(crate.Reverse<char>()));

	return crates;
}

(int count, int from, int to) ParseInstruction(string text)
{
	var match = instructionRegex!.Match(text);

	return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value) - 1, int.Parse(match.Groups[3].Value) - 1);
}

string GetTopItems(List<Stack<char>> crates)
{
	var result = "";

	foreach (var crate in crates)
		result += crate.Peek();

	return result;
}

string GetPart1()
{
	var crates = GetCrates();

	for (var lineIdx = instructionStart; lineIdx < data.Length; ++lineIdx)
	{
		var (count, from, to) = ParseInstruction(data[lineIdx]);

		for (; count > 0; --count)
			crates[to].Push(crates[from].Pop());
	}

	return GetTopItems(crates);
}

string GetPart2()
{
	var crates = GetCrates();

	for (var lineIdx = instructionStart; lineIdx < data.Length; ++lineIdx)
	{
		var (count, from, to) = ParseInstruction(data[lineIdx]);
		var tempStack = new Stack<char>();

		for (; count > 0; --count)
			tempStack.Push(crates[from].Pop());

		while (tempStack.Count > 0)
			crates[to].Push(tempStack.Pop());
	}

	return GetTopItems(crates);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
