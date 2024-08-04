using System;
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
        DisplayHeader();

        string versionUrl = "https://raw.githubusercontent.com/playboifusi/Celine/main/bin2/version2.txt";
        string currentVersion = "0.0.2";

        string latestVersion = await CheckVersionAsync(versionUrl);
        if (latestVersion == null)
        {
            Console.WriteLine("[Error: Unable to check for updates. Exiting...]", Color.Red);
            return;
        }

        Console.WriteLine($"Current version: {currentVersion}");
        Console.WriteLine($"Latest version: {latestVersion}");

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
        }

        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string celinePath = Path.Combine(appDataPath, "celine-v1");

        if (Directory.Exists(celinePath))
        {
            Console.WriteLine("[Welcome back]", Color.BlueViolet);
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
            Console.Clear();
            DisplayHeader();
            PerformFileCheck(currentVersion);
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
        Header header = new Header
        {
            Owner = "Opium",
            GitHub = "https://github.com/playboifusi/Celine",
            Version = "0.0.2"
        };
        Console.WriteLine(header.Generate(), Color.BlueViolet);
    }

    static void InitializeDirectories(string path)
    {
        Directory.CreateDirectory(path);
        Directory.CreateDirectory(Path.Combine(path, "integrity"));
        Directory.CreateDirectory(Path.Combine(path, "logSettings"));

        string filePath = Path.Combine(path, "logSettings", "FindDeletedFiles.txt");
        File.WriteAllText(filePath, "true");
    }

    static void PerformFileCheck(string version)
    {
        Console.WriteLine("[Initiating file check]", Color.BlueViolet);

        CheckCelex();
        CheckWave();
        CheckHiddenCelex();
        CheckSolara();

        Console.WriteLine("[Download BAM Tools? (Y/N)]: ", Color.BlueViolet);
        string response = Console.ReadLine();
        if (response?.ToLower() == "y")
        {
            DownloadBamTools().Wait();  // Use Wait to block until the async method completes
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

    static void CheckCelex()
    {
        string celexPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "celex-v2");
        Console.WriteLine(Directory.Exists(celexPath) ? "[Celex detected]" : "[Celex not detected]", Color.Red);
    }

    static void CheckWave()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string wavePath = Path.Combine(localAppData, "Wave");
        string waveFilePath = Path.Combine(wavePath, "WaveBootstrapper.exe");

        if (Directory.Exists(wavePath))
        {
            Console.WriteLine("[Wave folder detected]", Color.Red);
            Console.WriteLine(File.Exists(waveFilePath) ? "[WaveBootstrapper.exe detected]" : "[WaveBootstrapper.exe not detected]", Color.Red);
        }
        else
        {
            Console.WriteLine("[Wave folder not detected]", Color.LimeGreen);
        }
    }

    static void CheckHiddenCelex()
    {
        string hiddenCelexPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Internet Explorer", "configs");
        if (Directory.Exists(hiddenCelexPath))
        {
            Console.WriteLine("[Hidden Celex folder detected]", Color.Red);
        }
        else
        {
            Console.WriteLine("[Hidden Celex folder not detected]", Color.LimeGreen);
        }
    }

    static void CheckSolara()
    {
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string solaraFolder = "SolaraB2";

        string solaraPathDownloads = Path.Combine(downloadsPath, solaraFolder);
        string solaraPathDesktop = Path.Combine(desktopPath, solaraFolder);
        Console.WriteLine(Directory.Exists(solaraPathDownloads) ? "[SolaraB2 folder in Downloads detected]" : "[SolaraB2 folder in Downloads not detected]", Color.Red);
        Console.WriteLine(Directory.Exists(solaraPathDesktop) ? "[SolaraB2 folder on Desktop detected]" : "[SolaraB2 folder on Desktop not detected]", Color.Red);

        string programDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Solara");
        Console.WriteLine(Directory.Exists(programDataPath) ? "[Solara folder in ProgramData detected]" : "[Solara folder in ProgramData not detected]", Color.Red);
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
        string ownerPart = $"• {Owner}".PadLeft(width / 2 + $"• {Owner}".Length / 2);
        string githubPart = GitHub.PadLeft(width / 2 + GitHub.Length / 2);
        string versionPart = Version.PadLeft(width / 2 + Version.Length / 2);

        return $@"
                                    ██████ ███████ ██      ██ ███    ██ ███████
                                    ██     ██      ██      ██ ████   ██ ██      
                                    ██     █████   ██      ██ ██ ██  ██ █████   
                                    ██     ██      ██      ██ ██  ██ ██ ██      
                                    ██████ ███████ ███████ ██ ██   ████ ███████

{ownerPart}
{githubPart}
{versionPart}
                                    ------------------------------------------";
    }
}
