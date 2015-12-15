using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DS_Network.Helpers
{
    public static class StringHelper
    {
        private static List<String> _commandNames = new List<string>()
        {
            "join",
            "start",
            "signoff",
            "start",
            "gethosts",
            "election"
        }; 

        public static String[] GetCommandArray(string command)
        {
            var newArr = new List<String>();
            var arr = command.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);

            if (!_commandNames.Contains(arr[0]))
            {
                return null;
            }

            foreach (var str in arr)
            {
                if (!String.IsNullOrWhiteSpace(str))
                {
                    newArr.Add(str);
                }
            }

            return newArr.ToArray();
        }

        public static bool IsWithParameter(string[] command)
        {
            return command.Length == 2;
        }

        public static IPAddress ConvertIpAddress(string ip)
        {
            IPAddress toJoinAddress;
            if (!IPAddress.TryParse(ip, out toJoinAddress))
            {
                throw new ArgumentException("Parameter is not IP address");
            }

            return toJoinAddress;
        }

        public static int GetPort(string port)
        {
            int res = 0;
            if (!Int32.TryParse(port, out res))
            {
                throw new ArgumentException("Parameter is not int in port");
            }

            return res;
        }
    }
}
