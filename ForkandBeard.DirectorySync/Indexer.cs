using System;
using System.Collections.Generic;
using System.Text;

namespace ForkandBeard.DirectorySync
{
    public class Indexer
    {
        public static void IndexAllDirectories(string root)
        {
            List<string> allDirectories;
            int counter = 1;

            Console.WriteLine($"Indexing all directories @ root {root}.");
            allDirectories = new List<string>( System.IO.Directory.GetDirectories(root, "*", System.IO.SearchOption.AllDirectories));

            foreach(string directory in allDirectories)
            {
                Console.WriteLine($"Indexing directory {counter} of {allDirectories.Count}.");
                IndexDirectory(directory, false);
            }
        }


        public static void IndexDirectory(string directory, bool force)
        { 
            Index index = Index.Load(directory);

            if(
                (index != null)
                && (!force)
                && (!IndexingRequired(index.IndexCreated, directory))
                )
            {
                Console.WriteLine($"No indexing required @ {directory}.");
                return;
            }

            index = new Index();

            Console.WriteLine($"Indexing @ {directory}...");
            index.Files = new List<string>(System.IO.Directory.GetFiles(directory, "*", System.IO.SearchOption.TopDirectoryOnly));
            index.Files = index.Files.ForEach(path => path.ToLower().Replace(directory.ToLower(), "")).ToList();
            index.IndexCreated = DateTime.Now;

            Console.WriteLine($"Saving index @ {directory}.");
            index.Save(directory);

            Console.WriteLine($"Indexed @ {directory}.");
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
