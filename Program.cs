using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Console = Colorful.Console;
using System.Drawing;
using System.Diagnostics;
using System.Net.Http;

public class Program
{
    private static readonly string[] SuspiciousFolderNames = { "Temp", "Hidden", "$$Recycle.Bin" };
    private static long _scannedFiles = 0;
    private static long _totalFiles = 0;
    private static bool _scanComplete = false;
    private static readonly Stopwatch _stopwatch = new Stopwatch();

    static async Task Main(string[] args)
    {
        if (!IsAdministrator())
        {
            Console.WriteLine("[Error: Please run the program as an administrator.]", Color.Red);
            return;
        }

        Console.Title = "Celine-v2";
        Console.Clear();
        DisplayHeader();

        string latestVersion = await CheckVersionAsync("https://raw.githubusercontent.com/playboifusi/Celine/main/bin2/version2.txt");

        if (latestVersion == null || latestVersion != "0.0.2")
        {
            Console.WriteLine("[Error: Update required. Exiting...]", Color.Red);
            return;
        }

        string celinePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "celine-v2");
        InitializeDirectories(celinePath);

        Console.WriteLine("[Initializing Scan...]", Color.BlueViolet);
        Thread.Sleep(1500); // wait for 1.5 seconds cool ig

        _stopwatch.Start();

        var cancellationTokenSource = new CancellationTokenSource();
        var rotatingIndicatorTask = ShowRotatingIndicatorAsync(cancellationTokenSource.Token);

        await PerformFileCheckAsync(celinePath, cancellationTokenSource.Token);

        _scanComplete = true;
        cancellationTokenSource.Cancel();
        await rotatingIndicatorTask;

