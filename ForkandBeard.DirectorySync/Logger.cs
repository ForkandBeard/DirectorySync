using System;
using System.Collections.Generic;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Logger
    {
        public static void Log(string root, string message)
        {
            var logPath = GetLogPath(root);

            message = $"== {DateTime.Now} == {message}";

            Console.WriteLine(message);
            System.IO.File.AppendAllText(logPath, message + Environment.NewLine);
        }

        private static string GetLogPath(string root)
        {
            return System.IO.Path.Combine(root, "fab.ds.log.txt");
        }

        public static void DeleteLogs(string root1, string root2)
        {
            System.IO.File.Delete(GetLogPath(root1));
            System.IO.File.Delete(GetLogPath(root2));
        }
    }
}
