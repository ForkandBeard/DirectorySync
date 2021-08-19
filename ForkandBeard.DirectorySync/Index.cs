using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ForkandBeard.DirectorySync
{
    public class Index
    {
        public DateTime IndexCreated { get; set; }
        public List<string> Files { get; set; }

        public Index()
        {
        }

        public void Save(string directory)
        {
            var path = GetIndexPath(directory);

            if(System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            System.IO.File.WriteAllText(path, JsonSerializer.Serialize(this));
        }

        public static Index Load(string directory)
        {
            var path = GetIndexPath(directory);

            if (!System.IO.File.Exists(path))
            {
                Console.WriteLine($"No index found @ {directory}.");
                return null;
            }

            Console.WriteLine($"Loading index found @ {directory}.");
            var index = JsonSerializer.Deserialize<Index>(System.IO.File.ReadAllText(path));

            Console.WriteLine($"Loaded index @ {directory}.");
            return index;
        }

        public static string GetIndexPath(string directory)
        {
            return System.IO.Path.Combine(directory, @"fab.ds.index.json");
        }
    }
}
