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

using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using System;
using System.Collections.Generic;

namespace BH.Engine.Adapters.Karamba3D
{
    internal class BHoMModel
    {
        private readonly IDictionary<string, Loadcase> _loadCases = new Dictionary<string, Loadcase>();

        internal IDictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();

        internal IDictionary<int, Bar> Elements1D { get; set; } = new Dictionary<int, Bar>();

        internal List<ILoad> Loads { get; set; } = new List<ILoad>();

        internal ICollection<Loadcase> LoadCases => _loadCases.Values;

        internal IDictionary<Guid, IMaterialFragment> Materials { get; set; } = new Dictionary<Guid, IMaterialFragment>();

        internal IDictionary<Guid, ISectionProperty> CrossSections { get; set; } = new Dictionary<Guid, ISectionProperty>();

        internal bool TryGetLoadCase(string loadCaseName, out Loadcase loadcase)
        {
            return _loadCases.TryGetValue(loadCaseName, out loadcase);
        }

        internal Loadcase RegisterLoadCase(string loadCaseName)
        {
            if (_loadCases.TryGetValue(loadCaseName, out var loadCase))
            {
                return loadCase;
            }

            loadCase = new Loadcase
            {
                Name = loadCaseName,
                Nature = LoadNature.Other,
                Number = _loadCases.Count + 1 // Added +1 as some adapters (Robot) require loadcases starting from 1 (not 0)
            };

            _loadCases.Add(loadCaseName, loadCase);

            return loadCase;
        }
    }
}