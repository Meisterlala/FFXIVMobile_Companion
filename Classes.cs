using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFXIVMobile_Companion
{
    public static class Color
    {
        //List of colors here: https://i.imgur.com/v25JiRU.png

        /// <summary>Blue, typically an URL</summary>
        public const string Blue = "\u001b[94m";

        /// <summary>Cyan, typically ADB output or drawing attention to differences between options</summary>
        public const string Cyan = "\u001b[96m";

        /// <summary>The default console color, typically used at the end of strings</summary>
        public const string Default = "\u001b[0m";

        /// <summary>Green, typically indicating success</summary>
        public const string Green = "\u001b[92m";

        /// <summary>Magenta, used to draw attention to something</summary>
        public const string Magenta = "\u001b[95m";

        /// <summary>Red, typically an error</summary>
        public const string Red = "\u001b[91m";

        /// <summary>Yellow, typically something that -may- be wrong or may not</summary>
        public const string Yellow = "\u001b[93m";
    }

    public static class Style
    {
        //List of styles here: https://i.imgur.com/v25JiRU.png

        /// <summary>Bold</summary>
        public const string Bold = "\u001b[1m";

        /// <summary>Inverse foreground <-> background</summary>
        public const string Inverse = "\u001b[7m";

        /// <summary>Underline</summary>
        public const string Underline = "\u001b[4m";

        /// <summary>The default console style, typically used at the end of strings</summary>
        public const string Default = "\u001b[0m";
    }

    public static class ConnectionType
    {
        /// <summary>A USB connection, uses 'adb -d' for commands</summary>
        public const string USB = "USB";

        /// <summary>Wifi connection, uses 'adb -s IP_and_Port' for commands</summary>
        public const string WiFi = "WiFi";

        /// <summary>MuMu emulator, uses 'adb -s 127.0.0.1:7555' for commands</summary>
        public const string MuMu = "MuMu";

        /// <summary>BlueStacks emulator, uses 'adb -s 127.0.0.1:5555' for commands</summary>
        public const string BlueStacks = "BlueStacks";
    }

    public class GameLanguage
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }

        public static GameLanguage English = new GameLanguage { LongName = "English", ShortName = "en" };
        public static GameLanguage Japanese = new GameLanguage { LongName = "Japanese", ShortName = "ja" };
        public static GameLanguage Korean = new GameLanguage { LongName = "Korean", ShortName = "ko" };
        public static GameLanguage German = new GameLanguage { LongName = "German", ShortName = "de" };
        public static GameLanguage French = new GameLanguage { LongName = "French", ShortName = "fr" };
        public static GameLanguage Chinese = new GameLanguage { LongName = "Chinese", ShortName = "zh" };
        public static GameLanguage None = new GameLanguage { LongName = "None", ShortName = "None" };
    };

    public struct Status
    {
        [JsonPropertyName("ProgramMD5")]
        public string ProgramMD5 { get; set; }

        [JsonPropertyName("TranslationMD5")]
        public string TranslationMD5 { get; set; }

        [JsonPropertyName("ProgramUpdateURL")]
        public string ProgramUpdateURL { get; set; }
        
        [JsonPropertyName("BuildDate")]
        public string BuildDate { get; set; }
        
        [JsonPropertyName("Codename")]
        public string Codename { get; set; }
    };

    public class MyWebClient : WebClient
    {
        private int _timeout;

        public int Timeout
        {
            get { return _timeout; }

            set { _timeout = value; }
        }

        public MyWebClient()
        {
            Timeout = 60000;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            dynamic result = base.GetWebRequest(address);
            result.Timeout = _timeout;
            return result;
        }
    }
}