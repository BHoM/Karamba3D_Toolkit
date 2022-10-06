namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Base;
    using BH.oM.Structure.Constraints;
    using BH.oM.Structure.Elements;
    using BH.oM.Structure.Loads;
    using Karamba.Models;
    using Karamba.Supports;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Convert
    {
        internal static Constraint6DOF ToBhOM(this Support k3dSupport)
        {
            var bhomSupport = Structure.Create.Constraint6DOF(
                k3dSupport.Condition[0],
                k3dSupport.Condition[1],
                k3dSupport.Condition[2],
                k3dSupport.Condition[3],
                k3dSupport.Condition[4],
                k3dSupport.Condition[5],
                name: null);

            return bhomSupport;
        }

        public static void ToBhOM(this Support k3dSupport, Model k3dModel, BhOMModel bhomModel)
        {
            var bhomSupport = k3dSupport.ToBhOM();
            var bhomNode = bhomModel.Nodes[k3dSupport.node_ind];

            bhomNode.RegisterSupport(k3dSupport);

            if (k3dSupport.IsConvertibleToPointDisplacement())
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

                bhomModel.Loads = bhomModel.Loads.Concat(new[] { load });
            }
        }

        internal static void RegisterSupport (this Node node, Support support)
        {
            node.Support = support.ToBhOM();

            if (support.hasLocalCoosys)
            {
                node.Orientation = support.local_coosys.ToBhOM();
            }
        }

        private static bool IsConvertibleToPointDisplacement(this Support k3dSupport)

        {
            return (!(k3dSupport._displacement?.All(v => v == 0) ?? true)) && 
                   k3dSupport.LcName != null &&
                   k3dSupport._displacement.Length == 6;
        }
    }
}