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

void RecordAnswer(Dictionary<string, long> answers, Dictionary<string, List<Unsolved>> waitingOn, string name, long value)
{
	answers[name] = value;

	if (!waitingOn.TryGetValue(name, out var items))
		return;

	foreach (var item in items.ToList())
	{
		if ((item.lhs is string lhsName) && lhsName == name)
		{
			item.lhs = value;
			var result = item.Solve();
			if (result != null)
			{
				items.Remove(item);
				if (items.Count == 0)
					waitingOn.Remove(name);

				RecordAnswer(answers, waitingOn, item.name, result.Value);
			}
		}

		if ((item.rhs is string rhsName) && rhsName == name)
		{
			item.rhs = value;
			var result = item.Solve();
			if (result != null)
			{
				items.Remove(item);
				if (items.Count == 0)
					waitingOn.Remove(name);

				RecordAnswer(answers, waitingOn, item.name, result.Value);
			}
		}
	}
}

long RecordAnswerReverse(Dictionary<string, long> answers, Dictionary<string, Unsolved> problems, string name, long value)
{
	answers[name] = value;

	while (true)
	{
		var problem = problems[name];
		var result = problem.SolveReverse(value);

		if (result.name == "humn")
			return result.value;

		name = result.name;
		value = result.value;
	}
}

long GetPart1()
{
	var answers = new Dictionary<string, long>();
	var waitingOn = new Dictionary<string, List<Unsolved>>();

	foreach (var line in data)
	{
		var pieces = line.Split(' ');
		var name = pieces[0].TrimEnd(':');

		if (pieces.Length == 2)
			answers.Add(name, long.Parse(pieces[1]));
		else
		{
			var lhs = pieces[1];
			var rhs = pieces[3];
			var unsolved = new Unsolved(name, lhs, pieces[2][0], rhs);
			if (!waitingOn.ContainsKey(lhs))
				waitingOn.Add(lhs, new());
			if (!waitingOn.ContainsKey(rhs))
				waitingOn.Add(rhs, new());

			waitingOn[lhs].Add(unsolved);
			waitingOn[rhs].Add(unsolved);
		}
	}

	foreach (var answer in answers.ToList())
		RecordAnswer(answers, waitingOn, answer.Key, answer.Value);

	return answers["root"];
}

long GetPart2()
{
	var answers = new Dictionary<string, long>();
	var problems = new Dictionary<string, Unsolved>();
	var waitingOn = new Dictionary<string, List<Unsolved>>();

	foreach (var line in data)
	{
		var pieces = line.Split(' ');
		var name = pieces[0].TrimEnd(':');

		if (pieces.Length == 2)
		{
			if (name != "humn")
				answers.Add(name, long.Parse(pieces[1]));
		}
		else
		{
			var lhs = pieces[1];
			var rhs = pieces[3];
			var operation = name == "root" ? '=' : pieces[2][0];
			var unsolved = new Unsolved(name, lhs, operation, rhs);
			problems.Add(name, unsolved);
			if (!waitingOn.ContainsKey(lhs))
				waitingOn.Add(lhs, new());
			if (!waitingOn.ContainsKey(rhs))
				waitingOn.Add(rhs, new());

			waitingOn[lhs].Add(unsolved);
			waitingOn[rhs].Add(unsolved);
		}
	}

	foreach (var answer in answers.ToList())
		RecordAnswer(answers, waitingOn, answer.Key, answer.Value);

	var root = problems["root"];
	var solvedName = root.lhs as string ?? root.rhs as string ?? throw new InvalidOperationException();
	var solvedValue = root.lhs is long lhsLong ? lhsLong : root.rhs is long rhsLong ? rhsLong : throw new InvalidOperationException();
	return RecordAnswerReverse(answers, problems, solvedName, solvedValue);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

class Unsolved
{
	public Unsolved(string name, string lhs, char operation, string rhs)
	{
		this.name = name;
		this.lhs = lhs;
		this.operation = operation;
		this.rhs = rhs;
	}

	public string name;
	public object lhs;
	public object rhs;
	public char operation;

	public long? Solve()
	{
		if (lhs is not long lhsLong || rhs is not long rhsLong)
			return null;

		return operation switch
		{
			'=' => null,
			'+' => lhsLong + rhsLong,
			'-' => lhsLong - rhsLong,
			'*' => lhsLong * rhsLong,
			'/' => lhsLong / rhsLong,
			_ => throw new NotImplementedException(),
		};
	}

	public (string name, long value) SolveReverse(long answer)
	{
		if (lhs is string unknown)
		{
			var known = (long)rhs;

			return operation switch
			{
				'+' => (unknown, answer - known),
				'-' => (unknown, answer + known),
				'*' => (unknown, answer / known),
				'/' => (unknown, answer * known),
				_ => throw new InvalidOperationException(),
			};
		}
		else
		{
			unknown = (string)rhs;
			var known = (long)lhs;

			return operation switch
			{
				'+' => (unknown, answer - known),
				'-' => (unknown, known - answer),
				'*' => (unknown, answer / known),
				'/' => (unknown, known / answer),
				_ => throw new InvalidOperationException(),
			};
		}
	}

	public override string ToString() => $"{name}: {lhs} {operation} {rhs}";
}
