using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FFXIVMobile_Companion
{
    internal class Functions
    {
        public static void DownloadFile(string address, string filename)
        {
            var cURL_Process = new Process();
            var cURL_StartInfo = new ProcessStartInfo("cmd.exe", @"/C curl -L " + address  + " --output " + filename);
            cURL_StartInfo.UseShellExecute = true;
            cURL_StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            cURL_Process.StartInfo = cURL_StartInfo;

            cURL_Process.Start();
            cURL_Process.WaitForExit();
        }

        public static Status GetRemoteStatus()
        {
            try
            {
                string data;
                using (MyWebClient client = new MyWebClient { Encoding = Encoding.UTF8, Timeout = 10000, Proxy = null })
                {
                    data = client.DownloadString("http://cdn.arks-layer.com/na/tab_status.json");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                return JsonSerializer.Deserialize<Status>(data, options);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static bool ValidateIPAndPort(string IPAndPort)
        {
            if (string.IsNullOrWhiteSpace(IPAndPort)) { return true; }

            string[] splitValues = IPAndPort.Split('.');
            if (splitValues.Length != 4)
            {
                Console.WriteLine(Color.Red + "Please enter a valid IP address and port." + Color.Default);
                return false;
            }

            if (!IPAndPort.Contains(":")) 
            {
                Console.WriteLine(Color.Red + "Please enter a valid IP address and port." + Color.Default); 
                return false; 
            }

            return true;
        }
        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpper();
                }
            }
        }
    }
}