        _stopwatch.Stop();
    }

    static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    static void DisplayHeader()
    {
        string asciiArt = @"
    ╔════════════════════════════════════════╗
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@&##&@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@#BGGGBB&@@&BPGGPB@@@@@@@║
    ║@@@@@@@@@@@@@@BPPGGGGGGGGBP5PGPB@@@@@@@@║
    ║@@@@@@@@@@@@#PP5PGPGGGGGGPPPPP@@@@@@@@@@║
    ║@@@@@@@@@@@BPP5PPP#@GPGGGGGPPB@@@@@@@@@@║
    ║@@@@@@@@@@BPP5PPP&@@&5GGGGPP&@@@@@@@@@@@║
    ║@@@@@@@@@#5PPPPP@@@@@PPPPP5B@@@@@@@@@@@@║
    ║@@@@@@@@@PPP5P5&@@@@@&P5PP5&@@@@@@@@@@@@║
    ║@@@@@@@@#5PPP5G@@@@@@@@#BGG#&@@@@@@@@@@@║
    ║@@@@@@@@B55555B@@@@@@@@@B5YY5G@@@@@@@@@@║
    ║@@@@@@@@BY555YB@@@@@@@#PY5555#@@@@@@@@@@║
    ║@@@@@@@@&55555P@@@@&BPYY55YG&@@@@@@@@@@@║
    ║@@@@@@@@@#5YY5YPBGP5YY55YYB@@@@@@@@@@@@@║
    ║@@@@@@@@@@@BP5YYYYYYYYY5P#@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@&#BBGGGGB#@@@@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@║
    ╚════════════════════════════════════════╝
    ";

        var headerColor = Color.BlueViolet;
        var asciiColor = Color.BlueViolet;

        Console.WriteLine($@"
╔═════════════════════════════════════════╗
║ Owner  : Opium | zyna                   ║
║ GitHub : https://github.com/playboifusi ║
║ Version: 0.0.2                          ║
╚═════════════════════════════════════════╝
", headerColor);
        Console.WriteLine();
        PrintCentered(asciiArt, asciiColor);
    }

    static void PrintCentered(string text, Color color)
    {
        int consoleWidth = Console.WindowWidth;
        foreach (var line in text.Split('\n'))
        {
            string trimmedLine = line.Trim();
            if (string.IsNullOrEmpty(trimmedLine)) continue;
            int leftPadding = (consoleWidth - trimmedLine.Length) / 2;
            Console.WriteLine(new string(' ', leftPadding) + trimmedLine, color);
        }
    }

    static void InitializeDirectories(string path)
    {
        Directory.CreateDirectory(path);
        Directory.CreateDirectory(Path.Combine(path, "logSettings"));
        File.WriteAllText(Path.Combine(path, "logSettings", "FindDeletedFiles.txt"), "true");
    }

    static async Task<string?> CheckVersionAsync(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                string version = await client.GetStringAsync(url);
                return version?.Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error: {ex.Message}]", Color.Red);
                return null;
            }
        }
    }

    static async Task PerformFileCheckAsync(string celinePath, CancellationToken cancellationToken)
    {
        string logFileName = $"Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string logFilePath = Path.Combine(celinePath, logFileName);

        var logEntries = new ConcurrentBag<string>();

        var directoriesToScan = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Documents")
        };

        _totalFiles = await CountTotalFilesAsync(directoriesToScan);

        var scanTasks = directoriesToScan.Select(dir => ScanDirectoryAsync(dir, logEntries, cancellationToken)).ToArray();

        await Task.WhenAll(scanTasks);

        try
        {
            await File.WriteAllLinesAsync(logFilePath, logEntries);
            Console.WriteLine($"[Scan Complete, Find results at {logFilePath}]", Color.BlueViolet);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error writing log file: {ex.Message}]", Color.Red);
        }
    }

    static async Task<long> CountTotalFilesAsync(string[] directories)
    {
        long total = 0;
        foreach (var dir in directories)
        {
            total += await CountFilesInDirectoryAsync(dir);
        }
        return total;
    }

    static async Task<long> CountFilesInDirectoryAsync(string directory)
    {
        long count = 0;
        try
        {
            count += Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly).Length;
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                count += await CountFilesInDirectoryAsync(subDir);
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (Exception) { }
        return count;
    }

    static async Task ScanDirectoryAsync(string directory, ConcurrentBag<string> logEntries, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var file in Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly))
            {
                if (cancellationToken.IsCancellationRequested) return;

                Interlocked.Increment(ref _scannedFiles);
                
                FileInfo fileInfo = new FileInfo(file);
                
                // Check file size
                if (fileInfo.Length > 100 * 1024 * 1024) // larger than 100 MB
                {
                    logEntries.Add($"[LARGE FILE] {file} - Size: {fileInfo.Length / (1024 * 1024)} MB");
                }

                // Check file extension
                string extension = Path.GetExtension(file).ToLower();
                if (new[] { ".exe", ".dll", ".bat" }.Contains(extension))
                {
                    logEntries.Add($"[EXECUTABLE] {file}");
                }

                // Check for hidden files
                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    logEntries.Add($"[HIDDEN] {file}");
                }

                // Check file creation and modification times
                if (fileInfo.CreationTime > DateTime.Now.AddDays(-7) || 
                    fileInfo.LastWriteTime > DateTime.Now.AddDays(-7))
                {
                    logEntries.Add($"[RECENT] {file} - Created: {fileInfo.CreationTime}, Modified: {fileInfo.LastWriteTime}");
                }

                // Check for suspicious file names
                string fileName = Path.GetFileName(file).ToLower();
                if (fileName.Contains("cheat") || fileName.Contains("loader") || fileName.Contains("installer") || fileName.Contains("map"))
                {
                    logEntries.Add($"[SUSPICIOUS] {file}");
                }
            }

            foreach (var subDir in Directory.EnumerateDirectories(directory))
            {
                if (cancellationToken.IsCancellationRequested) return;

                string folderName = Path.GetFileName(subDir);
                if (SuspiciousFolderNames.Contains(folderName))
                {
                    logEntries.Add($"[SUSPICIOUS FOLDER] {subDir}");
                }

                await ScanDirectoryAsync(subDir, logEntries, cancellationToken);
            }
        }
        catch (UnauthorizedAccessException)
        {
            logEntries.Add($"[ACCESS DENIED] {directory}");
        }
        catch (Exception ex)
        {
            logEntries.Add($"[ERROR] {directory} - {ex.Message}");
        }
    }

    static async Task ShowRotatingIndicatorAsync(CancellationToken cancellationToken)
    {
        var spinner = new[] { '|', '/', '-', '\\' };
        int counter = 0;

        Console.CursorVisible = false;
        Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
        Console.SetCursorPosition(0, Console.CursorTop);

        while (!cancellationToken.IsCancellationRequested && !_scanComplete)
        {
            long scannedFiles = Interlocked.Read(ref _scannedFiles);
            double progress = _totalFiles > 0 ? (double)scannedFiles / _totalFiles * 100 : 0;
            double speed = _stopwatch.Elapsed.TotalSeconds > 0 ? scannedFiles / _stopwatch.Elapsed.TotalSeconds : 0;
            TimeSpan estimatedTimeRemaining = speed > 0 ? TimeSpan.FromSeconds((_totalFiles - scannedFiles) / speed) : TimeSpan.Zero;

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"[{spinner[counter % spinner.Length]}] Scanning {progress:0.00}% - Files Scanned: {scannedFiles}/{_totalFiles}, Scan Speed: {speed:0.00} files/s, Estimated Time Remaining: {estimatedTimeRemaining:hh\\:mm\\:ss}|");

            counter++;
            await Task.Delay(100);
        }

        Console.SetCursorPosition(0, Console.CursorTop);
        Console.WriteLine($"[/] Scan Complete - Total Files Scanned: {_scannedFiles}/{_totalFiles}, Time Elapsed: {_stopwatch.Elapsed.TotalSeconds:0.00} seconds");
        Console.CursorVisible = true;
    }
}
