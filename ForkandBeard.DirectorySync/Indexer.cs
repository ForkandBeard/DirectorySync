using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Indexer
    {
        public static void IndexAllDirectories(string root, bool force)
        {
            List<string> allDirectories;
            int counter = 0;

            Logger.Log(root, $"Indexing all directories @ root {root}...");
            allDirectories = new List<string>( System.IO.Directory.GetDirectories(root, "*", System.IO.SearchOption.AllDirectories));

            foreach(string directory in allDirectories)
            {
                counter++;
                Logger.Log(root, $"Indexing directory {counter} of {allDirectories.Count}.");
                IndexDirectory(root, directory, force);
            }

            Logger.Log(root, $"Indexed all directories @ root {root}.");
            Logger.Log(root, $"Indexing root @ {root}...");
            IndexDirectory(root, root, force);
            Logger.Log(root, $"Indexed root @ {root}.");
        }

        public static Guid IndexDirectory(string root, string directory, bool force)
        { 
            Index index = Index.Load(root, directory);

            if(
                (index != null)
                && (!force)
                && (!IndexingRequired(index.IndexUpdated, directory))
                )
            {
                Logger.Log(root, $"No indexing required @ {directory}.");
                return index.Instance.Version;
            }

            if (index == null)
            {
                index = new Index();
            }

            Logger.Log(root, $"Indexing @ {directory}...");
            index.Files = new List<string>(System.IO.Directory.GetFiles(directory, "*", System.IO.SearchOption.TopDirectoryOnly));
            index.Files = index.Files.Select(path => RemoveDirectoryFromPath(directory, path)).ToList();
            index.IndexUpdated = DateTime.Now;
            index.Instance.Version = Guid.NewGuid();

            Logger.Log(root, $"Saving index @ {directory}.");
            index.Save(directory);

            Logger.Log(root, $"Indexed @ {directory}.");

            return index.Instance.Version;
        }

        public static bool IndexingRequired(DateTime lastIndex, string directory)
        {
            var indexPath = Index.GetIndexPath(directory).ToLower();

            foreach(string path in System.IO.Directory.GetFiles(directory, "*", System.IO.SearchOption.TopDirectoryOnly))
            {
                if(!path.ToLower().EndsWith(indexPath))
                {
                    var file = new System.IO.FileInfo(path);
                    if (file.CreationTime >= lastIndex)
                    {
                        return true;
                    }                    
                }
            }

            return false;
        }

        public static string RemoveRootFromPath(string root, string path)
        {
            return path.Replace(root, String.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public static string RemoveDirectoryFromPath(string directory, string path)
        {
            return path.Replace(directory, String.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public static string RemoveRootFromDirectory(string root, string directory)
        {
            return directory.Replace(root, String.Empty, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetFullDirectoryPath(string root, string directory)
        {
            if(String.IsNullOrWhiteSpace(directory))
            {
                return root;
            }

            if (directory.StartsWith(@"\"))
            {
                return System.IO.Path.Combine(root, directory.Substring(1));
            }
            else
            {
                return System.IO.Path.Combine(root, directory);
            }
        }

        public static string GetFullFilePath(string root, string directory, string file)
        {
            string directoryPath = GetFullDirectoryPath(root, directory);

            if (file.StartsWith(@"\"))
            {
                return System.IO.Path.Combine(directoryPath, file.Substring(1));
            }
            else
            {
                return System.IO.Path.Combine(directoryPath, file);
            }
        }
    }
}
