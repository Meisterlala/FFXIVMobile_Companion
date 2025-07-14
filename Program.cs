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

namespace FFXIVMobile_Companion
{
    internal class Program
    {
        public static string BuildDate;
        public static bool AdvancedMode = false;
        public static string SelectedLanguage = "none";
        public static string SelectedConnectionType = "none";
        public static string ADB_IPAddress = "127.0.0.1";
        public static string ADB_Pairing_IPAddress = "127.0.0.1";

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
            WriteLine("[Final Fantasy XIV Mobile Companion, created by Aida Enna]");
            WriteLine("Source code: " + Colors.Blue + "https://github.com/Aida-Enna/FFXIVMobile_Companion");
            var entryAssembly = Assembly.GetEntryAssembly();
            var fileInfo = new FileInfo(entryAssembly.Location);
            WriteLine(Colors.Yellow + "[Built on " + fileInfo.LastWriteTime.ToString() + "]", true);
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"adb\adb.exe")))
            {
                WriteLine(Colors.Red + "ADB Folder/file does not exist! (Checked \"" + Directory.GetCurrentDirectory() + "\")\nAttempting to fix by downloading required files...");
                try
                {
                    Functions.DownloadFile("https://github.com/Aida-Enna/FFXIVMobile_Companion/blob/main/extras/adb.zip?raw=true", "adb.zip");
                }
                catch
                {
                    WriteLine(Colors.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                    WriteLine(Colors.Red + "The program will now exit. Please try again after fixing the above issue.");
                    Environment.Exit(1);
                }
                if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip")))
                {
                    ZipFile.ExtractToDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"), Directory.GetCurrentDirectory());
                }
                else
                {
                    WriteLine(Colors.Red + "Failed to download ADB! Please re-extract the zip file you downloaded and make sure to extract -all- the files!");
                    WriteLine(Colors.Red + "The program will now exit. Please try again after fixing the above issue.");
                    Environment.Exit(1);
                }
                WriteLine(Colors.Green + "ADB downloaded and installed successfully!");
                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), @"adb.zip"));
            }
            if (AdvancedMode) { WriteLine(Colors.Red + "[Advanced mode enabled - Here be dragons]"); }
            WriteLine(Environment.NewLine);
            do
            {
                switch (SelectLanguage())
                {
                    case "1":
                        SelectedLanguage = "en";
                        break;

                    case "2":
                        SelectedLanguage = "jp";
                        break;

                    case "3":
                        SelectedLanguage = "ko";
                        break;

                    case "4":
                        SelectedLanguage = "de";
                        break;

                    case "5":
                        SelectedLanguage = "fr";
                        break;

                    case "6":
                        SelectedLanguage = "zh";
                        break;

                    default:
                        WriteLine(Colors.Red + " Invalid selection! Please try again." + Environment.NewLine);
                        break;
                }
            } while (SelectedLanguage == "none");

            WriteLine(Environment.NewLine);
            do
            {
                switch (SelectConnectionType())
                {
                    case "1":
                        SelectedConnectionType = ConnectionTypes.USB;
                        break;

                    case "2":
                        SelectedConnectionType = ConnectionTypes.WiFi;
                        break;

                    case "3":
                        SelectedConnectionType = ConnectionTypes.MuMu;
                        ADB_IPAddress = "127.0.0.1:7555";
                        break;

                    case "4":
                        SelectedConnectionType = ConnectionTypes.BlueStacks;
                        ADB_IPAddress = "127.0.0.1:5555";
                        break;

                    default:
                        WriteLine(Colors.Red + " Invalid selection! Please try again." + Environment.NewLine);
                        break;
                }
            } while (SelectedConnectionType == "none");

            if (SelectedConnectionType == ConnectionTypes.USB)
            {
                WriteLine(" 1. Go to Settings → About Phone → Software Information");
                WriteLine(" 2. Tap 'Build number' 7 times until you see 'Developer mode enabled'");
                WriteLine(" 3. Return to Settings, go into Developer Options");
                WriteLine(" 4. Enable USB Debugging");
                WriteLine(" 5. Plug in your phone and hit 'OK' if an 'Allow USB debugging?' menu pops up");
                WriteLine(" 5. Press enter once plugged in and ready");
                WriteLine(" (If you have a Xiaomi device, you may need to enable 'USB debugging(Security Settings)' and 'Install via USB')");
                Console.ReadLine();
                WriteLine(" Attempting to complete task via USB...");
            }
            else if (SelectedConnectionType == ConnectionTypes.WiFi)
            {
                WriteLine(" 1. Go to Settings → About Phone → Software Information");
                WriteLine(" 2. Tap 'Build number' 7 times until you see 'Developer mode enabled'");
                WriteLine(" 3. Return to Settings, go into Developer Options");
                WriteLine(" 4. Go into the 'Wireless debugging' menu and enable Wireless debugging");
                WriteLine(" 5. Select 'Pair device with pairing code'");
                WriteLine(" 6. Enter the information that shows up below");
                do
                {
                    WriteLine("Enter the IP address & Port (hit enter if you are already paired): ", true);
                    ADB_Pairing_IPAddress = Console.ReadLine();
                } while (Functions.ValidateIPAndPort(ADB_Pairing_IPAddress) == false);
                
                WriteLine("Pairing...");
                WriteLine(Colors.Blue + ADB("pair " + ADB_Pairing_IPAddress));
                WriteLine(Colors.Blue + ADB("disconnect"));
                //adb\adb.exe pair ADB_Pairing_IPAddress
                //adb\adb.exe disconnect
                WriteLine("Enter the IP address & Port (not the pairing one, the one in the wireless debugging settings): ", true);
                ADB_IPAddress = Console.ReadLine();
                WriteLine("Attempting to complete task via WiFi...");
            }
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

            Console.Write("Type your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string SelectOperation()
        {
            WriteLine(Colors.Green + "What would you like to do?");
            WriteLine("1. Download the latest non-UI translations");
            WriteLine(Environment.NewLine);
            if (AdvancedMode)
            {
                WriteLine(Colors.Red + "A. [ADV] Change language without wiping data");
                WriteLine(Colors.Red + "B. [ADV] Rename PAK Files without wiping data");
            }
            Console.Write("Type your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string SelectConnectionType()
        {
            WriteLine(Colors.Green + "What device are you using?");
            WriteLine("1. An actual phone (" + Colors.Blue + "over USB" + Colors.Default + ")");
            WriteLine("2. An actual phone (" + Colors.Blue + "over WiFi" + Colors.Default + ")");
            WriteLine("3. MuMu");
            WriteLine("4. BlueStacks");

            WriteLine(Environment.NewLine);
            Console.Write("Type your choice: ");
            return Console.ReadKey().KeyChar.ToString();
        }

        public static string ADB(string Command)
        {
            try
            {
                switch (SelectedConnectionType)
                {
                    case ConnectionTypes.USB:
                        Command = "-d " + Command;
                        break;
                    case ConnectionTypes.WiFi:
                    case ConnectionTypes.MuMu:
                    case ConnectionTypes.BlueStacks:
                        Command = "-s " + ADB_IPAddress + " " + Command;
                        break;
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
                string ReturnString = ADBProcess.StandardOutput.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim();
                if (string.IsNullOrWhiteSpace(ReturnString))
                {
                    return Colors.Red + "ERROR - " + ADBProcess.StandardError.ReadToEnd().Replace("/r/n", "").Replace("/n", "").Trim(); ;
                }
                else
                {
                    return Colors.Cyan + ReturnString; 
                }                    
            }
            catch(Exception ex)
            {
                return "Something went wrong with ADB - " + ex.ToString();
            }
        }

        public static void WriteLine(string Text, bool NoNewLine = false)
        {
            if (NoNewLine)
            {
                Console.Write(Text + Colors.Default);
            }
            else
            {
                Console.WriteLine(Text + Colors.Default);
            }
        }
    }
}