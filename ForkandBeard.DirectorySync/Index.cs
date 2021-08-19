using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ForkandBeard.DirectorySync
{
    public class Index
    {
        public DateTime IndexUpdated { get; set; }
        public List<string> Files { get; set; }
        public IdAndVersion Instance { get; set; }
        public List<IdAndVersion> Synced { get; set; }

        public Index()
        {
            this.Instance = new IdAndVersion();
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

        public class IdAndVersion
        {
            public Guid Id { get; set; }
            public Guid Version { get; set; }

            public IdAndVersion()
            {
                this.Id = Guid.NewGuid();
                this.Version = Guid.NewGuid();
            }
        }
    }
}
