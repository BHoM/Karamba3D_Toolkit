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
    public static partial class Query
    {
        /***************************************************/
        /*** Methods                                     ***/
        /***************************************************/

        public static Point ToBHoM(this Point3 obj)
        {
            Point result = new Point()
            {
                X = obj.X,
                Y = obj.Y,
                Z = obj.Z
            };

            return result;
        }

        public static Vector ToBHoM(this Karamba.Geometry.Vector3 obj)
        {
            Vector result = new Vector()
            {
                X = obj.X,
                Y = obj.Y,
                Z = obj.Z
            };

            return result;
        }

        public static Node ToBHoM(this Karamba.Nodes.Node obj)
        {
            Node result = new Node()
            {
                Position = obj.pos_disp.ToBHoM()
            };

            return result;
        }

        public static IIsotropic ToBHoM(this Karamba.Materials.FemMaterial_Isotrop obj)
        {
            if (obj.family.ToLower().Contains("concrete"))
            {
                Concrete concrete = new Concrete()
                {
                    Density = obj.gamma(), // TODO: map units
                    YoungsModulus = obj.E(),
                    PoissonsRatio = obj.nue12(),
                    Name = obj.name,
                    ThermalExpansionCoeff = obj.alphaT(),
                    CylinderStrength = obj.fc(),
                };

                return concrete;
            }

            if (obj.family.ToLower().Contains("steel"))
            {
                Steel steel = new Steel()
                {
                    Density = obj.gamma(), // TODO: map units
                    YoungsModulus = obj.E(),
                    PoissonsRatio = obj.nue12(),
                    Name = obj.name,
                    ThermalExpansionCoeff = obj.alphaT(),
                    UltimateStress = obj.ft(),
                    YieldStress = obj.ft() // TODO: where's Fy in Karamba?
                };

                return steel;
            }

            if (obj.family.ToLower().Contains("aluminum"))
            {
                Aluminium aluminium = new Aluminium()
                {
                    Density = obj.gamma(), // TODO: map units
                    YoungsModulus = obj.E(),
                    PoissonsRatio = obj.nue12(), //(obj.E() / (2 * obj.G12())) - 1,
                    Name = obj.name // this may store the type of aluminum
                };

                return aluminium;
            }

            // Fallback: GenericIsotropicMaterial

            // TODO: Check Karamba settings for Units of Measure (INIReader reader)
            // OR use the automated Karamba conversion (which only converts numbers if they are not in the right Unit - works only at runtime via Karamba)
            GenericIsotropicMaterial genericIsotropicMaterial = new GenericIsotropicMaterial()
            {
                Density = obj.gamma(), // TODO: map units
                YoungsModulus = obj.E(),
                PoissonsRatio = obj.nue12(),//(obj.E() / (2 * obj.G12())) - 1,
                Name = obj.name
            };

            return genericIsotropicMaterial;
        }

      

        //public static Bar ToBHoM(this Karamba.Models.Model obj)
        //{

        //}

        /***************************************************/
    }
}


