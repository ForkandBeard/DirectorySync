using System;
using System.Collections.Generic;
using System.Linq;
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
            this.Synced = new List<IdAndVersion>();
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

        public void Delete(string directory)
        {
            var path = GetIndexPath(directory);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public static Index Load(string root, string directory)
        {
            var path = GetIndexPath(directory);

            if (!System.IO.File.Exists(path))
            {
                Logger.Log(root, $"No index found @ {directory}.");
                return null;
            }

            Logger.Log(root, $"Loading index found @ {directory}.");
            var index = JsonSerializer.Deserialize<Index>(System.IO.File.ReadAllText(path));

            Logger.Log(root, $"Loaded index @ {directory}.");
            return index;
        }

        public static void UpdateVersion(string root, string directory, IdAndVersion newVersion)
        {
            Index index = Load(root, directory);
            IdAndVersion currentVersion;

            currentVersion = index.Synced.Where(idAndVerion => idAndVerion.Id == newVersion.Id).FirstOrDefault();

            if(currentVersion == null)
            {
                index.Synced.Add(newVersion);
            }
            else
            {
                currentVersion.Version = newVersion.Version;
            }

            index.Save(directory);
        }

        public static string GetIndexPath(string directory)
        {
            return System.IO.Path.Combine(directory, @"_fab.ds.index.json");
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
