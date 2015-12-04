using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_Network.Helpers
{
    public static class StringHelper
    {
        private static List<String> _commandNames = new List<string>()
        {
            "Join",
            "Start",
            "Signoff"
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

    }
}
