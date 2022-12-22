using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

var map = data[0..(data.Length - 2)].Select(d => d.ToArray()).ToArray();

const int RIGHT = 0;
const int DOWN = 1;
const int LEFT = 2;
const int UP = 3;

long GetPart1()
{
	var posX = 0;
	var posY = 0;
	var direction = 0;

	var instructions = data[data.Length - 1];
	var instructionIdx = 0;

	while (posX < map[0].Length)
		if (map[0][posX] == ' ')
			posX++;
		else
			break;

	while (instructionIdx < instructions.Length)
	{
		if (char.IsAsciiLetter(instructions[instructionIdx]))
		{
			direction = instructions[instructionIdx] switch
			{
				'R' => direction + 1,
				'L' => direction - 1,
				_ => throw new InvalidOperationException(),
			};

			if (direction == -1)
				direction = 3;
			else if (direction == 4)
				direction = 0;

			instructionIdx++;
		}
		else
		{
			var endIdx = instructionIdx;
			while (endIdx < instructions.Length && char.IsNumber(instructions[endIdx]))
				++endIdx;

			for (var number = int.Parse(instructions[instructionIdx..endIdx]); number > 0; --number)
			{
				var resultX = posX;
				var resultY = posY;

				if (direction == RIGHT)
				{
					resultX++;
					if (resultX >= map[resultY].Length)
						resultX = 0;

					while (map[resultY][resultX] == ' ')
					{
						resultX++;
						if (resultX >= map[resultY].Length)
							resultX = 0;
					}
				}
				else if (direction == DOWN)
				{
					resultY++;
					if (resultY >= map.Length)
						resultY -= map.Length;

					while (resultX >= map[resultY].Length || map[resultY][resultX] == ' ')
					{
						resultY++;
						if (resultY >= map.Length)
							resultY -= map.Length;
					}
				}
				else if (direction == LEFT)
				{
					resultX--;
					if (resultX < 0)
						resultX += map[resultY].Length;

					while (map[resultY][resultX] == ' ')
					{
						resultX--;
						if (resultX < 0)
							resultX += map[resultY].Length;
					}
				}
				else if (direction == UP)
				{
					resultY--;
					if (resultY < 0)
						resultY += map.Length;

					while (resultX >= map[resultY].Length || map[resultY][resultX] == ' ')
					{
						resultY--;
						if (resultY < 0)
							resultY += map.Length;
					}
				}

				if (map[resultY][resultX] != '#')
				{
					posX = resultX;
					posY = resultY;
				}
			}

			instructionIdx = endIdx;
		}
	}

	return 1000 * (posY + 1) + 4 * (posX + 1) + direction;
}

