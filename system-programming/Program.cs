using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Select mode (1 - Single, 2 - Multi): ");
        string mode = Console.ReadLine();

        Console.WriteLine("Enter the JSON file paths: ");
        string input = Console.ReadLine();
        string[] filePaths = input.Split(',');

        CancellationTokenSource cts = new CancellationTokenSource();

        Console.WriteLine("Press 'c' to cancel.");
        Task.Run(() =>
        {
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    cts.Cancel();
                    break;
                }
            }
        });

        if (mode == "1")
        {
            await ProcessFilesSingleMode(filePaths, cts.Token);
        }
        else if (mode == "2")
        {
            await ProcessFilesMultiMode(filePaths, cts.Token);
        }
        else
        {
            Console.WriteLine("Invalid mode selected.");
        }
    }

    static async Task ProcessFilesSingleMode(string[] filePaths, CancellationToken token)
    {
        foreach (var path in filePaths)
        {
            token.ThrowIfCancellationRequested();
            await Task.Run(() => ReadAndDisplayJson(path));
        }
    }

    static async Task ProcessFilesMultiMode(string[] filePaths, CancellationToken token)
    {
        var tasks = new List<Task>();

        foreach (var path in filePaths)
        {
            token.ThrowIfCancellationRequested();
            tasks.Add(Task.Run(() => ReadAndDisplayJson(path), token));
        }

        await Task.WhenAll(tasks);
    }

    static void ReadAndDisplayJson(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            Console.WriteLine($"Content of {Path.GetFileName(path)}:\n{jsonContent}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file {path}: {ex.Message}");
        }
    }
}
