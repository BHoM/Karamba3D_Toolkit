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
    using System.Data.SqlTypes;
    using Karamba.Joints;
    using oM.Structure.Constraints;

    public static partial class Convert
    {
        internal static BarRelease ToBHoM(this Joint k3dJoint)
        {
            if (k3dJoint?.c is null)
            {
                return null;
            }

            // Assign start release
            var startRelease = new Constraint6DOF();
            if (k3dJoint.c[0].HasValue)
            {
                startRelease.TranslationX = DOFType.Spring;
                startRelease.TranslationalStiffnessX = k3dJoint.c[0].Value;
            }

            if (k3dJoint.c[1].HasValue)
            {
                startRelease.TranslationY = DOFType.Spring;
                startRelease.TranslationalStiffnessY = k3dJoint.c[1].Value;
            }
            
            if (k3dJoint.c[2].HasValue)
            {
                startRelease.TranslationZ = DOFType.Spring;
                startRelease.TranslationalStiffnessZ = k3dJoint.c[2].Value;
            }

            if (k3dJoint.c[3].HasValue)
            {
                startRelease.RotationX = DOFType.Spring;
                startRelease.RotationalStiffnessX = k3dJoint.c[3].Value;
            }

            if (k3dJoint.c[4].HasValue)
            {
                startRelease.RotationY = DOFType.Spring;
                startRelease.RotationalStiffnessY = k3dJoint.c[4].Value;
            }

            if (k3dJoint.c[5].HasValue)
            {
                startRelease.RotationZ = DOFType.Spring;
                startRelease.RotationalStiffnessZ = k3dJoint.c[5].Value;
            }

            // Assign end release
            var endRelease = new Constraint6DOF();
            if (k3dJoint.c[6].HasValue)
            {
                endRelease.TranslationX = DOFType.Spring;
                endRelease.TranslationalStiffnessX = k3dJoint.c[6].Value;
            }

            if (k3dJoint.c[7].HasValue)
            {
                endRelease.TranslationY = DOFType.Spring;
                endRelease.TranslationalStiffnessY = k3dJoint.c[7].Value;
            }
            
            if (k3dJoint.c[8].HasValue)
            {
                endRelease.TranslationZ = DOFType.Spring;
                endRelease.TranslationalStiffnessZ = k3dJoint.c[8].Value;
            }

            if (k3dJoint.c[9].HasValue)
            {
                endRelease.RotationX = DOFType.Spring;
                endRelease.RotationalStiffnessX = k3dJoint.c[9].Value;
            }

            if (k3dJoint.c[10].HasValue)
            {
                endRelease.RotationY = DOFType.Spring;
                endRelease.RotationalStiffnessY = k3dJoint.c[10].Value;
            }

            if (k3dJoint.c[11].HasValue)
            {
                endRelease.RotationZ = DOFType.Spring;
                endRelease.RotationalStiffnessZ = k3dJoint.c[11].Value;
            }

            return new BarRelease()
            {
                Name = k3dJoint.name is null || k3dJoint.name == string.Empty ? null : k3dJoint.name,
                StartRelease = startRelease,
                EndRelease = endRelease,
            };
        }
    }
}