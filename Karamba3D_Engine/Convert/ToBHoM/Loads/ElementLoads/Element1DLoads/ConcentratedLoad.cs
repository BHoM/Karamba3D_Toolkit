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
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using Karamba.Loads.Beam;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Utilities;

    public static partial class Convert
    {
        internal static IEnumerable<ILoad> ToBHoM(this ConcentratedLoad k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);

            var groupedBars = from element in k3dLoad.GetLoadedK3dElements(k3dModel)
                              let length = element.characteristic_length(k3dModel.nodes)
                              let bhomBar = bhomModel.Elements1D[element.ind]
                              group bhomBar by length;
            
            var ucf = UnitsConversionFactory.Conv();
            var N = ucf.conversion["N"];
            var Nm = ucf.conversion["Nm"];
            var m = ucf.conversion["m"];

            foreach (var group in groupedBars)
            {
                yield return new BarPointLoad
                {
                    Force = N.toUnit(1) * (k3dLoad is ConcentratedForce ? k3dLoad.Values.ToBHoM() : new Vector()),
                    Moment = Nm.toUnit(1) * (k3dLoad is ConcentratedMoment ? k3dLoad.Values.ToBHoM() : new Vector()),
                    DistanceFromA = m.toUnit(group.Key * k3dLoad.Position),
                    Loadcase = null,
                    Objects =  new BHoMGroup<Bar> { Elements = group.ToList() },
                    Axis = loadAxis,
                    Projected = isProjected
                };
            }
        }
    }
}
