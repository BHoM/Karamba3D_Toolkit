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

using BH.Engine.Adapter.Karamba3D;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Offsets;
using BH.oM.Structure.SectionProperties;
using Karamba.Elements;
using Karamba3D_Engine;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        internal static Bar ToBHoM(this ModelElementStraightLine k3dElement, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            if (k3dElement is ModelSpring)
            {
                K3dLogger.RecordWarning(string.Format(Resource.WarningNotYetSupportedType, nameof(ModelSpring)));
                return null;
            }
            
            // In Karamba there are 3 eccentricities:
            //- Local beam eccentricity
            //- Global beam eccentricity
            //- Local cross section eccentricity that belongs to the assigned cross section.
            // The offset combines all of them together.
            
            var eccentricityVector = k3dElement.totalEccentricity(k3dModel).ToBHoM();
            var offset = eccentricityVector == new Vector() ?
                null :
                new Offset { Start = eccentricityVector, End = eccentricityVector };
            var release = ((ModelTruss)k3dElement).joint.ToBHoM();
            return new Bar
            {
                Name = k3dElement.ind.ToString(),
                StartNode = bhomModel.Nodes[k3dElement.node_inds[0]],
                EndNode = bhomModel.Nodes[k3dElement.node_inds[1]],
                SectionProperty = (ISectionProperty)k3dElement.crosec.IToBHoM(k3dModel, bhomModel),
                FEAType = k3dElement is ModelBeam ? BarFEAType.Flexural : BarFEAType.Axial,
                Offset = offset,
                OrientationAngle = k3dElement.res_alpha, // TODO check how it works with vertical elements.
                Release = release
            };
        }
    }
}