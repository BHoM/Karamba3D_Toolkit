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

namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba3D_Engine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Elements;
    using BH.oM.Structure.Elements;
    using BH.oM.Structure.Loads;

    public static partial class Convert
    {
        internal static List<Bar> GetLoadedBhomBars(ElementLoad k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            return k3dLoad.GetLoadedK3dElements(k3dModel)
                   .Select(e => bhomModel.Elements1D[e.ind])
                   .ToList();
        }

        internal static void GetOrientation<T>(this T k3dLoad, out LoadAxis loadAxis, out bool isProjected) 
            where T : ElementLoad
        {
            isProjected = false;
            switch (k3dLoad.LoadOrientation)
            {
                case LoadOrientation.local:
                    loadAxis = LoadAxis.Local;
                    break;

                case LoadOrientation.global:
                    loadAxis = LoadAxis.Global;
                    break;

                case LoadOrientation.proj:
                    loadAxis = LoadAxis.Global;
                    isProjected = true;
                    break;

                default:
                    var message = string.Format(
                        Resource.ErrorBarPointLoadOrientation,
                        typeof(T),
                        typeof(BarPointLoad),
                        k3dLoad.LoadOrientation.GetType());
                    throw new ArgumentOutOfRangeException(message);
            }
        }

        internal static IEnumerable<Karamba.Elements.ModelElement> GetLoadedK3dElements(this ElementLoad k3dLoad, Karamba.Models.Model k3dModel)
        {
            var elementsFromTags = Enumerable.Empty<ModelElement>();
            var elementsFromGuids = Enumerable.Empty<ModelElement>();
            if (k3dLoad is null || k3dModel is null)
            {
                return Enumerable.Empty<ModelElement>();
            }

            if (k3dLoad.ElementIds?.Any() ?? false)
            {
                elementsFromTags = k3dModel.Elements(k3dLoad.ElementIds);
            }

            if (k3dLoad.ElementGuids?.Any() ?? false)
            {
                elementsFromGuids = k3dLoad.ElementGuids.SelectMany(k3dModel.Elements);
            }

            return elementsFromTags.Concat(elementsFromGuids);
        }

        internal static IEnumerable<int> GetElementIndices(this ElementLoad k3dLoad, Karamba.Models.Model k3dModel)
        {
            var indicesFromTags = Enumerable.Empty<int>();
            var indicesFromGuids = Enumerable.Empty<int>();

            if (k3dLoad.ElementIds?.Any() ?? false)
            {
                indicesFromTags = k3dModel.ElementInds(k3dLoad.ElementIds);
            }

            if (k3dLoad.ElementGuids?.Any() ?? false)
            {
                indicesFromGuids = k3dModel.ElementInds(k3dLoad.ElementGuids);
            }

            return indicesFromTags.Concat(indicesFromGuids);
        }
    }
}