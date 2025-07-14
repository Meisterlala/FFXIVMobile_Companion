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
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace FFXIVMobile_Companion
{
    internal class Program
    {
        public static string BuildDate;
        public static bool AdvancedMode = false;
        public static GameLanguage SelectedLanguage = GameLanguage.None;
        public static string SelectedConnectionType = "none";
        public static string ADB_IPAddress = "127.0.0.1";
        public static string ADB_Pairing_IPAddress = "127.0.0.1";
        public static string SelectedTask = "none";
        public static Random RandomNumber = new Random();

        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args.Contains("-adv"))
                {
                    AdvancedMode = true;
                }
            }
            /*TODO:
            - Add LDPlayer support
            - Implement a check system for program updates/language updates using a remote JSON
            - Implement an updater (squirrel? writing a bat file to just download the latest?)
            - Implement a config JSON to remember your IP and possibly the language you select?
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
            var entryAssembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(entryAssembly.Location);
            WriteLine(Color.Yellow + "[Built2 on " + fileInfo.LastWriteTime.ToString() + " | Codename: ???]");

            /*
            ██    ██ ██████  ██████   █████  ████████ ███████      ██████ ██   ██ ███████  ██████ ██   ██
            ██    ██ ██   ██ ██   ██ ██   ██    ██    ██          ██      ██   ██ ██      ██      ██  ██
            ██    ██ ██████  ██   ██ ███████    ██    █████       ██      ███████ █████   ██      █████
            ██    ██ ██      ██   ██ ██   ██    ██    ██          ██      ██   ██ ██      ██      ██  ██
             ██████  ██      ██████  ██   ██    ██    ███████      ██████ ██   ██ ███████  ██████ ██   ██
            */
            string CurrentMD5 = Functions.CalculateMD5(entryAssembly.Location);
            Status RemoteStatus = Functions.GetRemoteStatus();
            if (CurrentMD5 != RemoteStatus.ProgramMD5)
            {
                WriteLine("A new version is available, downloading now...");
                Functions.DownloadFile(RemoteStatus.ProgramUpdateURL, "FFXIVMobile_Companion.exe");
                Process.Start(entryAssembly.Location); // to start new instance of application
                Environment.Exit(0);
            }

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
                    WriteLine(Color.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                    WriteLine(Color.Red + "The program will now exit. Please try again after fixing the above issue.");
                    Environment.Exit(1);
                }
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip")))
                {
                    ZipFile.ExtractToDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"), Directory.GetCurrentDirectory());
                }
                else
                {
                    WriteLine(Color.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                    WriteLine(Color.Red + "The program will now exit. Please try again after fixing the above issue.");
                    Environment.Exit(1);
                }
                WriteLine(Color.Green + "ADB downloaded and installed successfully!");
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"));
            }
            if (AdvancedMode)
            {
                WriteLine(Color.Red + "[Advanced mode enabled - Here be dragons]");
            }
            WriteLine(Environment.NewLine, true);
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
                        SelectedLanguage = GameLanguage.English;
                        break;

                    case "2":
                        SelectedLanguage = GameLanguage.Japanese;
                        break;

                    case "3":
                        SelectedLanguage = GameLanguage.Korean;
                        break;

                    case "4":
                        SelectedLanguage = GameLanguage.German;
                        break;

                    case "5":
                        SelectedLanguage = GameLanguage.French;
                        break;

                    case "6":
                        SelectedLanguage = GameLanguage.Chinese;
                        break;

                    default:
                        WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine);
                        break;
                }
            } while (SelectedLanguage.LongName == "None");
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
                        break;

                    case "2":
                        SelectedConnectionType = ConnectionType.WiFi;
                        break;

                    case "3":
                        SelectedConnectionType = ConnectionType.MuMu;
                        ADB_IPAddress = "127.0.0.1:7555";
                        break;

                    case "4":
                        SelectedConnectionType = ConnectionType.BlueStacks;
                        ADB_IPAddress = "127.0.0.1:5555";
                        break;

                    default:
                        WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine);
                        break;
                }
            } while (SelectedConnectionType == "none");
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
                WriteLine("1. Go to Settings -> About Phone -> Software Information");
                WriteLine("2. Tap 'Build number' 7 times until you see 'Developer mode enabled'");
                WriteLine("3. Return to Settings, go into Developer Options");
                WriteLine("4. Go into the 'Wireless debugging' menu and enable Wireless debugging");
                WriteLine("5. Select 'Pair device with pairing code'");
                WriteLine("6. Enter the information that shows up below");
                do
                {
                    WriteLine("Enter the IP address & Port (hit enter if you are already paired): ", true);
                    ADB_Pairing_IPAddress = Console.ReadLine();
                } while (Functions.ValidateIPAndPort(ADB_Pairing_IPAddress) == false);
                if (!string.IsNullOrWhiteSpace(ADB_Pairing_IPAddress))
                {
                    WriteLine("Pairing...");
                    WriteLine(ADB("pair " + ADB_Pairing_IPAddress));
                }
                WriteLine(ADB("disconnect"));
                bool ADBConnected = false;
                do
                {
                    WriteLine("Enter the IP address & Port (not the pairing one, the one in the wireless debugging settings): ", true);
                    ADB_IPAddress = Console.ReadLine();
                    if (Functions.ValidateIPAndPort(ADB_IPAddress) == false)
                    {
                        continue;
                    }
                    string ConnectionResult = ADB("connect " + ADB_IPAddress, true);
                    WriteLine(Color.Blue + ConnectionResult);
                    if (ConnectionResult.Contains("cannot resolve host"))
                    {
                        WriteLine("Connection failed, please check your IP and port and try again!");
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

            do
            {
                switch (SelectTask())
                {
                    case "1":
                        //Change the language to the specified
                        ChangeLanguage();
                        break;

                    case "2":
                        //Download/Fix the story patch
                        UpdateStoryPatch();
                        break;

                    //case "A":
                    //case "a":
                    //    //Go to updating the MSQ dialog shit
                    //    FixStoryLanguageWithoutWiping();
                    //    break;

                    default:
                        WriteLine(Color.Red + " Invalid selection! Please try again." + Environment.NewLine);
                        break;
                }
            } while (SelectedTask == "none");
        }

        public static string SelectLanguage()
        {
            WriteLine("Please select what language you would like your game in:");
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
            WriteLine(Color.Green + "What device are you using?");
            WriteLine("1. An actual phone (" + Color.Blue + "over USB" + Color.Default + ")");
            WriteLine("2. An actual phone (" + Color.Blue + "over WiFi" + Color.Default + ")");
            WriteLine("3. MuMu");
            WriteLine("4. BlueStacks");

            Console.Write("\nType your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string SelectTask()
        {
            WriteLine(Color.Green + "What do you want to do?");
            WriteLine("1. Change language to " + Color.Blue + SelectedLanguage.LongName);
            WriteLine("2. Download/Update the " + Color.Blue + SelectedLanguage.LongName + Color.Default + " story patch");
            //if (AdvancedMode)
            //{
            //    WriteLine(Color.Red + "A. [ADV] Rename PAK Files without wiping data (UI fix)");
            //}
            Console.Write("\nType your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static void ChangeLanguage()
        {
            WriteLine(Environment.NewLine);
            /*
            adb\adb.exe -s %ip% shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
	        adb\adb.exe -s %ip% shell echo "[Internationalization]\\nCulture=%lang%" ">>" /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
            */
            if (IsGameOpen())
            {
                WriteLine("Closing game");
                WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            }
            WriteLine("Removing old config");
            WriteLine(ADB("shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini"));
            string StringToWrite = @"[Internationalization]\\nCulture=" + SelectedLanguage.ShortName;
            WriteLine("Changing language to " + SelectedLanguage.LongName);
            WriteLine(ADB("shell echo " + StringToWrite + " >> /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini"));
            bool DoesPAKExist = ADBFileExist("/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak");
            if (DoesPAKExist)
            {
                WriteLine("Renaming PAK files to fix language issues");
                //adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
                string ADBResult = ADB("shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak");
                WriteLine(ADBResult);
                if (ADBResult.Contains("ERROR - "))
                {
                    //TODO: See if we can work around this, need to reset my phone to test
                    //ADBResult = ADB("shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks_" + RandomNumber.Next(9999));
                    //Failed to move the PAK file, let's try the renaming trick?
                    //echo [0mCreating folder for language fix[36m
                    //adb\adb.exe -s %ip% shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/
                }
            }
            WriteLine("Opening game, enjoy!");
            WriteLine(ADB("shell monkey -p com.tencent.tmgp.fmgame 1 >/dev/null 2>&1"));
        }

        public static void UpdateStoryPatch()
        {
            WriteLine(Environment.NewLine);
            /*
            adb\adb.exe -s %ip% shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
	        adb\adb.exe -s %ip% shell echo "[Internationalization]\\nCulture=%lang%" ">>" /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
            */
            if (IsGameOpen())
            {
                WriteLine("Closing game");
                WriteLine(ADB("shell am force-stop com.tencent.tmgp.fmgame"));
            }

            WriteLine("Downloading the latest " + SelectedLanguage.LongName + " translations");
            try
            {
                Functions.DownloadFile("https://github.com/Aida-Enna/FFXIVM_Language_Selector/blob/main/other/FDataBaseLoc.db?raw=true", Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db"));
            }
            catch
            {
                WriteLine(Color.Red + "Failed to download Localization DB! Please download it from https://github.com/Aida-Enna/FFXIVM_Language_Selector/blob/main/other/FDataBaseLoc.db?raw=true and put it with this program.");
                WriteLine(Color.Red + "The program will now exit. Please try again after fixing the above issue.");
                Environment.Exit(1);
            }
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "FDataBaseLoc.db")))
            {
                WriteLine(Color.Red + "Failed to download Localization DB! Please download it from https://github.com/Aida-Enna/FFXIVM_Language_Selector/blob/main/other/FDataBaseLoc.db?raw=true and put it with this program.");
                WriteLine(Color.Red + "The program will now exit. Please try again after fixing the above issue.");
                Environment.Exit(1);
            }
            WriteLine(Color.Green + "Translations downloaded successfully!");

            //adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database_orig
            //adb\adb.exe -s %ip% shell mkdir /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database
            ADB("shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database_" + RandomNumber.Next(9999));
            ADB("shell mkdir /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database");
            ADB("push FDataBaseLoc.db /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database");
            ADB("shell chmod -w /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/FDataBaseLoc.db");

            WriteLine("Opening game, enjoy!");
            WriteLine(ADB("shell monkey -p com.tencent.tmgp.fmgame 1 >/dev/null 2>&1"));
        }

        public static string ADB(string Command, bool RawCommand = false)
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
                            Command = "-s " + ADB_IPAddress + " " + Command;
                            break;
                    }
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
                ADBProcess.WaitForExit();
                string SuccessString = ADBProcess.StandardOutput.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim();
                if (string.IsNullOrWhiteSpace(SuccessString))
                {
                    string ErrorString = ADBProcess.StandardError.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim();
                    if (string.IsNullOrWhiteSpace(ErrorString))
                    {
                        return "";
                    }
                    else
                    {
                        return Color.Red + "ERROR - " + ErrorString;
                    }
                }
                else
                {
                    return Color.Cyan + SuccessString;
                }
            }
            catch (Exception ex)
            {
                return "Something went wrong with ADB - " + ex.ToString();
            }
        }

        public static bool IsGameOpen()
        {
            string IsGameOpenString = ADB("pidof com.tencent.tmgp.fmgame");
            if (!string.IsNullOrWhiteSpace(IsGameOpenString))
            {
                return true;
            }
            return false;
        }

        public static bool ADBFileExist(string Filename)
        {
            string ADBFileExistString = ADB("ls " + Filename);
            if (string.IsNullOrWhiteSpace(ADBFileExistString))
            {
                return false;
            }
            return true;
        }

        public static void WriteLine(string Text, bool NoNewLine = false)
        {
            if (Text == "") { return; }
            if (NoNewLine)
            {
                Console.Write(Text + Color.Default);
            }
            else
            {
                Console.WriteLine(Text + Color.Default);
            }
        }
    }
}