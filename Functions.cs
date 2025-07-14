using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMobile_Companion
{
    internal class Functions
    {
        public static void DownloadFile(string address, string filename)
        {
            MyWebClient Client = new MyWebClient { Encoding = Encoding.UTF8, Timeout = 10000, Proxy = null };

            Client.Headers.Set("user-agent", "FFXIVMC (Built: " + Program.BuildDate + ")");

            Client.Timeout = 10000;

            while (Client.IsBusy)
            {
                Thread.Sleep(16);
            }
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch
                {
                }
            }
            Client.DownloadFileAsync(new Uri(address), filename);

            while (Client.IsBusy)
            {
                Thread.Sleep(16);
            }
        }
        public static bool ValidateIPAndPort(string IPAndPort)
        {
            if (String.IsNullOrWhiteSpace(IPAndPort))
            {
                Console.WriteLine(Colors.Red + "Please enter a valid IP address and port." + Colors.Default);
                return false;
            }

            string[] splitValues = IPAndPort.Split('.');
            if (splitValues.Length != 4)
            {
                Console.WriteLine(Colors.Red + "Please enter a valid IP address and port." + Colors.Default);
                return false;
            }

            if (!IPAndPort.Contains(":")) 
            {
                Console.WriteLine(Colors.Red + "Please enter a valid IP address and port." + Colors.Default); 
                return false; 
            }

            return true;
        }
    }
}