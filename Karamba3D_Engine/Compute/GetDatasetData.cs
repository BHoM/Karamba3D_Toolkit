/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Base;
using BH.oM.Data.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<T> GetDatasetData<T>(string datasetNameOrPath, bool concatenateMatchingDatasets = false)
        {
            List<string> datasetPaths = Compute.GetDatasetPaths();
            List<string> matchingDatasetsPaths = datasetPaths.Where(d => d.Contains(datasetNameOrPath)).ToList();
            if (!matchingDatasetsPaths.Any())
                throw new Exception($"No dataset matching `{datasetNameOrPath}` was found.");

            if (matchingDatasetsPaths.Count() > 1 && !concatenateMatchingDatasets)
                throw new Exception($"More than one matching dataset found for `{datasetNameOrPath}`. " +
                    $"Please specify full path, OR set `{nameof(concatenateMatchingDatasets)}` to true. Datasets found:\n\t{string.Join("\n\t", matchingDatasetsPaths)}");

            matchingDatasetsPaths.Sort();
            string cacheKey = string.Join("_", matchingDatasetsPaths
                .SelectMany(n => Path.GetFileNameWithoutExtension(n).Split('\\').Reverse().Take(2).Reverse())) + ".json";

            string cacheFile = "";
            try
            {
                cacheFile = Directory.GetFiles(_cachedDatasetFolder, cacheKey).FirstOrDefault(f => f.Contains(cacheKey));
            }
            catch { }

            JsonSerializerSettings settings = new JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto };
            if (!string.IsNullOrEmpty(cacheFile))
            {
                string serialized = File.ReadAllText(cacheFile);
                List<T> deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(serialized, settings);

                return deserialized;
            }
            else
                cacheFile = Path.Combine(_cachedDatasetFolder, cacheKey);

            // LoadAllAssemblies is required by the BHoM_Engine datasets reading.
            if (!_assembliesLoaded)
            {
                BH.Engine.Base.Compute.LoadAllAssemblies();
                _assembliesLoaded = true;
            }

            List<T> resultData = new List<T>();
            foreach (string datasetPath in matchingDatasetsPaths)
            {
                Dataset dataset = BH.Engine.Library.Query.Datasets(datasetPath.ToDatasetPath()).FirstOrDefault();
                if (dataset.Data == null || !dataset.Data.Any())
                    throw new Exception($"Could not deserialize the Dataset `{datasetPath}`. " +
                        $"\nCheck that the dataset file has not been altered (for example by opening it in a code editor and formatted): try building the visual studio solution `BHoM_Dataset` or re-installing BHoM." +
                        $"\nOtherwise, check that you are not using BHoM Nuget packages and mixing references from disk and Nuget.");
                resultData.AddRange(dataset.Data?.OfType<T>());
            }

            // Cache in directory for faster reads.
            if (resultData.Any())
                try
                {
                    Directory.CreateDirectory(_cachedDatasetFolder);
                    string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(resultData, settings);
                    File.WriteAllText(cacheFile, serialized);
                }
                catch { }

            return resultData;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<string> GetDatasetPaths(string sourceFolder = _datasetsDiskLocation, string subFolder = "")
        {
            return System.IO.Directory.GetFiles(Path.Combine(sourceFolder, subFolder), "*.json", SearchOption.AllDirectories).ToList();
        }

        /***************************************************/

        private static string ToDatasetPath(this string path)
        {
            path = path.Replace(_datasetsDiskLocation, "");
            path = path.Replace(".json", "");
            path = path.TrimStart('\\');
            return path;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static bool _assembliesLoaded = false;
        private const string _datasetsDiskLocation = @"C:\ProgramData\BHoM\Datasets";
        private const string _cachedDatasetFolder = @"..\..\..\Cache\CachedDatasets";
    }
}
