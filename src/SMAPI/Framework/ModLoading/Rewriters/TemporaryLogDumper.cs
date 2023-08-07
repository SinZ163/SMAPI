using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StardewModdingAPI.Framework.ModLoading.Rewriters
{
    internal static class TemporaryLogDumper
    {
        private class TemporaryModEntry
        {
            public TemporaryModEntry(IModMetadata metadata)
            {
                this.metadata = metadata;
            }

            [JsonIgnore] 
            public IModMetadata metadata;
            public List<string> rewrites = new();
            public List<string> broken = new();
        }


        private static readonly ConcurrentDictionary<string, TemporaryModEntry> moddb = new();
        internal static void AddMod(IModMetadata metadata)
        {
            moddb.TryAdd(getKey(metadata), new(metadata));
        }
        internal static void AddRewrite(IModMetadata metadata, string rewrite)
        {
            TemporaryModEntry mod = moddb[getKey(metadata)];
            mod.rewrites.Add(rewrite);
        }
        internal static void AddBroken(IModMetadata metadata, string broken)
        {
            TemporaryModEntry mod = moddb[getKey(metadata)];
            mod.broken.Add(broken);
        }

        internal static void Dump()
        {
            string output = JsonConvert.SerializeObject(moddb, Formatting.Indented);
            Directory.CreateDirectory(Path.Join(Constants.GamePath, "mod-export"));
            File.WriteAllText(Path.Join(Constants.GamePath, "mod-export", $"log-dump-{DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss", CultureInfo.InvariantCulture)}.json"), output);
        }

        private static string getKey(IModMetadata mod)
        {
            string relativePath = mod.GetRelativePathWithRoot();
            return $"{mod.DisplayName} (from {relativePath}{Path.DirectorySeparatorChar}{mod.Manifest.EntryDll})";
        }
    }
}
