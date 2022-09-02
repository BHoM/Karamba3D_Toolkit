using System.Collections.Generic;
using System.Runtime.InteropServices;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using Karamba.Supports;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        public static Constraint6DOF ToBHoM(this Support k3dSupport)
        {
            // TODO add prescribed displacements.
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
    }
}