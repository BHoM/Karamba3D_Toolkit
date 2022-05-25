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
            return System.IO.Directory.GetFiles(Path.Combine(sourceFolder, subFolder), "*.*", SearchOption.AllDirectories);

            //FileAdapter fa = new FileAdapter(sourceFolder);
            //FileDirRequest req = new FileDirRequest() { IncludeFiles = true, SearchSubdirectories = true };
            //IEnumerable<object> result = fa.Pull(req);
            //return null;
        }


        public static IEnumerable<object> ReadDataSets()
        {
            foreach (var path in GetDataSetPaths())
            {
                var dataSets = BH.Engine.Library.Query.Datasets("HE");


                var jsonSettings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                };
                string values = System.IO.File.ReadAllText(path);

                object obj = BH.Engine.Serialiser.Convert.FromJson(values);

                yield return Newtonsoft.Json.JsonConvert.DeserializeObject(values, jsonSettings);
            }
        }

    }
}
