using System;
using System.Collections.Generic;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Logger
    {
        public static void Log(string root, string message)
        {
            message = $"== {DateTime.Now} == {message}";

            Console.WriteLine(message);
            System.IO.File.AppendAllText(GetLogPath(root), message);
            System.IO.File.AppendAllText(GetLogPath(root), Environment.NewLine);
        }

        private static string GetLogPath(string root)
        {
            return System.IO.Path.Combine(root, "fab.ds.log.txt");
        }
    }
}
