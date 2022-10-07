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
        private static IEnumerable<ILoad> ToBhOM(this StrainLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var message = string.Format(Resource.WarningNotSupportedType, nameof(StrainLoad));
            K3dLogger.RecordWarning(message);

            return Enumerable.Empty<ILoad>();
        }
    }
}