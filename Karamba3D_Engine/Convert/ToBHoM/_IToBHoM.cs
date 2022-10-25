namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using Adapter.Karamba3D;
    using BH.oM.Base;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        // Fallback methods
        private static IObject ToBhOM(this object obj)
        {
            K3dLogger.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }

        private static IEnumerable<ILoad> ToBhOM(this object obj, Model k3dModel, BhOMModel bhomModel)
        {
            K3dLogger.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return null;
        }
    }
}


