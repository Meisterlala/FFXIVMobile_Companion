/*
Humans must learn to apply their intelligence correctly and evolve beyond their current state.
People must change. Otherwise, even if humanity expands into space, it will only create new
conflicts, and that would be a very sad thing. - Aeolia Schenberg, 2091 A.D.
　　　　 ,r‐､　　　　 　, -､
　 　 　 !　 ヽ　　 　 /　　}
　　　　 ヽ､ ,! -─‐- ､{　　ﾉ
　　　 　 ／｡　｡　　　 r`'､´
　　　　/ ,.-─- ､　　 ヽ､.ヽ　　　Haro
　　 　 !/　　　　ヽ､.＿, ﾆ|　　　　　Haro!
 　　　 {　　　 　  　 　 ,'
　　 　 ヽ　 　     　 ／,ｿ
　　　　　ヽ､.＿＿__r',／
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace FFXIVMobile_Companion
{
    internal class Program
    {
        public static bool AdvancedMode = false;
        public static GameLanguage SelectedLanguage = GameLanguage.None;
        public static string SelectedConnectionType = ConnectionType.None;
        public static string ADB_IPAddress = "1.2.3.4";
        public static string ADB_Pairing_IPAddress = "1.2.3.4";
        public static string SelectedTask = "None";
        public static Random RandomNumber = new Random();
        public static Status RemoteStatus = new Status();
        public static bool SkipPairing = false;
        public static bool DoInitialSetup = false;
        public static string LogFile = Path.Combine(Directory.GetCurrentDirectory(), "FFXIVMC_log.txt");
        public static string InfoDumpFile = Path.Combine(Directory.GetCurrentDirectory(), "FFXIVMC_infodump.txt");

        // Constants
        private const int STD_OUTPUT_HANDLE = -11;

        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        // P/Invoke signatures
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        private static void Main(string[] args)
        {
            var handle = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(handle, out uint mode))
            {
                Console.WriteLine("GetConsoleMode failed.");
                return;
            }

            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if (!SetConsoleMode(handle, mode))
            {
                Console.WriteLine("SetConsoleMode failed.");
                return;
            }

            if (File.Exists(LogFile)) { File.Delete(LogFile); }
            /*TODO:
            - Add LDPlayer support
            - Implement a check system for program updates/language updates using a remote JSON
            - Implement a config JSON to remember your IP and possibly the language you select?
            - See if I can do this stuff without root on a normal install on phone (renaming PAKs too)
            */

            //http://patorjk.com/software/taag/#p=display&f=ANSI%20Shadow&t=MEKABOT
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("███████╗███████╗██╗  ██╗██╗██╗   ██╗███╗   ███╗ ██████╗");
            Console.WriteLine("██╔════╝██╔════╝╚██╗██╔╝██║██║   ██║████╗ ████║██╔════╝");
            Console.WriteLine("█████╗  █████╗   ╚███╔╝ ██║██║   ██║██╔████╔██║██║     ");
            Console.WriteLine("██╔══╝  ██╔══╝   ██╔██╗ ██║╚██╗ ██╔╝██║╚██╔╝██║██║     ");
            Console.WriteLine("██║     ██║     ██╔╝ ██╗██║ ╚████╔╝ ██║ ╚═╝ ██║╚██████╗");
            Console.WriteLine("╚═╝     ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝  ╚═╝     ╚═╝ ╚═════╝");
            Console.ResetColor();
            WriteLine("Final Fantasy XIV Mobile Companion (Created by Aida Enna)");
            WriteLine("Source/Credits: " + Color.Blue + "https://github.com/Aida-Enna/FFXIVMobile_Companion");
            WriteLine("FFXIV Mobile EN Discord: " + Color.Blue + "https://discord.gg/ffxivmobile");
            var entryAssembly = Assembly.GetEntryAssembly();
            RemoteStatus = Functions.GetRemoteStatus();
            //https://umamusume.com/characters
            WriteLine(Color.Yellow + $"[Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " built on " + Properties.Resources.BuildDate.Replace(Environment.NewLine, "") + "]");

            /*
             █████  ██████   ██████  ███████
            ██   ██ ██   ██ ██       ██
            ███████ ██████  ██   ███ ███████
            ██   ██ ██   ██ ██    ██      ██
            ██   ██ ██   ██  ██████  ███████
            */

            if (args.Length > 0)
            {
                string TotalArgs = "";
                foreach (string argument in args)
                {
                    TotalArgs += argument + " ";
                    if (argument.ToLower() == "-adv")
                    {
                        AdvancedMode = true;
                        WriteLine(Color.Magenta + "[ARGS] Advanced mode enabled - Here be dragons");
                    }
                    if (argument.ToLower().Contains("-lang="))
                    {
                        string Language = argument.ToLower().Replace("-lang=", "");
                        switch (Language)
                        {
                            case "en":
                                SelectedLanguage = GameLanguage.English;
                                break;

                            case "ja":
                                SelectedLanguage = GameLanguage.Japanese;
                                break;

                            case "ko":
                                SelectedLanguage = GameLanguage.Korean;
                                break;

                            case "de":
                                SelectedLanguage = GameLanguage.German;
                                break;

                            case "fr":
                                SelectedLanguage = GameLanguage.French;
                                break;

                            case "zh":
                                SelectedLanguage = GameLanguage.Chinese;
                                break;

                            default:
                                WriteLine(Color.Red + "[ARGS] Invalid patch language specified: " + Language);
                                break;
                        }
                        if (SelectedLanguage != GameLanguage.None) { WriteLine(Color.Magenta + "[ARGS] Patch language set to " + SelectedLanguage.LongName); }
                    }
                    if (argument.ToLower() == "-usb")
                    {
                        SelectedConnectionType = ConnectionType.USB;
                        WriteLine(Color.Magenta + "[ARGS] Connection type set to USB");
                    }
                    if (argument.ToLower() == "-wifi")
                    {
                        SelectedConnectionType = ConnectionType.WiFi;
                        WriteLine(Color.Magenta + "[ARGS] Connection type set to WiFi");
                    }
                    if (argument.ToLower() == "-nopair")
                    {
                        SkipPairing = true;
                        WriteLine(Color.Magenta + "[ARGS] Skipping pairing process for WiFi");
                    }
                    if (argument.ToLower() == "-mumu")
                    {
                        SelectedConnectionType = ConnectionType.MuMu;
                        WriteLine(Color.Magenta + "[ARGS] Connection type set to MuMu");
                    }
                    if (argument.ToLower() == "-bluestacks")
                    {
                        SelectedConnectionType = ConnectionType.BlueStacks;
                        WriteLine(Color.Magenta + "[ARGS] Connection type set to BlueStacks");
                    }
                    if (argument.ToLower() == "-ldplayer")
                    {
                        SelectedConnectionType = ConnectionType.LDPlayer;
                        WriteLine(Color.Magenta + "[ARGS] Connection type set to LDPlayer");
                    }
                    if (argument.ToLower().Contains("-ip="))
                    {
                        ADB_IPAddress = argument.Replace("-ip=", "");
                        WriteLine(Color.Magenta + "[ARGS] ADB IP Address/Port set to " + ADB_IPAddress);
                    }
                    if (argument.ToLower().Contains("-initialsetup"))
                    {
                        DoInitialSetup = true;
                        WriteLine(Color.Magenta + "[ARGS] Running initial patch setup");
                    }
                }
                File.AppendAllText(LogFile, "Arguments: " + TotalArgs + Environment.NewLine);
            }
            WriteLog("OS: " + RuntimeInformation.OSDescription);
            WriteLog("Current Working Directory: " + Directory.GetCurrentDirectory());
            //#if !DEBUG

            /*
            ██    ██ ██████  ██████   █████  ████████ ███████      ██████ ██   ██ ███████  ██████ ██   ██
            ██    ██ ██   ██ ██   ██ ██   ██    ██    ██          ██      ██   ██ ██      ██      ██  ██
            ██    ██ ██████  ██   ██ ███████    ██    █████       ██      ███████ █████   ██      █████
            ██    ██ ██      ██   ██ ██   ██    ██    ██          ██      ██   ██ ██      ██      ██  ██
             ██████  ██      ██████  ██   ██    ██    ███████      ██████ ██   ██ ███████  ██████ ██   ██
            */
            string CurrentMD5 = Functions.CalculateMD5(entryAssembly.Location);

            if (File.Exists(entryAssembly.Location + ".bak"))
            {
                File.Delete(entryAssembly.Location + ".bak");
            }

            var hasMessage = !string.IsNullOrEmpty(RemoteStatus.Message);

            var previousColor = Console.ForegroundColor;

            if (RemoteStatus.MessageEnabled && hasMessage)
            {
                Console.ForegroundColor = RemoteStatus.MessageColor;
                Console.WriteLine("{0}", RemoteStatus.Message);
            }

            if (RemoteStatus.Disabled)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This program is currently disabled remotely by the creator (Aida Enna). Please wait until a fix is live, at which point the program will auto-update and begin working again. Sorry!");
                Console.WriteLine("Press the enter key to exit.");
                Console.ReadLine();
                Console.ForegroundColor = previousColor;
                Environment.Exit(0);
            }

            if (RemoteStatus.MessageEnabled)
            {
                for (int countdown = RemoteStatus.MessageTimeout; countdown > 0; countdown--)
                {
                    Console.Write("\rContinuing in {0} second(s)...   ", countdown);
                    Thread.Sleep(1000);
                }
                Console.WriteLine("\rContinuing in 0 seconds...   ");
            }

            Console.ForegroundColor = previousColor;

            if (RemoteStatus.UpdatingEnabled)
            {
                WriteLog("Checking for program updates...");
                if (CurrentMD5 != RemoteStatus.ProgramMD5)
                {
                    try
                    {
                        WriteLine("A new version is available, downloading now...");
                        int UpdateAttempts = 0;
                        if (File.Exists("update_attempts.txt"))
                        {
                            UpdateAttempts = Convert.ToInt32(File.ReadAllText("update_attempts.txt"));
                        }
                        UpdateAttempts++;
                        File.WriteAllText("update_attempts.txt", UpdateAttempts.ToString());
                        if (UpdateAttempts > 5)
                        {
                            WriteLine(Color.Red + "Updating has failed 5+ times in a row and will not be attempted again until the next time you open the program.\nPlease download the latest version from " + Color.Blue + RemoteStatus.ProgramUpdateURL);
                        }
                        else
                        {
                            File.Move(entryAssembly.Location, entryAssembly.Location + ".bak");
                            Functions.DownloadFileFallback(RemoteStatus.ProgramUpdateURL, "FFXIVMobile_Companion.exe");
                            CurrentMD5 = Functions.CalculateMD5(entryAssembly.Location);
                            if (CurrentMD5 != RemoteStatus.ProgramMD5)
                            {
                                WriteLine(Color.Red + "Updating failed! Please download the latest version from " + Color.Blue + RemoteStatus.ProgramUpdateURL);
                            }
                            else
                            {
                                Process.Start(entryAssembly.Location); // to start new instance of application
                                Environment.Exit(0);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine(Color.Red + "Updating failed! Please download the latest version from " + Color.Blue + RemoteStatus.ProgramUpdateURL);
                    }
                }
            }
            else
            {
                WriteLog(Color.Yellow + "Auto-updating is currently disabled remotely by the creator (Aida Enna). The program will attempt to update once it is re-enabled remotely.");
            }
            //#endif
            File.WriteAllText("update_attempts.txt", "0");
            /*
             █████  ██████  ██████       ██████ ██   ██ ███████  ██████ ██   ██
            ██   ██ ██   ██ ██   ██     ██      ██   ██ ██      ██      ██  ██
            ███████ ██   ██ ██████      ██      ███████ █████   ██      █████
            ██   ██ ██   ██ ██   ██     ██      ██   ██ ██      ██      ██  ██
            ██   ██ ██████  ██████       ██████ ██   ██ ███████  ██████ ██   ██
            */
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"adb\adb.exe")))
            {
                WriteLine(Color.Red + "ADB Folder/file does not exist! (Checked \"" + Directory.GetCurrentDirectory() + "\")\nAttempting to fix by downloading required files...");
                try
                {
                    Functions.DownloadFile("https://github.com/Aida-Enna/FFXIVMobile_Companion/blob/main/extras/adb.zip?raw=true", Path.Combine(Directory.GetCurrentDirectory(), "adb.zip"));
                }
                catch
                {
                    try
                    {
                        Functions.DownloadFileFallback("https://github.com/Aida-Enna/FFXIVMobile_Companion/blob/main/extras/adb.zip?raw=true", Path.Combine(Directory.GetCurrentDirectory(), "adb.zip"));
                    }
                    catch
                    {
                        WriteLine(Color.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                        Close(false);
                    }
                }
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip")))
                {
                    ZipFile.ExtractToDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"), Directory.GetCurrentDirectory());
                }
                else
                {
                    WriteLine(Color.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                    Close(false);
                }
                WriteLine(Color.Green + "ADB downloaded and installed successfully!");
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"));
            }
            Write(Environment.NewLine);
            /*
            ███████ ██ ██████  ███████ ████████     ███████ ███████ ██      ███████  ██████ ████████ ██  ██████  ███    ██
            ██      ██ ██   ██ ██         ██        ██      ██      ██      ██      ██         ██    ██ ██    ██ ████   ██
            █████   ██ ██████  ███████    ██        ███████ █████   ██      █████   ██         ██    ██ ██    ██ ██ ██  ██
            ██      ██ ██   ██      ██    ██             ██ ██      ██      ██      ██         ██    ██ ██    ██ ██  ██ ██
            ██      ██ ██   ██ ███████    ██        ███████ ███████ ███████ ███████  ██████    ██    ██  ██████  ██   ████
            */
            do
            {
                switch (SelectLanguage())
                {
                    case "1":
                        File.AppendAllText(LogFile, "-> User selected 1" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.English;
                        break;

                    case "2":
                        File.AppendAllText(LogFile, "-> User selected 2" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.Japanese;
                        break;

                    case "3":
                        File.AppendAllText(LogFile, "-> User selected 3" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.Korean;
                        break;

                    case "4":
                        File.AppendAllText(LogFile, "-> User selected 4" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.German;
                        break;

                    case "5":
                        File.AppendAllText(LogFile, "-> User selected 5" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.French;
                        break;

                    case "6":
                        File.AppendAllText(LogFile, "-> User selected 6" + Environment.NewLine);
                        SelectedLanguage = GameLanguage.Chinese;
                        break;

                    default:
                        if (SelectedLanguage.LongName == GameLanguage.None.LongName) { WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine); }
                        break;
                }
            } while (SelectedLanguage.LongName == GameLanguage.None.LongName);
            WriteLine(Environment.NewLine);
            /*
             ██████  ██████  ███    ██ ███    ██ ███████  ██████ ████████ ██  ██████  ███    ██     ████████ ██    ██ ██████  ███████
            ██      ██    ██ ████   ██ ████   ██ ██      ██         ██    ██ ██    ██ ████   ██        ██     ██  ██  ██   ██ ██
            ██      ██    ██ ██ ██  ██ ██ ██  ██ █████   ██         ██    ██ ██    ██ ██ ██  ██        ██      ████   ██████  █████
            ██      ██    ██ ██  ██ ██ ██  ██ ██ ██      ██         ██    ██ ██    ██ ██  ██ ██        ██       ██    ██      ██
             ██████  ██████  ██   ████ ██   ████ ███████  ██████    ██    ██  ██████  ██   ████        ██       ██    ██      ███████
            */
            do
            {
                switch (SelectConnectionType())
                {
                    case "1":
                        SelectedConnectionType = ConnectionType.USB;
                        File.AppendAllText(LogFile, "-> User selected 1" + Environment.NewLine);
                        break;

                    case "2":
                        SelectedConnectionType = ConnectionType.WiFi;
                        File.AppendAllText(LogFile, "-> User selected 2" + Environment.NewLine);
                        break;

                    case "3":
                        SelectedConnectionType = ConnectionType.MuMu;
                        File.AppendAllText(LogFile, "-> User selected 3" + Environment.NewLine);
                        break;

                    case "4":
                        SelectedConnectionType = ConnectionType.BlueStacks;
                        File.AppendAllText(LogFile, "-> User selected 4" + Environment.NewLine);
                        break;

                    case "5":
                        SelectedConnectionType = ConnectionType.LDPlayer;
                        File.AppendAllText(LogFile, "-> User selected 5" + Environment.NewLine);
                        break;

                    default:
                        if (SelectedConnectionType == ConnectionType.None) { WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine); }
                        break;
                }
            } while (SelectedConnectionType == ConnectionType.None);
            WriteLine(Environment.NewLine);
            /*
             ██████  ██████  ███    ██ ███    ██ ███████  ██████ ████████ ██  ██████  ███    ██
            ██      ██    ██ ████   ██ ████   ██ ██      ██         ██    ██ ██    ██ ████   ██
            ██      ██    ██ ██ ██  ██ ██ ██  ██ █████   ██         ██    ██ ██    ██ ██ ██  ██
            ██      ██    ██ ██  ██ ██ ██  ██ ██ ██      ██         ██    ██ ██    ██ ██  ██ ██
             ██████  ██████  ██   ████ ██   ████ ███████  ██████    ██    ██  ██████  ██   ████
            */
            if (SelectedConnectionType == ConnectionType.USB)
            {
                WriteLine("1. Go to Settings -> About Phone -> Software Information");
                WriteLine("2. Tap 'Build number' 7 times until you see 'Developer mode enabled'");
                WriteLine("3. Return to Settings, go into Developer Options");
                WriteLine("4. Enable USB Debugging");
                WriteLine("5. Plug in your phone and hit 'OK' if an 'Allow USB debugging?' menu pops up");
                WriteLine("5. Press enter once plugged in and ready");
                WriteLine("(If you have a Xiaomi device, you may need to enable 'USB debugging(Security Settings)' and 'Install via USB')");
                Console.ReadLine();
            }
            else if (SelectedConnectionType == ConnectionType.WiFi)
            {
                if (SkipPairing)
                {
                    WriteLine("Skipping pairing");
                }
                else
                {
                    WriteLine("1. Go to Settings -> About Phone -> Software Information");
                    WriteLine("2. Tap 'Build number' 7 times until you see 'Developer mode enabled'");
                    WriteLine("3. Return to Settings, go into Developer Options");
                    WriteLine("4. Go into the 'Wireless debugging' menu and enable Wireless debugging");
                    WriteLine("5. Select 'Pair device with pairing code'");
                    WriteLine("6. Enter the information that shows up below");
                    do
                    {
                        Write("\nEnter the IP address & Port (hit enter if you are already paired): ");
                        ADB_Pairing_IPAddress = Console.ReadLine();
                    } while (Functions.ValidateIPAndPort(ADB_Pairing_IPAddress) == false);
                    if (!string.IsNullOrWhiteSpace(ADB_Pairing_IPAddress))
                    {
                        string PairingCode = "";
                        do
                        {
                            Write("Enter the Wi-Fi pairing code: ");
                            PairingCode = Console.ReadLine();
                        } while (string.IsNullOrWhiteSpace(PairingCode));
                        WriteLine("Pairing...");
                        WriteLine(ADB("pair " + ADB_Pairing_IPAddress + " " + PairingCode));
                    }
                }
            }
            if (SelectedConnectionType == ConnectionType.WiFi || SelectedConnectionType == ConnectionType.MuMu || SelectedConnectionType == ConnectionType.BlueStacks || SelectedConnectionType == ConnectionType.LDPlayer)
            {
                ADB("disconnect");
                int ConnectionAttempts = 0;
                bool ADBConnected = false;
                do
                {
                    ConnectionAttempts++;
                    if (ADB_IPAddress == "1.2.3.4")
                    {
                        if (SelectedConnectionType == ConnectionType.WiFi)
                        {
                            Write("\nEnter the IP address & Port (not the pairing one, the one in the wireless debugging settings): ");
                            ADB_IPAddress = Console.ReadLine();
                            if (Functions.ValidateIPAndPort(ADB_IPAddress) == false)
                            {
                                continue;
                            }
                        }
                        if (SelectedConnectionType == ConnectionType.MuMu)
                        {
                            if (ConnectionAttempts > 4)
                            {
                                WriteLine(Color.Red + "Failed to connect to MuMu! Please restart the emulator and try again.");
                                Close(false);
                            }
                            ADB_IPAddress = "127.0.0.1:7555";
                        }
                        if (SelectedConnectionType == ConnectionType.BlueStacks)
                        {
                            if (ConnectionAttempts > 4)
                            {
                                WriteLine(Color.Red + "Failed to connect to BlueStacks! Please restart the emulator and try again.");
                                Close(false);
                            }
                            ADB_IPAddress = "127.0.0.1:5555";
                        }
                        if (SelectedConnectionType == ConnectionType.LDPlayer)
                        {
                            if (ConnectionAttempts > 4)
                            {
                                WriteLine(Color.Red + "Failed to connect to LDPlayer! Please restart the emulator and try again.");
                                Close(false);
                            }
                            ADB_IPAddress = "127.0.0.1:5554";
                        }
                    }
                    else
                    {
                        WriteLine("Using " + ADB_IPAddress);
                    }
                    string ConnectionResult = ADB("connect " + ADB_IPAddress, true);
                    WriteLine(ConnectionResult);
                    if (ConnectionResult.Contains("ERROR - ") || ConnectionResult.ToLower().Contains("failed"))
                    {
                        WriteLine(Color.Red + "Connection failed, please check your IP and port and try again! (Attempt " + ConnectionAttempts + "/5)");
                        if (ConnectionAttempts > 4)
                        {
                            WriteLine(Color.Red + "Failed to connect to your device! Please restart your device and try again.");
                            Close(false);
                        }
                        continue;
                    }
                    ADBConnected = true;
                }
                while (ADBConnected == false);
            }

            /*
            ███████ ███████ ██      ███████  ██████ ████████     ████████  █████  ███████ ██   ██
            ██      ██      ██      ██      ██         ██           ██    ██   ██ ██      ██  ██
            ███████ █████   ██      █████   ██         ██           ██    ███████ ███████ █████
                 ██ ██      ██      ██      ██         ██           ██    ██   ██      ██ ██  ██
            ███████ ███████ ███████ ███████  ██████    ██           ██    ██   ██ ███████ ██   ██
            */
            Write(Environment.NewLine);
            if (DoInitialSetup)
            {
                InitialSetup();
            }
            else
            {
                do
                {
                    switch (SelectTask())
                    {
                        case "0":
                            //Download/Fix the story patch
                            File.AppendAllText(LogFile, "-> User selected 0" + Environment.NewLine);
                            InitialSetup();
                            break;

                        case "1":
                            //Change the language to the specified
                            File.AppendAllText(LogFile, "-> User selected 1" + Environment.NewLine);
                            ChangeLanguage();
                            break;

                        case "2":
                            File.AppendAllText(LogFile, "-> User selected 2" + Environment.NewLine);
                            FixPermissions();
                            Close();
                            break;

                        case "L":
                        case "l":
                            File.AppendAllText(LogFile, "-> User selected L" + Environment.NewLine);
                            LogInfo();
                            break;

                        case "A":
                        case "a":
                            //Install the game
                            InstallGame();
                            break;

                        default:
                            WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine);
                            break;
                    }
                } while (SelectedTask == "None");
            }
        }

        public static string SelectLanguage()
        {
            if (SelectedLanguage != GameLanguage.None)
            {
                WriteLine(SelectedLanguage.LongName + " selected");
                return SelectedLanguage.ShortName;
            }
            WriteLine(Color.Green + "Please select what language you would like your game to be:");
            WriteLine("1. English (en)");
            WriteLine("2. Japanese (jp)");
            WriteLine("3. Korean (ko)");
            WriteLine("4. German (de)");
            WriteLine("5. French (fr)");
            WriteLine("6. Chinese (zh)");

            Console.Write("\nType your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string SelectConnectionType()
        {
            if (SelectedConnectionType != ConnectionType.None)
            {
                WriteLine(SelectedConnectionType + " connection selected");
                return SelectedConnectionType;
            }
            WriteLine(Color.Green + "What device are you using?");
            WriteLine("1. An actual phone (" + Color.Blue + "over USB" + Color.Default + ")");
            WriteLine("2. An actual phone (" + Color.Blue + "over WiFi" + Color.Default + ")");
            WriteLine("3. MuMu");
            WriteLine("4. BlueStacks");
            WriteLine("5. LDPlayer");

            Console.Write("\nType your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string SelectTask()
        {
            WriteLine(Color.Green + "What do you want to do?");
            WriteLine("1. Change language to " + SelectedLanguage.LongName + " and/or install/update the story patch");
            WriteLine("2. Fix game permissions (Fix error 70254604/Updater reverting language/Updater locking up)");
            if (AdvancedMode)
            {
                //WriteLine(Color.Red + "A. [ADV] Non root UI swap");
            }
            WriteLine("\n" + Color.Yellow + "If you've just installed the game on a non-rooted device" + Color.Default + ", you need to go through a one-time setup to change languages.");
            WriteLine("If that's the case, please select one of the options below:");
            WriteLine("0. Initial setup for non-rooted devices");

            WriteLine("\n" + Color.Yellow + "If you haven't installed the game yet" + Color.Default + ", you can select the following option to download/install it for you:");
            WriteLine("A. Download/Install FFXIV Mobile");

            WriteLine("\n" + Color.Yellow + "Troubleshooting:");
            WriteLine("L. Generate a file full of information so people in the discord can try to help you");

            Console.Write("\nType your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static void ChangeLanguage()
        {
            WriteLine(Environment.NewLine);

            if (IsGameOpen())
            {
                WriteLine("Closing game");
                WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            }

            WriteLine("Deleting exiting config INI file (if it exists)");
            WriteLine(ADB("shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini", SilenceOutput: true));

            string StringToWrite = @"[Internationalization]\\nCulture=" + SelectedLanguage.ShortName;

            WriteLine("Changing language to " + SelectedLanguage.LongName);
            string ADBResult = ADB("shell echo " + StringToWrite + " >> /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to create the INI file, make sure your phone is connected and you followed all the steps!");
                Close(false);
            }

            CheckForStoryPatchUpdates();

            ADBResult = ADB("shell mkdir /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to apply the story patch (making the directory) - Is your game set up correctly? Please do the initial set up if not.");
                Close(false);
            }

            ADBResult = ADB("push FDataBaseLoc.db /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to apply the story patch (pushing the DB) - Is your game set up correctly? Please do the initial set up if not.");
                Close(false);
            }

            WriteLine("Checking/Renaming " + RemoteStatus.BadFiles.Count + " bad file(s) to fix language issues:");
            foreach (string BadPakFile in RemoteStatus.BadFiles)
            {
                WriteLine("Checking/Renaming " + BadPakFile.Split('/').Last());
                //adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
                ADBResult = ADB("shell mv " + BadPakFile + " " + BadPakFile + ".bak_" + Random());
                if (ADBResult.Contains("ERROR - ") & ADBResult.Contains("No such file or directory") == false)
                {
                    WriteLine(Color.Red + "Failed to rename PAK files - Is your game set up correctly? Please do the initial set up if not.");
                    Close(false);
                }
                else
                {
                    if (ADBResult.Contains("No such file or directory") == false) { WriteLine(ADBResult); }
                }
            }

            //FixPermissions();

            ADBResult = ADB("shell chmod -w /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/FDataBaseLoc.db");

            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to apply the story patch (chmod read only) - Is your game set up correctly? Please do the initial set up if not.");
                Close(false);
            }

            WriteLine("Opening game, enjoy!");
            ADB("shell monkey -p com.tencent.tmgp.fmgame 1 >/dev/null 2>&1");
            Close();
        }

        public static void InitialSetup()
        {
            /*
            ███████ ██ ██████  ███████ ████████       ████████ ██ ███    ███ ███████     ███████ ███████ ████████ ██    ██ ██████
            ██      ██ ██   ██ ██         ██             ██    ██ ████  ████ ██          ██      ██         ██    ██    ██ ██   ██
            █████   ██ ██████  ███████    ██    █████    ██    ██ ██ ████ ██ █████       ███████ █████      ██    ██    ██ ██████
            ██      ██ ██   ██      ██    ██             ██    ██ ██  ██  ██ ██               ██ ██         ██    ██    ██ ██
            ██      ██ ██   ██ ███████    ██             ██    ██ ██      ██ ███████     ███████ ███████    ██     ██████  ██
            */
            WriteLine(Environment.NewLine);
            WriteLine("In order to modify the game's files on a non-rooted device, we have to " + Color.Yellow + "delete the update and config data.");
            WriteLine("Continuing will delete the update data (and your settings), forcing you to redownload the game.");
            WriteLine("Clearing this update data " + Color.Inverse + "will not delete your characters or progress" + Color.Default + ", as those are saved on the server.");
            WriteLine("If you'd like to start the initial setup (and wipe the update data), please type 'Yes' below.");
            string Response = Console.ReadLine();
            if (Response.ToLower() != "yes")
            {
                WriteLine("You have selected not to continue. If you want to later, simply open the program again.");
                Close();
            }
            WriteLine("Continuing with setup");
            if (IsGameOpen())
            {
                WriteLine("Closing game");
                WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            }

            WriteLine("Clearing local game data");
            string ADBResult = ADB("shell pm clear com.tencent.tmgp.fmgame");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to clear local game data. Please go to your settings and make sure that 'Disable Permission Monitoring' is enabled and try again.");
                WriteLine(Color.Red + "If you have a MIUI phone, it may be called 'USB debugging (Security Settings)' instead.");
                Close(false);
            }

            WriteLine("Creating folder for language change");
            ADBResult = ADB("shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to create the /Config/Android/ folder, make sure your phone is connected and you followed all the steps!");
                Close(false);
            }

            WriteLine("Creating folder for language fix");
            ADBResult = ADB("shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to create the /1.0.2.0/Dolphin/Paks/ folder, make sure your phone is connected and you followed all the steps!");
                Close(false);
            }

            WriteLine("Creating folder for story patch");
            ADBResult = ADB("shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to create the files/Database/ folder, make sure your phone is connected and you followed all the steps!");
                Close(false);
            }

            WriteLine("Deleting exiting config INI file (if it exists)");
            WriteLine(ADB("shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini", SilenceOutput: true));

            string StringToWrite = @"[Internationalization]\\nCulture=" + SelectedLanguage.ShortName;
            WriteLine("Changing language to " + Color.Blue + SelectedLanguage.LongName);
            ADBResult = ADB("shell echo " + StringToWrite + " >> /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to create the INI file, make sure your phone is connected and you followed all the steps!");
                Close(false);
            }

            CheckForStoryPatchUpdates();

            WriteLine("Applying story patch");
            ADBResult = ADB("push FDataBaseLoc.db /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to apply the story patch (pushing the DB) - Is your game set up correctly? Please do the initial set up if not.");
                Close(false);
            }

            WriteLine("Story patch applied!");

            //FixPermissions();

            ADBResult = ADB("shell chmod -w /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/FDataBaseLoc.db");
            WriteLine(ADBResult);
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to apply the story patch (chmod read only) - Is your game set up correctly? Please do the initial set up if not.");
                Close(false);
            }

            Write(Environment.NewLine);

            WriteLine("The game will now open - Let it fully update and patch. Press enter once it is complete and you're at the login screen.");
            WriteLine("The updater will be partially translated, but you " + Color.Yellow + "need to continue these steps" + Color.Default + " for your game to be in that language.");
            WriteLine("If your updater is not in " + SelectedLanguage.LongName + ", please close the app and re-open it and let fully update and patch.");
            if (SelectedConnectionType == ConnectionType.USB) { WriteLine(Color.Yellow + "Do not unplug your device from your computer or lock your device."); }
            if (SelectedConnectionType == ConnectionType.WiFi) { WriteLine(Color.Yellow + "Do not disconnect your phone from your WiFi or lock your device."); }

            ADB("shell monkey -p com.tencent.tmgp.fmgame 1 >/dev/null 2>&1");

            Console.ReadLine();
            WriteLine("Press enter one more time just to be super sure that the game is fully updated and at the login screen");
            Console.ReadLine();

            WriteLine("Closing game and fixing language issues");
            WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            //Let's wait 10 seconds just to be sure it's really closed
            Thread.Sleep(10000);

            WriteLine("Checking/Renaming " + RemoteStatus.BadFiles.Count + " bad file(s) to fix language issues:");
            foreach (string BadPakFile in RemoteStatus.BadFiles)
            {
                WriteLine("Checking/Renaming " + BadPakFile.Split('/').Last());
                //adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
                ADBResult = ADB("shell mv " + BadPakFile + " " + BadPakFile + ".bak_" + Random());
                if (ADBResult.Contains("ERROR - ") & ADBResult.Contains("No such file or directory") == false)
                {
                    WriteLine(Color.Red + "Failed to rename PAK files - Is your game set up correctly? Please do the initial set up if not.");
                    Close(false);
                }
                else
                {
                    if (ADBResult.Contains("No such file or directory") == false) { WriteLine(ADBResult); }
                }
            }

            WriteLine("All done! Your game should now be fully patched into " + Color.Blue + SelectedLanguage.LongName + ".");
            ADB("shell monkey -p com.tencent.tmgp.fmgame 1 >/dev/null 2>&1");
            WriteLine("If you need to update the story patch later, re-open this app, select the language you want, select how you want to connect, then select 'Download/Update the story patch'.");
            Close();
        }

        public static void FixPermissions()
        {
            if (IsGameOpen())
            {
                WriteLine("Closing game");
                WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            }
            WriteLine(Environment.NewLine + "Fixing permissions");
            string ADBResult = ADB("shell chmod -R 777 /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/");
            if (ADBResult.Contains("ERROR - "))
            {
                WriteLine(Color.Red + "Failed to change permissions on the folder - Please try this initial setup again!");
                Close(false);
            }
            else
            {
                WriteLine(ADBResult);
            }
        }

        public static void InstallGame()
        {
            Write(Environment.NewLine);
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ffxiv_m.apk")))
            {
                WriteLine("Downloading FFXIV Mobile installer APK (" + RemoteStatus.FFXIVM_APKURL + "), please wait...");
                try
                {
                    Functions.DownloadFile(RemoteStatus.FFXIVM_APKURL, Path.Combine(Directory.GetCurrentDirectory(), "ffxiv_m.apk"));
                }
                catch
                {
                    try
                    {
                        Functions.DownloadFileFallback(RemoteStatus.FFXIVM_APKURL, Path.Combine(Directory.GetCurrentDirectory(), "ffxiv_m.apk"));
                    }
                    catch
                    {
                        WriteLine(Color.Red + "Failed to download installer APK! Please download it from " + RemoteStatus.FFXIVM_APKURL + " and put it with this program.");
                        Close(false);
                    }
                }
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ffxiv_m.apk")))
                {
                    WriteLine(Color.Red + "Failed to download installer APK! Please download it from " + RemoteStatus.FFXIVM_APKURL + " and put it with this program.");
                    Close(false);
                }
                WriteLine(Color.Green + "Installer APK downloaded successfully!");
            }
            WriteLine("Installing FFXIV Mobile...");
            WriteLine("A prompt may open on your phone - Please click " + Color.Green + "Install");
            WriteLine(ADB("install ffxiv_m.apk"));
            WriteLine("Install completed!");
            Close();
        }

        public static void LogInfo()
        {
            Write(Environment.NewLine);
            WriteLine("Generating log, please wait...");
            File.WriteAllText(InfoDumpFile, "---Log Dump created on " + DateTime.UtcNow.ToString("G") + " (UTC)---" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "Selected Language: " + SelectedLanguage.LongName + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "Connection Type: " + SelectedConnectionType + Environment.NewLine);
            if (SelectedConnectionType == ConnectionType.WiFi) { File.AppendAllText(InfoDumpFile, "ADB IP Address: " + ADB_IPAddress + Environment.NewLine); }
            List<string> ValuesToCheck = new List<string>
            {
                "Model|model",
                "Manufacturer|ro.product.manufacturer",
                "Market name|ro.product.cpu.marketname",
                "Brand|ro.product.odm.brand",
                "CPU|ro.product.cpu.abi",
                "Language|language",
                "Timezone|persist.sys.timezone",
                "Locale|persist.sys.locale",
                "Hardware|ro.boot.hardware",
                "Android version|ro.build.version.release",
                "Android codename|ro.build.version.codename"
            };
            foreach (string DescriptionAndValue in ValuesToCheck)
            {
                string Description = DescriptionAndValue.Split('|')[0];
                string Value = DescriptionAndValue.Split('|')[1];
                string ADBValue = ADB("shell getprop | grep " + Value, NoColors: true);
                if (!string.IsNullOrWhiteSpace(ADBValue))
                {
                    File.AppendAllText(InfoDumpFile, Environment.NewLine + "-" + Description + "-" + Environment.NewLine + ADBValue + Environment.NewLine);
                }
            }

            if (ADB("shell getprop | grep .miui.", NoColors: true).Length > 0)
            {
                File.AppendAllText(InfoDumpFile, Environment.NewLine + "!WARNING: Miui phone detected!" + Environment.NewLine);
            }
            File.AppendAllText(InfoDumpFile, Environment.NewLine + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "    ██ ███████ ██ ██      ███████ ███████     ████████ ██████  ███████ ███████" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "   ██  ██      ██ ██      ██      ██             ██    ██   ██ ██      ██     " + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "  ██   █████   ██ ██      █████   ███████        ██    ██████  █████   █████  " + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, " ██    ██      ██ ██      ██           ██        ██    ██   ██ ██      ██     " + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "██     ██      ██ ███████ ███████ ███████        ██    ██   ██ ███████ ███████" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, ADB("shell find \"/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/ -print | sort | sed 's;[^/]*/;|---;g;s;---|; |;g'\"", NoColors: true) + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "    ██ ███████ ██ ██      ███████ ███████     ██████  ███████ ██████  ███    ███ ███████" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "   ██  ██      ██ ██      ██      ██          ██   ██ ██      ██   ██ ████  ████ ██     " + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "  ██   █████   ██ ██      █████   ███████     ██████  █████   ██████  ██ ████ ██ ███████" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, " ██    ██      ██ ██      ██           ██     ██      ██      ██   ██ ██  ██  ██      ██" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, "██     ██      ██ ███████ ███████ ███████     ██      ███████ ██   ██ ██      ██ ███████" + Environment.NewLine);
            File.AppendAllText(InfoDumpFile, ADB("shell ls -l -R /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/", NoColors: true) + Environment.NewLine);
            WriteLine("Log dump complete, please upload the " + Color.Yellow + Functions.TerminalURL(InfoDumpFile, "file://" + Path.Combine(Directory.GetCurrentDirectory(), InfoDumpFile)) + Color.Default + " file to the discord's help channel.");
            WriteLine("Press enter to return to the task menu.");
            Console.ReadLine();
            Write(Environment.NewLine);
        }

        public static void Close(bool Success = true)
        {
            //Terminate ADB stuff
            ADB("disconnect");
            ADB("kill-server");
            if (Success)
            {
                WriteLine(Color.Green + "Task completed - Closing program in 20 seconds...");
                Thread.Sleep(20000);
                Environment.Exit(0);
            }
            else
            {
                WriteLine(Color.Red + "The program will now exit. Please try again after fixing the above issue.");
                WriteLine(Color.Yellow + "A log of what happened was saved to " + Path.Combine(Directory.GetCurrentDirectory(), LogFile));
                WriteLine(Color.Red + "Closing program in 20 seconds...");
                Thread.Sleep(20000);
                Environment.Exit(1);
            }
        }

        public static string ADB(string Command, bool RawCommand = false, bool SilenceOutput = false, bool NoColors = false)
        {
            try
            {
                if (!RawCommand)
                {
                    switch (SelectedConnectionType)
                    {
                        case ConnectionType.USB:
                            Command = "-d " + Command;
                            break;

                        case ConnectionType.WiFi:
                        case ConnectionType.MuMu:
                        case ConnectionType.BlueStacks:
                        case ConnectionType.LDPlayer:
                            Command = "-s " + ADB_IPAddress + " " + Command;
                            break;
                    }
                }

                if (SilenceOutput)
                {
                    Command += " >/dev/null 2>&1";
                }

                var ADBProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(Directory.GetCurrentDirectory(), @"adb\adb.exe"),
                        Arguments = Command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                ADBProcess.Start();
                string SuccessString = ADBProcess.StandardOutput.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim();
                string ErrorString = ADBProcess.StandardError.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim();
                ADBProcess.WaitForExit();
                if (SuccessString.Contains("cannot connect") || SuccessString.Contains("cannot resolve"))
                {
                    //ADB why? who hurt you?
                    if (NoColors)
                    {
                        return "ERROR - " + SuccessString;
                    }
                    else
                    {
                        return Color.Red + "ERROR - " + SuccessString;
                    }
                }
                if (string.IsNullOrWhiteSpace(SuccessString))
                {
                    if (ErrorString.Contains("1 file pushed, 0 skipped"))
                    {
                        //ADB why? who hurt you?
                        if (NoColors)
                        {
                            return SuccessString;
                        }
                        else
                        {
                            return Color.Cyan + SuccessString;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(ErrorString))
                    {
                        return "";
                    }
                    else
                    {
                        if (NoColors)
                        {
                            return "ERROR - " + ErrorString;
                        }
                        else
                        {
                            return Color.Red + "ERROR - " + ErrorString;
                        }
                    }
                }
                else
                {
                    if (NoColors)
                    {
                        return SuccessString;
                    }
                    else
                    {
                        return Color.Cyan + SuccessString;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Something went wrong with ADB - " + ex.ToString();
            }
        }

        public static bool IsGameOpen()
        {
            string IsGameOpenString = ADB("shell pidof com.tencent.tmgp.fmgame");
            if (!string.IsNullOrWhiteSpace(IsGameOpenString))
            {
                return true;
            }
            return false;
        }

        public static void CheckForStoryPatchUpdates()
        {
            //Check for new story patch updates
            string LocalDBHash = "";
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db")))
            {
                LocalDBHash = Functions.CalculateMD5(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db"));
            }

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db")) || LocalDBHash != RemoteStatus.TranslationMD5)
            {
                WriteLine("Downloading the latest " + SelectedLanguage.LongName + " translations, please wait...");
                try
                {
                    Functions.DownloadFile(RemoteStatus.TranslationUpdateURL, Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db"));
                }
                catch
                {
                    try
                    {
                        Functions.DownloadFileFallback(RemoteStatus.TranslationUpdateURL, Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db"));
                    }
                    catch
                    {
                        WriteLine(Color.Red + "Failed to download Localization DB! Please download it from " + RemoteStatus.TranslationUpdateURL + " and put it with this program.");
                        Close(false);
                    }
                }
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db")))
                {
                    Functions.DownloadFileFallback(RemoteStatus.TranslationUpdateURL, Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db"));
                    if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db")))
                    {
                        WriteLine(Color.Red + "Failed to download Localization DB! Please download it from " + RemoteStatus.TranslationUpdateURL + " and put it with this program.");
                        Close(false);
                    }
                }
                WriteLine(Color.Green + "Translations downloaded successfully!");
            }
        }

        public static void WriteLine(string Text)
        {
            if (Text == "") { return; }
            File.AppendAllText(LogFile, Regex.Replace(Text, @"\[.*?m", "") + Environment.NewLine);
            Console.WriteLine(Text + Color.Default);
        }

        public static void WriteLog(string Text)
        {
            File.AppendAllText(LogFile, Regex.Replace(Text, @"\[.*?m", "") + Environment.NewLine);
        }

        public static void Write(string Text)
        {
            if (Text == "") { return; }
            File.AppendAllText(LogFile, Regex.Replace(Text, @"\[.*?m", ""));
            Console.Write(Text + Color.Default);
        }

        public static string Random()
        {
            return RandomNumber.Next(999999999).ToString();
        }
    }
}