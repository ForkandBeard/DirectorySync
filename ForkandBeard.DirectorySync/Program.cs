using System;

namespace ForkandBeard.DirectorySync
{
    class Program
    {
        static void Main(string[] args)
        {
            var root1 = args[0];
            var root2 = args[1];

            Console.WriteLine("[i]ndex only, [s]ync only or [b]oth? ([f]orce re-index only)");
            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.B:
                    Logger.DeleteLogs(root1, root2);
                    Indexer.IndexAllDirectories(root1, false);
                    if (root1 == root2)
                    {
                        return;
                    }
                    Indexer.IndexAllDirectories(root2, false);
                    Syncer.SyncAllDirectories(root1, root2);
                    break;

                case ConsoleKey.I:
                    Logger.DeleteLogs(root1, root2);
                    Indexer.IndexAllDirectories(root1, false);
                    if(root1 == root2)
                    {
                        return;
                    }
                    Indexer.IndexAllDirectories(root2, false);
                    break;
                case ConsoleKey.F:
                    Logger.DeleteLogs(root1, root2);
                    Indexer.IndexAllDirectories(root1, true);
                    if (root1 == root2)
                    {
                        return;
                    }
                    Indexer.IndexAllDirectories(root2, true);
                    break;
                case ConsoleKey.S:
                    Logger.DeleteLogs(root1, root2);
                    if (root1 == root2)
                    {
                        return;
                    }
                    Syncer.SyncAllDirectories(root1, root2);
                    break;
            }

        }
    }
}
