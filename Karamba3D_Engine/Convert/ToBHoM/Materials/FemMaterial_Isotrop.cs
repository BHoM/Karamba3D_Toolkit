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

using BH.oM.Structure.MaterialFragments;
using Karamba.Materials;
using Karamba.Utilities;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        private static MaterialType GetMaterialType(FemMaterial material)
        {
            switch (material.family)
            {
                case "Steel":
                    return MaterialType.Steel;

                case "Wood":
                case "Hardwood":
                case "ConiferousTimber":
                case "GlulamTimber":
                    return MaterialType.Timber;

                case "Aluminum":
                    return MaterialType.Aluminium;

                case "Concrete":
                case "LightweightConcrete":
                    return MaterialType.Concrete;

                case "ReinfSteel":
                    return MaterialType.Rebar;

                default:
                    return MaterialType.Undefined;
            }
        }

        internal static IMaterialFragment ToBHoM(this FemMaterial_Isotrop k3dMaterial, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            if (bhomModel.Materials.TryGetValue(k3dMaterial.guid, out var bhomMaterial))
            {
                return bhomMaterial;
            }

            var ucf = UnitsConversionFactory.Conv();
            var N_m2 = ucf.conversion["N/m/m"];

            IIsotropic bhomIsotropicMaterial;
            switch (GetMaterialType(k3dMaterial))
            {
                case MaterialType.Aluminium:
                {
                    bhomIsotropicMaterial = new Aluminium();
                    break;
                }

                case MaterialType.Steel:
                {
                    bhomIsotropicMaterial = new Steel()
                    {
                        UltimateStress = N_m2.toUnit(k3dMaterial.ft()),
                        YieldStress = N_m2.toUnit(k3dMaterial.ft()),
                    };
                    break;
                }

                case MaterialType.Concrete:
                {
                    bhomIsotropicMaterial = new Concrete()
                    {
                        CylinderStrength = N_m2.toUnit(-k3dMaterial.fc()),
                        CubeStrength = default,
                    };
                    break;
                }

                default:
                {
                    bhomIsotropicMaterial = new GenericIsotropicMaterial();
                    break;
                }
            }

            var N_m3 = ucf.conversion["N/m3"];

            bhomIsotropicMaterial.Density = N_m3.toUnit(k3dMaterial.gamma());
            bhomIsotropicMaterial.DampingRatio = default;
            bhomIsotropicMaterial.Name = k3dMaterial.name;
            bhomIsotropicMaterial.PoissonsRatio = k3dMaterial.nue12();
            bhomIsotropicMaterial.ThermalExpansionCoeff = k3dMaterial.alphaT();
            bhomIsotropicMaterial.YoungsModulus = N_m2.toUnit(k3dMaterial.E());

            bhomIsotropicMaterial.BHoM_Guid = k3dMaterial.guid;
            bhomModel.Materials.Add(bhomIsotropicMaterial.BHoM_Guid, bhomIsotropicMaterial);

            return bhomIsotropicMaterial;
        }
    }
}



