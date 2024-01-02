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

using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using Karamba.Loads.Beam;
using Karamba.Utilities;
using System.Collections.Generic;


namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        internal static IEnumerable<ILoad> ToBHoM(this UniformlyDistLoad_OLD k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            var ucf = UnitsConversionFactory.Conv();
            var N_m = ucf.conversion["N/m"];

            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);
            yield return new BarUniformlyDistributedLoad
            {
                Force = N_m.toUnit(1) * (k3dLoad.Load).ToBHoM(),
                Loadcase = null,
                Objects = new BHoMGroup<Bar> { Elements = GetLoadedBhomBars(k3dLoad, k3dModel, bhomModel) },
                Axis = loadAxis,
                Projected = isProjected,
            };
        }

    }
}
