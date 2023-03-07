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

using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Loads;
using Karamba.Elements;
using Karamba.Loads;
using Karamba.Supports;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        private static IEnumerable<Load> GetLoads(this Karamba.Models.Model k3dModel)
        {
            return ((IEnumerable<Load>)k3dModel.ploads)
                   .Concat(k3dModel.pmass)
                   .Concat(k3dModel.mloads)
                   .Concat(k3dModel.eloads)
                   .Concat(k3dModel.gravities.Values);
        }

        private static BH.oM.Karamba3D.FemModel ToFemModel(this BHoMModel bhomModel)
        {
            return new oM.Karamba3D.FemModel()
            {
                Nodes = bhomModel.Nodes.Values.ToList(),
                Bars = bhomModel.Elements1D.Values.ToList(),
                Loads = bhomModel.Loads.ToList(),
                LoadCases = bhomModel.LoadCases.ToList(),
                Materials = bhomModel.Materials.Values.ToList(),
                CrossSections = bhomModel.CrossSections.Values.ToList(),
            };
        }
        
        private static void RegisterDoFsToNode(Node bhomNode, Support k3dSupport)
        {
            var bhomDoFs = Structure.Create.Constraint6DOF(
                k3dSupport.Condition[0],
                k3dSupport.Condition[1],
                k3dSupport.Condition[2],
                k3dSupport.Condition[3],
                k3dSupport.Condition[4],
                k3dSupport.Condition[5],
                name: null);

            bhomNode.Support = bhomDoFs;
            
        }

        private static void RegisterOrientationToNode(Node bhomNode, Support k3dSupport)
        {
            bhomNode.Orientation = k3dSupport.local_coosys.ToBHoM();
        }

        private static void RegisterPointDisplacement(BHoMModel bhomModel, Support k3dSupport, Node bhomNode)
        {
            var translation = new BH.oM.Geometry.Vector()
            {
                X = k3dSupport._displacement[0],
                Y = k3dSupport._displacement[1],
                Z = k3dSupport._displacement[2],
            };

            var rotation = new BH.oM.Geometry.Vector()
            {
                X = k3dSupport._displacement[3],
                Y = k3dSupport._displacement[4],
                Z = k3dSupport._displacement[5],
            };

            var load = new PointDisplacement
            {
                Loadcase = bhomModel.RegisterLoadCase(k3dSupport.LcName),
                Objects = new BHoMGroup<Node> { Elements = new List<Node> { bhomNode } },
                Translation = translation,
                Rotation = rotation,
                Axis = k3dSupport.hasLocalCoosys ? LoadAxis.Local : LoadAxis.Global,
                Projected = false,
            };

            bhomModel.Loads.AddRange(bhomModel.Loads.Concat(new[] { load }));
        }

        private static bool IsConvertibleToPointDisplacement(this Support k3dSupport)

        {
            return (!(k3dSupport._displacement?.All(v => v == 0) ?? true)) && 
                   k3dSupport.LcName != null &&
                   k3dSupport._displacement.Length == 6;
        }

        internal static BHoMModel ToBHoMModel(this Karamba.Models.Model k3dModel)
        {
            var bhomModel = new BHoMModel();

            // Convert nodes.
            foreach (var k3dNode in k3dModel.nodes)
            {
                var bhomNode = k3dNode.ToBHoM();
                if (bhomNode is null)
                    continue;

                bhomModel.Nodes[k3dNode.ind] = bhomNode;
            }

            // Convert 1D elements.
            foreach (var k3dBeam in  k3dModel.elems.OfType<ModelElementStraightLine>())
            {
                var bhomBar = k3dBeam.ToBHoM(k3dModel, bhomModel);
                if (bhomBar is null)
                    continue;

                bhomModel.Elements1D[k3dBeam.ind] = bhomBar;
            }

            // Convert loads and add load cases.
            foreach (var k3dLoad in k3dModel.GetLoads())
            {
                var obj = k3dLoad.IToBHoM(k3dModel, bhomModel);
                if (!(obj is IEnumerable<ILoad> bhomLoads))
                    continue;

                foreach (var bhomLoad in bhomLoads)
                {
                    bhomLoad.Loadcase = bhomModel.RegisterLoadCase(k3dLoad.LcName);
                    bhomModel.Loads.Add(bhomLoad);
                }
            }
            
            // Register supports infos to nodes and loads.
            foreach (var k3dSupport in k3dModel.supports)
            {
                if (!bhomModel.Nodes.TryGetValue(k3dSupport.node_ind, out var bhomNode))
                    continue;

                RegisterDoFsToNode(bhomNode, k3dSupport);

                if (k3dSupport.hasLocalCoosys)
                    RegisterOrientationToNode(bhomNode, k3dSupport);
                if (k3dSupport.IsConvertibleToPointDisplacement())
                    RegisterPointDisplacement(bhomModel, k3dSupport, bhomNode);
            }

            return bhomModel;
        }

        [Description("Converts a Karamba model to a set of BHoMObjects.")]
        [Input("k3dModel", "The Karamba model to convert.")]
        [Output("femModel", "The container of all converted BHoM objects.")]
        public static oM.Karamba3D.FemModel ToBHoM(this Karamba.Models.Model k3dModel)
        {
            return k3dModel.ToBHoMModel().ToFemModel();
        }
    }
}