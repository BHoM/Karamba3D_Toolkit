namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this PointMass k3dPointMass, Model k3dModel, BhOMModel bhomModel)
        {
            Base.Compute.RecordError(
                string.Format(
                    Resource.WarningNotSupportedType,
                    nameof(PointMass)));

            return Enumerable.Empty<ILoad>();
        }
    }
}