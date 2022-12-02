using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

Choice ToChoice(string value) =>
	value switch
	{
		"A" or "X" => Choice.Rock,
		"B" or "Y" => Choice.Paper,
		"C" or "Z" => Choice.Scissors,
		_ => throw new InvalidOperationException(),
	};

Choice ToChoiceFromGoal(Choice opponent, string winTieOrLose)
{
	if (winTieOrLose == "Y")
		return opponent;

	return (opponent, winTieOrLose == "Z") switch
	{
		(Choice.Rock, false) => Choice.Scissors,
		(Choice.Rock, true) => Choice.Paper,

		(Choice.Paper, false) => Choice.Rock,
		(Choice.Paper, true) => Choice.Scissors,

		(Choice.Scissors, false) => Choice.Paper,
		(Choice.Scissors, true) => Choice.Rock,

		_ => throw new InvalidOperationException(),
	};
}

long ToChoiceScore(Choice me) =>
	me switch
	{
		Choice.Rock => 1,
		Choice.Paper => 2,
		_ => 3,
	};

long ToWinnerScore(Choice opponent, Choice me)
{
	if (opponent == me)
		return 3;

	var doIWin = me switch
	{
		Choice.Rock => opponent == Choice.Scissors,
		Choice.Paper => opponent == Choice.Rock,
		_ => opponent == Choice.Paper,
	};

	return doIWin ? 6 : 0;
}

long GetPart1()
{
	var score = 0L;

	foreach (var line in data)
	{
		var pieces = line.Split(" ");
		var opponent = ToChoice(pieces[0]);
		var me = ToChoice(pieces[1]);
		var lineScore = ToWinnerScore(opponent, me) + ToChoiceScore(me);
		score += lineScore;
	}

	return score;
}

long GetPart2()
{
	var score = 0L;

	foreach (var line in data)
	{
		var pieces = line.Split(" ");
		var opponent = ToChoice(pieces[0]);
		var me = ToChoiceFromGoal(opponent, pieces[1]);
		var lineScore = ToWinnerScore(opponent, me) + ToChoiceScore(me);
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

enum Choice { Rock, Paper, Scissors };
