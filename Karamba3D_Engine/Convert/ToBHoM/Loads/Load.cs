namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        // Entry point
        public static IEnumerable<ILoad> ToBhOM(this Load k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            return ToBhOM(k3dLoad as dynamic, k3dModel, bhomModel);
        }

        // Fallback method
        private static IEnumerable<ILoad> ToBhOM(this Load obj)
        {
            BH.Engine.Base.Compute.RecordError($"Could not find a convert method for {obj.GetType().FullName}.");
            return Enumerable.Empty<ILoad>();
        }
    }
}