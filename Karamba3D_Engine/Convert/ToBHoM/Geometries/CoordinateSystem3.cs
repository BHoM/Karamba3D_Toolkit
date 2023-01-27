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
    using Adapter.Karamba3D;
    using Karamba.Geometry;
    using Karamba3D_Engine;
    using oM.Geometry;
    using oM.Geometry.CoordinateSystem;

    public static partial class Convert
    {
        internal static Cartesian ToBHoM(this CoordinateSystem3 k3dCoSys)
        {
            if (k3dCoSys is null)
            {
                return null;
            }

            return new Cartesian(
                k3dCoSys.Origin.ToBHoM(),
                k3dCoSys.X.ToBHoM(),
                k3dCoSys.Y.ToBHoM(),
                k3dCoSys.Z.ToBHoM());
        }

        internal static Cartesian ToBHoM(this Vector3[] k3dVectorArray)
        {
            if (k3dVectorArray is null)
            {
                return null;
            }

            if (k3dVectorArray.Length != 3)
            {
                K3dLogger.RecordError(
                    string.Format(
                        Resource.ErrorInvalidVectorArray,
                        k3dVectorArray.GetType().GetElementType(),
                        k3dVectorArray.Length));
                return null;
            }
            
            return new Cartesian(
                new Point() { X = 0, Y = 0, Z = 0},
                k3dVectorArray[0].ToBHoM(),
                k3dVectorArray[1].ToBHoM(),
                k3dVectorArray[2].ToBHoM());
        }
    }
}