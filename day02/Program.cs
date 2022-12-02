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

var scores = new Dictionary<string, long>
{
	{ "X", 1 },
	{ "Y", 2 },
	{ "Z", 3 },
};

var names = new Dictionary<string, string>
{
	{ "A", "rock" },
	{ "B", "paper" },
	{ "C", "scissors" },
	{ "X", "rock" },
	{ "Y", "paper" },
	{ "Z", "scissors" },
};

long GetPart1()
{
	bool? isWinner(string opponent, string me)
	{
		return (opponent, me) switch
		{
			("A", "X") => null,
			("A", "Y") => true,
			("A", "Z") => false,

			("B", "X") => false,
			("B", "Y") => null,
			("B", "Z") => true,

			("C", "X") => true,
			("C", "Y") => false,
			("C", "Z") => null,
			_ => throw new InvalidOperationException(),
		};
	}

	long winnerScore(bool? winner)
	{
		return winner switch
		{
			true => 6,
			false => 0,
			_ => 3,
		};
	}

	var score = 0L;

	foreach (var line in data)
	{
		var pieces = line.Split(" ");
		var opponent = pieces[0];
		var me = pieces[1];
		var winner = isWinner(opponent, me);
		var lineScore = scores[me] + winnerScore(winner);
		score += lineScore;
	}

	return score;
}

long GetPart2()
{
	string chooseWeapon(string opponent, string result)
	{
		return (opponent, result) switch
		{
			("A", "X") => "Z",
			("A", "Y") => "X",
			("A", "Z") => "Y",

			("B", "X") => "X",
			("B", "Y") => "Y",
			("B", "Z") => "Z",

			("C", "X") => "Y",
			("C", "Y") => "Z",
			("C", "Z") => "X",
			_ => throw new InvalidOperationException(),
		};
	}

	long winnerScore(string result)
	{
		return result switch
		{
			"X" => 0,
			"Y" => 3,
			"Z" => 6,
			_ => throw new InvalidOperationException(),
		};
	}

	var score = 0L;

	foreach (var line in data)
	{
		var pieces = line.Split(" ");
		var opponent = pieces[0];
		var result = pieces[1];
		var me = chooseWeapon(opponent, result);
		var lineScore = scores[me] + winnerScore(result);
		score += lineScore;
	}

	return score;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
