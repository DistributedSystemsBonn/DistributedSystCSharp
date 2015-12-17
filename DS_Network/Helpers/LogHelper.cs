using System;

namespace DS_Network.Helpers
{
    public static class LogHelper
    {
        public static void WriteError(string errorMsg)
        {
            Console.WriteLine("Error: " + errorMsg);
        }

        public static void WriteStatus(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
