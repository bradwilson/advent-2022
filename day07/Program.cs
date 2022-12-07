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

var folders = new Dictionary<string, Folder>();
var root = new Folder("/");
folders["/"] = root;

var currentPath = "/";
var currentFolder = root;

string GetFullPath(string baseFolder, string subFolder) =>
	Path.GetFullPath(Path.Combine("C:/", baseFolder, subFolder)).Replace("\\", "/").Replace("C:", "");

foreach (var instruction in data)
{
	if (instruction.StartsWith("$ cd "))
	{
		currentPath = GetFullPath(currentPath, instruction.Substring(5));
		currentFolder = folders[currentPath];
	}
	else if (instruction == "$ ls")
	{
		// We parse the results, nothing to do here
	}
	else if (instruction.StartsWith("dir "))
	{
		var newDirName = GetFullPath(currentPath, instruction.Substring(4));
		currentFolder.SubFolders.Add(newDirName);
		var childFolder = new Folder(newDirName);
		folders.Add(newDirName, childFolder);
	}
	else
	{
		var pieces = instruction.Split(' ');
		var size = long.Parse(pieces[0]);
		currentFolder.FileSizes += size;
	}
}

long GetPart1()
{
	var result = 0L;

	foreach (var folder in folders.Values)
	{
		var totalSize = folder.TotalSize(folders);
		if (totalSize <= 100000)
			result += totalSize;
	}

	return result;
}

long GetPart2()
{
	var totalSpace = 70000000;
	var targetSpace = 30000000;
	var unusedSpace = totalSpace - root.TotalSize(folders);
	var neededSpace = targetSpace - unusedSpace;

	var targetToDelete = root;
	var targetToDeleteSize = root.TotalSize(folders);

	foreach (var folder in folders.Values)
	{
		var totalSize = folder.TotalSize(folders);
		if (totalSize >= neededSpace && totalSize < targetToDeleteSize)
		{
			targetToDelete = folder;
			targetToDeleteSize = totalSize;
		}
	}

	return targetToDeleteSize;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");

public class Folder
{
	public Folder(string name)
	{
		Name = name;
		SubFolders = new();
		FileSizes = 0L;
	}

	public string Name;
	public List<string> SubFolders;
	public long FileSizes;

	public long TotalSize(Dictionary<string, Folder> folders)
	{
		var result = FileSizes;

		foreach (var subFolderName in SubFolders)
			result += folders[subFolderName].TotalSize(folders);

		return result;
	}
}
