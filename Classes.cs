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
        // Summary:
        //      Blue, typically an URL
        public const string Blue = "\u001b[94m";

        // Summary:
        //      The default console color, typically used at the end of strings
        public const string Default = "\u001b[0m";

        // Summary:
        //      Green, typically indicating success
        public const string Green = "\u001b[32m";

        // Summary:
        //      Red, typically an error
        public const string Red = "\u001b[31m";
        
        // Summary:
        //      Yellow, typically something that -may- be wrong or may not
        public const string Yellow = "\u001b[93m";
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