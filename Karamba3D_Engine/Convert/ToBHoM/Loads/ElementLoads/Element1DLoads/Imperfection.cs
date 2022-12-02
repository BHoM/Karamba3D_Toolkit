namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using Adapter.Karamba3D;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBHoM(this Imperfection k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            var message = string.Format(Resource.WarningNotYetSupportedType, nameof(Imperfection));
            K3dLogger.RecordWarning(message);

            return Enumerable.Empty<ILoad>();
        }
    }
}