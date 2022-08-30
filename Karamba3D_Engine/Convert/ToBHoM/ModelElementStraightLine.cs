/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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
using System.Collections.Generic;

namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.CrossSections;
    using Karamba.Elements;
    using Karamba.Models;
    using oM.Structure.Offsets;

    public static partial class Convert
    {
        internal static Bar ToBhOM(this ModelElementStraightLine k3dElement, Model k3dModel, IDictionary<int, Node> bhomNodesMapper)
        {
            // TODO See if ModelSpring can be converted to an existing BhOM object 
            if (k3dElement is ModelSpring)
            {
                Base.Compute.RecordError("Spring is not supported yet");
                return null;
            }

            /* Release and support have no equivalent in Karamba so the won't be setup.
             */

            /*
             * Offset should be retrieve from eccentricity. In Karamba there are 3 eccentricities:
             * - Local beam eccentricity
             * - Global beam eccentricity
             * - Local cross section eccentricity that belongs to the assigned cross section.
             * The offset should combine all of them together. In custom data they should be stored as separately instead.
             */
            // TODO Store different eccentricities in custom data.
            var totalEccentricity = k3dElement.totalEccentricity(k3dModel).ToBHoM();

            return new Bar()
            {
                Name = k3dElement.ind.ToString(),
                StartNode = bhomNodesMapper[k3dElement.node_inds[0]],
                EndNode = bhomNodesMapper[k3dElement.node_inds[1]],
                SectionProperty = ((CroSec_Beam)k3dElement.crosec).ToBHoM(),
                FEAType = k3dElement is ModelBeam ? BarFEAType.Flexural : BarFEAType.Axial,
                Offset = new Offset
                {
                    Start = totalEccentricity,
                    End = totalEccentricity

                },
                OrientationAngle = k3dElement.res_alpha,
                Release = null,
                Support = null,
                Tags = new HashSet<string>(),
                CustomData = new Dictionary<string, object>()
            };
        }
    }
}