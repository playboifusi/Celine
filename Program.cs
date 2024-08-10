    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Console = Colorful.Console;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Celine-v1";
            Console.Clear();  // Clear console before starting

            DisplayHeader();  // Call to print header

            string versionUrl = "https://raw.githubusercontent.com/playboifusi/Celine/main/bin2/version2.txt";
            string currentVersion = "0.0.3";

            string latestVersion = await CheckVersionAsync(versionUrl);
            if (latestVersion == null)
            {
                Console.WriteLine("[Error: Unable to check for updates. Exiting...]", Color.Red);
                return;
            }

            if (currentVersion != latestVersion)
            {
                Console.WriteLine("[A new version of Celine is available. Update required.]", Color.Red);
                Console.ResetColor();
                Console.ReadKey();
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("[Welcome! Bootstrapping in progress..]", Color.BlueViolet);
                Thread.Sleep(10000);
            }

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string celinePath = Path.Combine(appDataPath, "celine-v1");

            if (Directory.Exists(celinePath))
            {
                Console.WriteLine("[Welcome back]", Color.BlueViolet);
                Thread.Sleep(5000);
            }
            else
            {
                Console.WriteLine("[Initializing Celine...]", Color.BlueViolet);
                InitializeDirectories(celinePath);
                Console.WriteLine("[Initialization complete]", Color.BlueViolet);
                Thread.Sleep(2000);
            }

            string logSettingsFilePath = Path.Combine(celinePath, "logSettings", "FindDeletedFiles.txt");
            string logSettings = File.ReadAllText(logSettingsFilePath);

            if (logSettings == "false")
            {
                Console.Clear();
                Console.Title = "Tamper Check Failed";
                Console.WriteLine("[Tamper check failed. Action needed.]", Color.Red);
                Console.WriteLine("[Recommended: Ban or blacklist user.]", Color.Red);
                Console.ReadKey();
                Environment.Exit(0);
            }

            if (logSettings == "true")
            {
                await PerformFileCheckAsync();  // Await async method
            }
        }

        static async Task<string> CheckVersionAsync(string url)
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

        static void DisplayHeader()
        {
            string asciiArt = @"
    ╔════════════════════════════════════════╗
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@@@@@@@@@@@&##&@@@@@@@@@@║
    ║@@@@@@@@@@@@@@@@#BGGGBB&@@&BPGGPB@@@@@@@║
    ║@@@@@@@@@@@@@@BPPGGGGGGGGBP5PGPB@@@@@@@@║
    ║@@@@@@@@@@@@#PP5PGPGPGGGGGPPPPP@@@@@@@@@║
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

            Header header = new Header
            {
                Owner = "Opium",
                GitHub = "https://github.com/playboifusi/Celine",
                Version = "0.0.2"
            };

            Console.WriteLine(header.Generate(), headerColor);
            Console.WriteLine();
            PrintCentered(asciiArt, asciiColor);
        }

        static void PrintCentered(string text, Color color)
        {
            int consoleWidth = Console.WindowWidth;
            foreach (var line in text.Split('\n'))
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue; // Skip empty lines
                int leftPadding = (consoleWidth - trimmedLine.Length) / 2;
                Console.WriteLine(new string(' ', leftPadding) + trimmedLine, color);
            }
        }

        static void InitializeDirectories(string path)
        {
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "integrity"));
            Directory.CreateDirectory(Path.Combine(path, "logSettings"));

            string filePath = Path.Combine(path, "logSettings", "FindDeletedFiles.txt");
            File.WriteAllText(filePath, "true");
        }


        static async Task PerformFileCheckAsync()
        {
            Console.WriteLine("[Initiating file check]", Color.BlueViolet);
            Thread.Sleep(5000);

            CheckCelex();
            CheckWave(); // Updated method
            CheckHiddenCelex();
            CheckSolara();
            CheckBootstrapper();

            Console.WriteLine("[Download BAM Tools? (Y/N)]: ", Color.BlueViolet);
            string response = Console.ReadLine();
            if (response?.ToLower() == "y")
            {
                await DownloadBamTools();  // Await async method
            }
            else if (response?.ToLower() == "n")
            {
                Console.WriteLine("[Download skipped]", Color.BlueViolet);
            }
            else
            {
                Console.WriteLine("[Invalid response]", Color.Red);
            }
        }

        static async Task CheckFoldersAsync()
        {
            // Check for different folders
            await Task.Run(() => 
            {
                CheckCelex();
                CheckHiddenCelex();
                CheckSolara();
            });
        }

        static async Task CheckShortcutsAsync()
        {
            // Check for different shortcuts
            await Task.Run(() => 
            {
                CheckWave();  // Perform shortcut checks
            });
        }

