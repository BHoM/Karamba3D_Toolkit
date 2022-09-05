using System.Collections.Generic;
using System.Runtime.InteropServices;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using Karamba.Supports;

namespace BH.Engine.Adapters.Karamba3D
{
    using System.Linq;
    using System.Threading;
    using Microsoft.SqlServer.Server;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static Constraint6DOF ToBHoM(this Support k3dSupport)
        {
            var bhomSupport = Structure.Create.Constraint6DOF(
                k3dSupport.Condition[0],
                k3dSupport.Condition[1],
                k3dSupport.Condition[2],
                k3dSupport.Condition[3],
                k3dSupport.Condition[4],
                k3dSupport.Condition[5]);

            bhomSupport.CustomData.Add(nameof(k3dSupport.indexSet), k3dSupport.indexSet);
            bhomSupport.CustomData.Add(nameof(k3dSupport.positionSet), k3dSupport.positionSet);
            bhomSupport.CustomData.Add(nameof(k3dSupport.node_ind), k3dSupport.node_ind);
            bhomSupport.CustomData.Add(nameof(k3dSupport.position), k3dSupport.position);

            return bhomSupport;
        }

        public static bool TryToConvertIntoPointDisplacement(
            this Support k3dSupport,
            Node bhomNode,
            out PointDisplacement load)

        {
            if ((k3dSupport._displacement?.All(v => v == 0) ?? true) ||
                k3dSupport.LcName == null ||
                k3dSupport._displacement.Length != 6)
            {
                load = null;
                // TODO Add error messages
                return false;
            }

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

            var loadCase = new Loadcase()
            {
                Name = k3dSupport.LcName,
                Nature = LoadNature.Other,
                Number = 0, // TODO this need to be reassigned once load will be ready.
            };

            var objects = new BHoMGroup<Node> { Elements = new List<Node> { bhomNode } };

            load = new PointDisplacement()
            {
                Name = string.Empty,
                Loadcase = loadCase,
                Objects = objects,
                Translation = translation,
                Rotation = rotation,
                Axis = k3dSupport.hasLocalCoosys ? LoadAxis.Local : LoadAxis.Global,
                Projected = false,
            };

            return true;
        }
    }
}