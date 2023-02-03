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
    using System;
    using System.Collections.Generic;
    using Adapter.Karamba3D;
    using BH.oM.Base;
    using Karamba.Materials;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static ISet<Type> UnsupportedType = new HashSet<Type>()
        {
            typeof(FemMaterial_Orthotropic),
        };

        internal static object IToBHoM(this object obj, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            return ToBHoM(obj as dynamic, k3dModel, bhomModel);
        }

        // Fallback methods
        private static IObject ToBHoM(this object obj)
        {
            K3dLogger.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }

        private static IObject ToBHoM(this object obj, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            if (UnsupportedType.Contains(obj.GetType()))
            {
                K3dLogger.RecordWarning(string.Format(Resource.WarningNotYetSupportedType, obj.GetType().FullName));
            }
            else
            {
                K3dLogger.RecordError(string.Format(Resource.ErrorConverterNotFound, obj.GetType().FullName));
            }
            return null;
        }
    }
}



