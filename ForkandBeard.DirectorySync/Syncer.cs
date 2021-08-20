using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Syncer
    {
        public static void SyncAllDirectories(string root1, string root2)
        {
            // TODO: Implement.
        }

        public static void SyncDirectories(string root1, string root2, string directory1, string directory2)
        {
            Index index1;
            Index index2;

            index1 = Index.Load(root1, directory1);
            index2 = Index.Load(root2, directory2);

            if(directory1 != directory2)
            {
                Logger.Log(root1, $"Cannot sync directories, directory names do not match. {directory1} vs {directory2}.");
                Logger.Log(root2, $"Cannot sync directories, directory names do not match. {directory1} vs {directory2}.");
                return;
            }

            Logger.Log(root1, $"Syncing directories {directory1}: {root1} <==> {root2}.");
            Logger.Log(root2, $"Syncing directories {directory2}: {root2} <==> {root1}.");

            if (index1 == null)
            {
                Logger.Log(root1, $"Cannot sync directories, no index found @ {directory1}.");
                return;
            }

            if (index2 == null)
            {
                Logger.Log(root2, $"Cannot sync directories, no index found @ {directory2}.");
                return;
            }

            SyncDirectories(root1, root2, directory1, directory2, index1, index2);
        }

        private static void SyncDirectories(string root1, string root2, string directory1, string directory2, Index index1, Index index2)
        {
            List<string> filesIn1NotIn2;
            List<string> filesIn2NotIn1;

            filesIn1NotIn2 = index1.Files.Except(index2.Files).ToList();
            filesIn2NotIn1 = index2.Files.Except(index1.Files).ToList();

            // TODO: Implement.
        }
    }
}
