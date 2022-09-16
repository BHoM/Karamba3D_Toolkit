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

using BH.oM.Base;
using BH.oM.Adapters.Karamba3D;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Karamba.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.MaterialFragments;
using Karamba.Utilities;

namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Models;

    public static partial class Convert
    {
        // Entry point
        public static IObject IToBhOM(this object obj)
        {
            return ToBhOM(obj as dynamic);
        }

        // Fallback method
        private static IObject ToBhOM(this object obj)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }

        /***************************************************/
    }
}


