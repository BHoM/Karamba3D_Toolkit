using System;
using System.Collections.Generic;
using System.IO;
using BH.oM.Data.Library;
using Newtonsoft.Json;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Query
    {
        public static IList<string> GetDataSetPaths(string subFolder = "SectionProperties")
        {
            string sourceFolder = @"C:\ProgramData\BHoM\Datasets";
            return Directory.GetFiles(Path.Combine(sourceFolder, subFolder), "*.*", SearchOption.AllDirectories);
        }


        public static IEnumerable<object> ReadDataSets()
        {
            foreach (var path in GetDataSetPaths())
            {
                var jsonSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                };
                string values = System.IO.File.ReadAllText(path);
                values = values.Replace("$", string.Empty);
                yield return Newtonsoft.Json.JsonConvert.DeserializeObject(values, jsonSettings);
            }
        }

    }
}