static void CheckCelex()
{
    string celexPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "celex-v2");

    if (Directory.Exists(celexPath))
    {
        Console.ForegroundColor = Color.Red;
        Console.WriteLine("[Celex detected]");
    }
    else
    {
        Console.ForegroundColor = Color.Green;
        Console.WriteLine("[Celex not detected]");
    }
}

        static void CheckWave()
        {
            // // Path for WaveInstaller.exe
            // string waveInstallerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "WaveInstaller.exe");
            // if (File.Exists(waveInstallerPath))
            // {
            //     Console.WriteLine("[WaveInstaller.exe detected]", Color.Red);
            // }
            // else
            // {
            //     Console.WriteLine("[WaveInstaller.exe not detected]", Color.LimeGreen);
            // }

            // Path for the wave folder in AppData\Local
            // string waveFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wave");
            // if (Directory.Exists(waveFolderPath))
            // {
            //     Console.WriteLine("[Wave folder detected]", Color.Red);

            //     // Check for wavebootstrapper inside wave folder
            //     string waveBootstrapperPath = Path.Combine(waveFolderPath, "waveboostrapper.exe");
            //     if (File.Exists(waveBootstrapperPath))
            //     {
            //         Console.WriteLine("[Wave bootstrapper detected]", Color.Red);
            //     }
            //     else
            //     {
            //         Console.WriteLine("[Wave bootstrapper not detected]", Color.LimeGreen);
            //     }
            // }
            // else
            // {
            //     Console.WriteLine("[Wave folder not detected]", Color.LimeGreen);
            // }

            // Path for the Wave shortcut
            string waveShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "Wave.lnk");
            if (File.Exists(waveShortcutPath))
            {
                Console.WriteLine("[Wave shortcut detected]", Color.Red);
            }
            else
            {
                Console.WriteLine("[Wave shortcut not detected]", Color.LimeGreen);
            }
        }

        static void CheckBootstrapper()
        {
            // Path for common bootstrapper files
            string bootstrapperPath1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "bootstrapper.exe");
            string bootstrapperPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "bootstrapper.exe");

            if (File.Exists(bootstrapperPath1))
            {
                Console.WriteLine("[Bootstrapper detected in AppData]", Color.Red);
            }
            else if (File.Exists(bootstrapperPath2))
            {
                Console.WriteLine("[Bootstrapper detected in LocalAppData]", Color.Red);
            }
            else
            {
                Console.WriteLine("[Bootstrapper not detected]", Color.LimeGreen);
            }
        }


static void CheckHiddenCelex()
{
    string hiddenCelexPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Internet Explorer", "configs");

    if (Directory.Exists(hiddenCelexPath))
    {
        Console.ForegroundColor = Color.Red;
        Console.WriteLine("[Hidden Celex folder detected]");
    }
    else
    {
        Console.ForegroundColor = Color.LimeGreen;
        Console.WriteLine("[Hidden Celex folder not detected]");
    }

}

static void CheckSolara()
{
    string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string solaraFolder = "SolaraB2";

    string solaraPathDownloads = Path.Combine(downloadsPath, solaraFolder);
    string solaraPathDesktop = Path.Combine(desktopPath, solaraFolder);
    string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Solara");

    if (Directory.Exists(solaraPathDownloads))
    {
        Console.WriteLine("[SolaraB2 folder in Downloads detected]", Color.Red);
    }
    else
    {
        Console.WriteLine("[SolaraB2 folder in Downloads not detected]", Color.Green);
    }

    if (Directory.Exists(solaraPathDesktop))
    {
        Console.WriteLine("[SolaraB2 folder on Desktop detected]", Color.Red);
    }
    else
    {
        Console.WriteLine("[SolaraB2 folder on Desktop not detected]", Color.Green);
    }

    if (Directory.Exists(programDataPath))
    {
        Console.WriteLine("[Solara folder in ProgramData detected]", Color.Red);
    }
    else
    {
        Console.WriteLine("[Solara folder in ProgramData not detected]", Color.Green);
    }
}

        static async Task DownloadBamTools()
        {
            string url = "https://github.com/playboifusi/Celine/raw/main/dropper/files/bam-tool.exe";
            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "celine-v1", "bam-tool.exe");

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] fileBytes = await client.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(destinationPath, fileBytes);
                    Console.WriteLine("[File downloaded to: " + destinationPath + "]", Color.BlueViolet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error: {ex.Message}]", Color.Red);
                }
            }
        }
    }

    class Header
    {
        public string Owner { get; set; } = "Unknown";
        public string GitHub { get; set; } = "Unknown";
        public string Version { get; set; } = "Unknown";

        public string Generate()
        {
            int width = Console.WindowWidth - 2;
            string ownerPart = $"• Owner: {Owner}".PadLeft(width / 2 + $"• Owner: {Owner}".Length / 2);
            string contributorsPart = "• Contributors: zynrax.".PadLeft(width / 2 + "• Contributors: zynrax".Length / 2);
            string githubPart = $"• GitHub: {GitHub}".PadLeft(width / 2 + $"• GitHub: {GitHub}".Length / 2);
            string versionPart = $"• Version: {Version}".PadLeft(width / 2 + $"• Version: {Version}".Length / 2);

            return $@"
    {ownerPart}
    {contributorsPart}
    {githubPart}
    {versionPart}
    ";
        }
    }
