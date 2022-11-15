namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using Karamba.Elements;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Karamba.Supports;
    using oM.Structure.Loads;
    using oM.Structure.SectionProperties;

    public static partial class Convert
    {
        private static IEnumerable<Load> GetLoads(this Model k3dModel)
        {
            return ((IEnumerable<Load>)k3dModel.ploads)
                   .Concat(k3dModel.pmass)
                   .Concat(k3dModel.mloads)
                   .Concat(k3dModel.eloads)
                   .Concat(k3dModel.gravities.Values);
        }

        private static BH.oM.Karamba3D.FemModel ToFemModel(this BhOMModel bhomModel)
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
            bhomNode.Orientation = k3dSupport.local_coosys.ToBhOM();
        }

        private static void RegisterPointDisplacement(BhOMModel bhomModel, Support k3dSupport, Node bhomNode)
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

        internal static BhOMModel ToBhomModel(this Karamba.Models.Model k3dModel)
        {
            var bhomModel = new BhOMModel();

            // Convert nodes.
            foreach (var k3dNode in k3dModel.nodes)
            {
                var bhomNode = k3dNode.ToBhOM();
                if (bhomNode is null)
                    continue;

                bhomModel.Nodes[k3dNode.ind] = bhomNode;
            }

            // Convert 1D elements.
            foreach (var k3dBeam in  k3dModel.elems.OfType<ModelElementStraightLine>())
            {
                var bhomBar = k3dBeam.ToBhOM(k3dModel, bhomModel);
                if (bhomBar is null)
                    continue;

                bhomModel.Elements1D[k3dBeam.ind] = bhomBar;
            }

            // Convert loads and add load cases.
            foreach (var k3dLoad in k3dModel.GetLoads())
            {
                var obj = k3dLoad.IToBhOM(k3dModel, bhomModel);
                if (!(obj is IEnumerable<ILoad> bhomLoads))
                    continue;

                foreach (var bhomLoad in bhomLoads)
                {
                    bhomLoad.Loadcase = bhomModel.RegisterLoadCase(k3dLoad.LcName);
                }
                bhomModel.Loads.AddRange(bhomLoads);
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

        public static oM.Karamba3D.FemModel ToBhOM(this Karamba.Models.Model k3dModel)
        {
            return k3dModel.ToBhomModel().ToFemModel();
        }
    }
}