/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Data.Library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using Karamba3D_Engine;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the data inside a BHoM dataset of cross sections.")]
        [Input("dataSetNameOrPath", "The path of the data set to read from.")]
        [Input("concatenateMatchingDataSets", "Input true if matching data sets should be concatenated.")]
        public static List<T> GetCrossSectionDataSetData<T>(string dataSetNameOrPath, bool concatenateMatchingDataSets = false)
        {
            var dataSetPaths = Compute.GetDataSetPaths();
            var matchingDataSetsPaths = dataSetPaths.Where(d => d.Contains(dataSetNameOrPath)).ToList();

            if (!matchingDataSetsPaths.Any())
            {
                var message = string.Format(Resource.NoCrossSectionDataSetError, dataSetNameOrPath);
                throw new FileNotFoundException(message);
            }

            if (matchingDataSetsPaths.Count() > 1 && !concatenateMatchingDataSets)
            {
                string message = string.Format(
                    Resource.MultipleCrossSectionDataSetError,
                    dataSetNameOrPath,
                    nameof(concatenateMatchingDataSets),
                    string.Join("\n\t", matchingDataSetsPaths));
                throw new Exception($"");
            }

            matchingDataSetsPaths.Sort();
            string cacheKey = string.Join("_", matchingDataSetsPaths
                .SelectMany(n => Path.GetFileNameWithoutExtension(n).Split('\\').Reverse().Take(2).Reverse())) + ".json";

            string cacheFile = "";
            try
            {
                cacheFile = Directory.GetFiles(_cachedDataSetFolder, cacheKey).FirstOrDefault(f => f.Contains(cacheKey));
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
                cacheFile = Path.Combine(_cachedDataSetFolder, cacheKey);

            if (!_assembliesLoaded)
            {
                BH.Engine.Base.Compute.LoadAllAssemblies();
                _assembliesLoaded = true;
            }

            List<T> resultData = new List<T>();
            foreach (string dataSetPath in matchingDataSetsPaths)
            {
                Dataset dataset = BH.Engine.Library.Query.Datasets(dataSetPath.ToDataSetPath()).FirstOrDefault();
                if (dataset.Data == null || !dataset.Data.Any())
                {
                    string message = string.Format(Resource.DeserializationOfDataSetError, dataSetPath);
                    throw new SerializationException(message);
                }

                resultData.AddRange(dataset.Data?.OfType<T>());
            }

            // Cache in directory for faster reads.
            if (resultData.Any())
                try
                {
                    Directory.CreateDirectory(_cachedDataSetFolder);
                    string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(resultData, settings);
                    File.WriteAllText(cacheFile, serialized);
                }
                catch { }

            return resultData;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<string> GetDataSetPaths(string sourceFolder = _dataSetsDiskLocation, string subFolder = "")
        {
            return System.IO.Directory.GetFiles(Path.Combine(sourceFolder, subFolder), "*.json", SearchOption.AllDirectories).ToList();
        }

        /***************************************************/

        private static string ToDataSetPath(this string path)
        {
            path = path.Replace(_dataSetsDiskLocation, "");
            path = path.Replace(".json", "");
            path = path.TrimStart('\\');
            return path;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static bool _assembliesLoaded = false;
        private const string _dataSetsDiskLocation = @"C:\ProgramData\BHoM\Datasets";
        private const string _cachedDataSetFolder = @"..\..\..\Cache\CachedDatasets";
    }
}


