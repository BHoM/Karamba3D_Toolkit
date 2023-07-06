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

using BH.oM.Structure.Constraints;
using Karamba.Joints;
using Karamba.Utilities;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        internal static BarRelease ToBHoM(this Joint k3dJoint)
        {
            if (k3dJoint?.c is null)
            {
                return null;
            }

            // Assign start release
            var ucf = UnitsConversionFactory.Conv();
            var N_m = ucf.conversion["N/m"];
            var Nm = ucf.conversion["Nm"];


            var startRelease = new Constraint6DOF();
            if (k3dJoint.c[0].HasValue)
            {
                startRelease.TranslationX = DOFType.Spring;
                startRelease.TranslationalStiffnessX = N_m.toUnit(k3dJoint.c[0].Value);
            }
            else
            {
                startRelease.TranslationX = DOFType.Fixed;
            }

            if (k3dJoint.c[1].HasValue)
            {
                startRelease.TranslationY = DOFType.Spring;
                startRelease.TranslationalStiffnessY = N_m.toUnit(k3dJoint.c[1].Value);
            }
            else
            {
                startRelease.TranslationY = DOFType.Fixed;
            }
            
            if (k3dJoint.c[2].HasValue)
            {
                startRelease.TranslationZ = DOFType.Spring;
                startRelease.TranslationalStiffnessZ = N_m.toUnit(k3dJoint.c[2].Value);
            }
            else
            {
                startRelease.TranslationZ = DOFType.Fixed;
            }

            if (k3dJoint.c[3].HasValue)
            {
                startRelease.RotationX = DOFType.Spring;
                startRelease.RotationalStiffnessX = Nm.toUnit(k3dJoint.c[3].Value);
            }
            else
            {
                startRelease.RotationX = DOFType.Fixed;
            }

            if (k3dJoint.c[4].HasValue)
            {
                startRelease.RotationY = DOFType.Spring;
                startRelease.RotationalStiffnessY = Nm.toUnit(k3dJoint.c[4].Value);
            }
            else
            {
                startRelease.RotationY = DOFType.Fixed;
            }

            if (k3dJoint.c[5].HasValue)
            {
                startRelease.RotationZ = DOFType.Spring;
                startRelease.RotationalStiffnessZ = Nm.toUnit(k3dJoint.c[5].Value);
            }
            else
            {
                startRelease.RotationZ = DOFType.Fixed;
            }

            // Assign end release
            var endRelease = new Constraint6DOF();
            if (k3dJoint.c[6].HasValue)
            {
                endRelease.TranslationX = DOFType.Spring;
                endRelease.TranslationalStiffnessX = N_m.toUnit(k3dJoint.c[6].Value);
            }
            else
            {
                endRelease.TranslationX = DOFType.Fixed;
            }

            if (k3dJoint.c[7].HasValue)
            {
                endRelease.TranslationY = DOFType.Spring;
                endRelease.TranslationalStiffnessY = N_m.toUnit(k3dJoint.c[7].Value);
            }
            else
            {
                endRelease.TranslationY = DOFType.Fixed;
            }
            
            if (k3dJoint.c[8].HasValue)
            {
                endRelease.TranslationZ = DOFType.Spring;
                endRelease.TranslationalStiffnessZ = N_m.toUnit(k3dJoint.c[8].Value);
            }
            else
            {
                endRelease.TranslationZ = DOFType.Fixed;
            }

            if (k3dJoint.c[9].HasValue)
            {
                endRelease.RotationX = DOFType.Spring;
                endRelease.RotationalStiffnessX = Nm.toUnit(k3dJoint.c[9].Value);
            }
            else
            {
                endRelease.RotationX = DOFType.Fixed;
            }

            if (k3dJoint.c[10].HasValue)
            {
                endRelease.RotationY = DOFType.Spring;
                endRelease.RotationalStiffnessY = Nm.toUnit(k3dJoint.c[10].Value);
            }
            else
            {
                endRelease.RotationY = DOFType.Fixed;
            }

            if (k3dJoint.c[11].HasValue)
            {
                endRelease.RotationZ = DOFType.Spring;
                endRelease.RotationalStiffnessZ = Nm.toUnit(k3dJoint.c[11].Value);
            }
            else
            {
                endRelease.RotationZ = DOFType.Fixed;
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