using System.Collections.Generic;
using System.Runtime.InteropServices;
using BH.Engine.Base;
using BH.oM.Structure.Constraints;
using Karamba.Supports;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Query
    {
        public static Constraint6DOF ToBHoM(this Support obj)
        {
            if (obj.hasLocalCoosys)
            {
                
                Compute.RecordError("Local coordinate system not implemented yet");
                return null;
            }

            // TODO add prescribed displacements.

            var support = Structure.Create.Constraint6DOF(obj.Condition[0], obj.Condition[1], obj.Condition[2], obj.Condition[3], obj.Condition[4], obj.Condition[5]);
            support.CustomData.Add(nameof(obj.indexSet), obj.indexSet);
            support.CustomData.Add(nameof(obj.positionSet), obj.positionSet);
            support.CustomData.Add(nameof(obj.node_ind), obj.node_ind);
            support.CustomData.Add(nameof(obj.position), obj.position);
            return support;
        }
    }
}