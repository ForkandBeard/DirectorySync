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
            List<string> all1Directories;
            List<string> all2Directories;
            int counter1 = 0;
            int counter2 = 0;

            Logger.Log(root1, $"Syncing all directories {root1} <==> {root2}...");
            Logger.Log(root2, $"Syncing all directories {root2} <==> {root1}...");

            all1Directories = new List<string>(System.IO.Directory.GetDirectories(root1, "*", System.IO.SearchOption.AllDirectories));
            all2Directories = new List<string>(System.IO.Directory.GetDirectories(root2, "*", System.IO.SearchOption.AllDirectories));

            // Iterate through all 1 folders first.
            foreach (string directory in all1Directories)
            {
                counter1++;
                Logger.Log(root1, $"Syncing directory {counter1} of {all1Directories.Count}.");
                SyncDirectory(root1, root2, Indexer.RemoveRootFromDirectory(root1, directory));
            }

            Logger.Log(root1, $"Synced all directories root {root1} <==> {root2}");

            // Now iterate through all 2 folders (most work should be done now).
            foreach (string directory in all2Directories)
            {
                counter2++;
                Logger.Log(root2, $"Syncing directory {counter2} of {all2Directories.Count}.");
                SyncDirectory(root2, root1, Indexer.RemoveRootFromDirectory(root2, directory));
            }

            Logger.Log(root2, $"Synced all directories root {root2} <==> {root1}");
        }

        public static void SyncDirectory(string root1, string root2, string directory)
        {
            Index index1;
            Index index2;
            string directory1;
            string directory2;

            directory1 = Indexer.GetFullDirectoryPath(root1, directory);
            directory2 = Indexer.GetFullDirectoryPath(root2, directory);

            // Ensure both directories actually exist.
            System.IO.Directory.CreateDirectory(directory1);
            System.IO.Directory.CreateDirectory(directory2);

            index1 = Index.Load(root1, directory1);
            index2 = Index.Load(root2, directory2);

            Logger.Log(root1, $"Syncing directory {directory}: {root1} <==> {root2}...");
            Logger.Log(root2, $"Syncing directory {directory}: {root2} <==> {root1}...");

            if (index1 == null)
            {
                Logger.Log(root1, $"No index found @ {directory1}. Creating new index.");
                Indexer.IndexDirectory(root1, directory1, true);
            }

            if (index2 == null)
            {
                Logger.Log(root2, $"No index found @ {directory2}. Creating new index.");
                Indexer.IndexDirectory(root2, directory2, true);
            }

            SyncDirectory(root1, root2, directory, index1, index2);

            Logger.Log(root1, $"Synced directory {directory}: {root1} <==> {root2}.");
            Logger.Log(root2, $"Synced directory {directory}: {root2} <==> {root1}.");
        }

        private static void SyncDirectory(string root1, string root2, string directory, Index index1, Index index2)
        {
            List<string> filesIn1NotIn2;
            List<string> filesIn2NotIn1;
            Index.IdAndVersion versionOf1In2;
            Index.IdAndVersion versionOf2In1;
            string sourcePath;
            string targetPath;
            string fullDirectoryPath;

            // Handle copy from 1 ==> 2.
            versionOf1In2 = index2.Synced.Where(idAndVerion2 => idAndVerion2.Id == index1.Instance.Id).FirstOrDefault(); // List could be zero if first sync but both folders are the same.

            if(versionOf1In2 == null || versionOf1In2.Version != index1.Instance.Version)
            {   // Either never synced or at different version.
                filesIn1NotIn2 = index1.Files.Except(index2.Files).ToList();
                // Do the sync.
                Logger.Log(root1, $"Copying {filesIn1NotIn2.Count} files @ {directory} to {root2}.");
                Logger.Log(root2, $"Copying {filesIn1NotIn2.Count} files @ {directory} from {root1}.");

                // Copy all missing files.
                foreach(string file in filesIn1NotIn2)
                {
                    sourcePath = Indexer.GetFullFilePath(root1, directory, file);
                    targetPath = Indexer.GetFullFilePath(root2, directory, file);
                    if (System.IO.File.Exists(targetPath))
                    {
                        Logger.Log(root1, $"Cannot copy file {targetPath}. It already exists.");
                        Logger.Log(root2, $"Cannot copy file {targetPath}. It already exists.");
                    }
                    else
                    {
                        System.IO.File.Copy(sourcePath, targetPath);
                    }
                }

                if (filesIn1NotIn2.Count > 0)
                {   // Now re-index the directory and save the latest version in it.
                    fullDirectoryPath = Indexer.GetFullDirectoryPath(root2, directory);
                    Indexer.IndexDirectory(root2, fullDirectoryPath, true);
                    Index.UpdateVersion(root2, fullDirectoryPath, index1.Instance);
                }
            }
            else
            {
                Logger.Log(root1, $"No changes to sync @ {directory} to {root2}.");
                Logger.Log(root2, $"No changes to sync @ {directory} from {root1}.");
            }

            // Handle copy from 2 ==> 1.
            versionOf2In1 = index1.Synced.Where(idAndVerion1 => idAndVerion1.Id == index2.Instance.Id).FirstOrDefault();

            if (versionOf2In1 == null || versionOf2In1.Version != index2.Instance.Version)
            {   // Either never synced or at different version.
                filesIn2NotIn1 = index2.Files.Except(index1.Files).ToList();
                // Do the sync.
                Logger.Log(root2, $"Copying {filesIn2NotIn1.Count} files @ {directory} to {root1}.");
                Logger.Log(root1, $"Copying {filesIn2NotIn1.Count} files @ {directory} from {root2}.");

                // Copy all missing files.
                foreach (string file in filesIn2NotIn1)
                {
                    sourcePath = Indexer.GetFullFilePath(root2, directory, file);
                    targetPath = Indexer.GetFullFilePath(root1, directory, file);
                    if (System.IO.File.Exists(targetPath))
                    {
                        Logger.Log(root2, $"Cannot copy file {targetPath}. It already exists.");
                        Logger.Log(root1, $"Cannot copy file {targetPath}. It already exists.");
                    }
                    else
                    {
                        System.IO.File.Copy(sourcePath, targetPath);
                    }
                }

                if (filesIn2NotIn1.Count > 0)
                {    // Now re-index the directory and save the latest version in it.
                    fullDirectoryPath = Indexer.GetFullDirectoryPath(root1, directory);
                    Indexer.IndexDirectory(root1, fullDirectoryPath, true);
                    Index.UpdateVersion(root1, fullDirectoryPath, index2.Instance);
                }
            }
            else
            {
                Logger.Log(root1, $"No changes to sync @ {directory} to {root2}.");
                Logger.Log(root2, $"No changes to sync @ {directory} from {root1}.");
            }
        }
    }
}
