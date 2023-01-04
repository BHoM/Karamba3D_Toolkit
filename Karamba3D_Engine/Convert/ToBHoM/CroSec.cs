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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BH.oM.Spatial.ShapeProfiles;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.SectionProperties;
using Karamba.CrossSections;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Query
    {
        public static ISectionProperty ToBHoM(this CroSec obj)
        {
            if (obj == null)
                return null;

            ISectionProperty result = null;
            if (!string.IsNullOrWhiteSpace(obj.family) &&
                obj.material.family.ToLower().Contains("steel") &&
                !obj.material.family.ToLower().Contains("reinf"))
            {
                string name = obj.family;

                string number = Regex.Match(obj.name, @"\d+")?.Value;
                string sectionType = obj.name.Replace(number, string.Empty);
                sectionType = sectionType.Replace(name, string.Empty);

                if (string.IsNullOrWhiteSpace(number))
                    throw new Exception($"Could not retrieve {nameof(number)} from Karamba's cross section");

                List<ISectionProperty> allSections = new List<ISectionProperty>();
                if (obj.material.name.ToLower().Contains("steel"))
                    allSections = Compute.GetDatasetData<SteelSection>(name, true).OfType<ISectionProperty>().ToList();

                foreach (SteelSection section in allSections)
                {
                    string bhomNumber = Regex.Match(section.Name, @"\d+").Value;

                    if (bhomNumber != number)
                        continue;

                    var bhomSectionType = section.Name.Replace(name, string.Empty);
                    bhomSectionType = bhomSectionType.Replace(bhomNumber, string.Empty).Trim();

                    if (bhomSectionType != sectionType)
                        continue;

                    result = (ISectionProperty)section;
                }
            }

            if (obj.material.family.ToLower().Contains("concrete"))
            {
                // TODO: Handle different shapes. What shapes are available in Karamba?
                string sectionShape = obj.shape();

                double height = obj.getHeight();
                double width = obj.maxWidth();

                // Right now only Rectangular shape is implemented.
                ConcreteSection BHoMConcreteSection = BH.Engine.Structure.Create.ConcreteRectangleSection(height, obj.maxWidth(), obj.material.ToBHoM() as Concrete);

                return BHoMConcreteSection;
            }

            return result;
        }
    }
}