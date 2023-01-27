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


using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Structure.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Modify
    {
        public static void SetIdFragment<T>(this List<object> iElementLoads, List<IBHoMObject> pulledObjects) where T : IBHoMObject
        {
            // Just used for testing. Should modify iElementLoads by ref but needs work.

            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach (IBHoMObject pulledObject in pulledObjects)
            {
                var stringhash = string.Join("", pulledObject.GeometryHash());
                dict.Add(stringhash, pulledObject);
            }

            foreach (object iElementLoad in iElementLoads)
            {
                T loadObject = (iElementLoad as dynamic).Objects.Elements[0];
                var stringhash = string.Join("", loadObject.GeometryHash());

                if (dict.TryGetValue(stringhash, out object pulledObj))
                {
                    loadObject.AddFragment(((IBHoMObject)pulledObj).Fragments.FirstOrDefault());
                }
            }
        }
    }
}
