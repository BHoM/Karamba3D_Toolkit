namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using Adapter.Karamba3D;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBhOM(this PointMass k3dPointMass, Model k3dModel, BhOMModel bhomModel)
        {
            K3dLogger.RecordWarning(string.Format(Resource.WarningNotYetSupportedType, nameof(PointMass)));
            
            return Enumerable.Empty<ILoad>();
        }
    }
}