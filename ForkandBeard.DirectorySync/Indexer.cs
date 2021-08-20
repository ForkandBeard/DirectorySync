using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Indexer
    {
        public static void IndexAllDirectories(string root)
        {
            List<string> allDirectories;
            int counter = 0;

            Logger.Log(root, $"Indexing all directories @ root {root}...");
            allDirectories = new List<string>( System.IO.Directory.GetDirectories(root, "*", System.IO.SearchOption.AllDirectories));

            foreach(string directory in allDirectories)
            {
                counter++;
                Logger.Log(root, $"Indexing directory {counter} of {allDirectories.Count}.");
                IndexDirectory(root, directory, false);
            }

            Logger.Log(root, $"Indexed all directories @ root {root}.");
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
            index.Files = index.Files.Select(path => path.ToLower().Replace(directory.ToLower(), "")).ToList();
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
    }
}
