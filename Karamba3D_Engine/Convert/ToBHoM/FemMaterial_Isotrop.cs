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
    using Karamba.Materials;

    public static partial class Convert
    {
        public static IIsotropic ToBHoM(this Karamba.Materials.FemMaterial_Isotrop k3dMaterial)
        {
            // TODO: Check Karamba settings for Units of Measure (INIReader reader)
            // OR use the automated Karamba conversion (which only converts numbers if they are not in the right Unit - works only at runtime via Karamba)

            switch (GetMaterialType(k3dMaterial))
            {
                case MaterialType.Aluminium:
                {
                    return new Aluminium()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(), //(obj.E() / (2 * obj.G12())) - 1,
                        Name = k3dMaterial.name // this may store the type of aluminum
                    };
                }

                case MaterialType.Steel:
                {
                    return new Steel()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),
                        Name = k3dMaterial.name,
                        ThermalExpansionCoeff = k3dMaterial.alphaT(),
                        UltimateStress = k3dMaterial.ft(),
                        YieldStress = k3dMaterial.ft() // TODO: where's Fy in Karamba?
                    };
                }

                case MaterialType.Concrete:
                {
                    return new Concrete()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),
                        Name = k3dMaterial.name,
                        ThermalExpansionCoeff = k3dMaterial.alphaT(),
                        CylinderStrength = k3dMaterial.fc(),
                    };
                }

                default:
                {
                    return new GenericIsotropicMaterial()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),//(obj.E() / (2 * obj.G12())) - 1,
                        Name = k3dMaterial.name
                    };
                }
            }
        }

        private static MaterialType GetMaterialType(FemMaterial material)
        {
            // TODO Should search inside K3D database. This require to have static method in Karamba to find our database.

            switch (material.family)
            {
                case "Steel":
                    return MaterialType.Steel;

                case "Wood":
                case "Hardwood":
                case "ConiferousTimber":
                case "GlulamTimber":
                case "Aluminum":
                    return MaterialType.Timber;

                case "Concrete":
                case "LightweightConcrete":
                    return MaterialType.Concrete;

                case "ReinfSteel":
                    return MaterialType.Rebar;

                default:
                    return MaterialType.Undefined;
            }
        }
    }
}


