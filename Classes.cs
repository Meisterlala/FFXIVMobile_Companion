using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVMobile_Companion
{ 
    public static class Colors
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

        /// <summary>Red, typically an error</summary>
        public const string Red = "\u001b[91m";

        /// <summary>Yellow, typically something that -may- be wrong or may not</summary>
        public const string Yellow = "\u001b[93m";
    }

    public static class ConnectionTypes
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