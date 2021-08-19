using System;

namespace ForkandBeard.DirectorySync
{
    class Program
    {
        static void Main(string[] args)
        {
            var root1 = args[0];
            var root2 = args[1];

            Console.WriteLine("[i]ndex only, [s]ync only or [b]oth?");
            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.B:
                case ConsoleKey.I:
                    Indexer.IndexAllDirectories(root1);
                    Indexer.IndexAllDirectories(root2);
                    break;
            }
        }
    }
}
