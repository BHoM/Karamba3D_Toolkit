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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using Karamba.Supports;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Query
    {
        public static Constraint6DOF ToBHoM(this Support obj)
        {
            if (obj.hasLocalCoosys)
            {
                BH.Engine.Base.Compute.RecordError("Local coordinate system not implemented yet");
                return null;
            }

            // TODO add prescribed displacements.

            var support = Structure.Create.Constraint6DOF(obj.Condition[0], obj.Condition[1], obj.Condition[2], obj.Condition[3], obj.Condition[4], obj.Condition[5]);
            support.CustomData.Add(nameof(obj.indexSet), obj.indexSet);
            support.CustomData.Add(nameof(obj.positionSet), obj.positionSet);
            support.CustomData.Add(nameof(obj.node_ind), obj.node_ind);
            support.CustomData.Add(nameof(obj.position), obj.position);
            return support;
        }
    }
}