long GetPart2()
{
	var emptySpaceMaps = new Dictionary<(int x, int y, int direction), (int x, int y, int directionChange)>();

	if (map.Length == 12)  // Sample data
	{
		for (int i = 0; i < 4; ++i)
		{
			// 0 left to 4 top
			emptySpaceMaps.Add((7, i, LEFT), (4 + i, 4, -1));
			emptySpaceMaps.Add((4 + i, 3, UP), (8, i, +1));
			// 0 top to 3 top
			emptySpaceMaps.Add((i + 8, -1, UP), (3 - i, 4, -2));
			emptySpaceMaps.Add((i, 3, UP), (11 - i, 0, +2));
			// 0 right to 5 right
			emptySpaceMaps.Add((12, i, RIGHT), (15, 11 - i, -2));
			emptySpaceMaps.Add((16, i + 8, RIGHT), (11, 3 - i, 2));
			// 1 right to 5 top
			emptySpaceMaps.Add((12, i + 4, RIGHT), (15 - i, 8, +1));
			emptySpaceMaps.Add((12 + i, 7, UP), (11, 7 - i, -1));
			// 2 left to 4 bottom
			emptySpaceMaps.Add((7, 8 + i, LEFT), (7 - i, 7, +1));
			emptySpaceMaps.Add((i + 4, 8, DOWN), (11 - i, 11, -1));
			// 2 bottom to 3 bottom
			emptySpaceMaps.Add((i + 8, 12, DOWN), (3 - i, 7, +2));
			emptySpaceMaps.Add((i, 8, DOWN), (11 - i, 11, -2));
			// 3 left to 5 bottom
			emptySpaceMaps.Add((-1, i + 4, LEFT), (15 - i, 11, +1));
			emptySpaceMaps.Add((12 + i, 12, DOWN), (0, 7 - i, -1));
		}
	}
	else  // The real data -- hand crafted for our known data set (not general purpose)
	{
		for (int i = 0; i < 50; ++i)
		{
			// 1 top to 6 left
			emptySpaceMaps.Add((50 + i, -1, UP), (0, 150 + i, +1));
			emptySpaceMaps.Add((-1, 150 + i, LEFT), (50 + i, 0, -1));
			// 1 left to 5 left
			emptySpaceMaps.Add((49, i, LEFT), (0, 149 - i, 2));
			emptySpaceMaps.Add((-1, 100 + i, LEFT), (50, 49 - i, -2));
			// 2 top to 6 bottom
			emptySpaceMaps.Add((100 + i, -1, UP), (i, 199, 0));
			emptySpaceMaps.Add((i, 200, DOWN), (100 + i, 0, 0));
			// 2 right to 4 right
			emptySpaceMaps.Add((150, i, RIGHT), (99, 149 - i, 2));
			emptySpaceMaps.Add((100, 100 + i, RIGHT), (149, 49 - i, -2));
			// 2 bottom to 3 right
			emptySpaceMaps.Add((100 + i, 50, DOWN), (99, 50 + i, +1));
			emptySpaceMaps.Add((100, 50 + i, RIGHT), (100 + i, 49, -1));
			// 3 left to 5 top
			emptySpaceMaps.Add((49, 50 + i, LEFT), (i, 100, -1));
			emptySpaceMaps.Add((i, 99, UP), (50, 50 + i, +1));
			// 4 bottom to 6 right
			emptySpaceMaps.Add((50 + i, 150, DOWN), (49, 150 + i, +1));
			emptySpaceMaps.Add((50, 150 + i, RIGHT), (50 + i, 149, -1));
		}
	}

	var posX = 0;
	var posY = 0;
	var direction = 0;
	var instructions = data[data.Length - 1];
	var instructionIdx = 0;

	while (posX < map[0].Length)
		if (map[0][posX] == ' ')
			posX++;
		else
			break;

	while (instructionIdx < instructions.Length)
	{
		if (char.IsAsciiLetter(instructions[instructionIdx]))
		{
			direction = instructions[instructionIdx] switch
			{
				'R' => direction + 1,
				'L' => direction - 1,
				_ => throw new InvalidOperationException(),
			};

			if (direction == -1)
				direction = 3;
			else if (direction == 4)
				direction = 0;

			instructionIdx++;
		}
		else
		{
			var endIdx = instructionIdx;
			while (endIdx < instructions.Length && char.IsNumber(instructions[endIdx]))
				++endIdx;

			var spaces = int.Parse(instructions[instructionIdx..endIdx]);

			for (var number = spaces; number > 0; --number)
			{
				var resultX = posX;
				var resultY = posY;
				var directionChange = 0;

				if (direction == 0)
					resultX++;
				else if (direction == 1)
					resultY++;
				else if (direction == 2)
					resultX--;
				else if (direction == 3)
					resultY--;

				if (emptySpaceMaps.TryGetValue((resultX, resultY, direction), out var nextSpace))
					(resultX, resultY, directionChange) = nextSpace;

				if (map[resultY][resultX] != '#')
				{
					posX = resultX;
					posY = resultY;

					direction += directionChange;
					if (direction < 0)
						direction += 4;
					else if (direction > 3)
						direction -= 4;
				}
			}

			instructionIdx = endIdx;
		}
	}

	return 1000 * (posY + 1) + 4 * (posX + 1) + direction;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
