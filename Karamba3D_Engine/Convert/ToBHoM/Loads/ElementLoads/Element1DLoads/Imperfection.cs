namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using Karamba3D_Engine;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this Imperfection k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var message = string.Format(Resource.WarningNotSupportedType, nameof(Imperfection));
            Base.Compute.RecordWarning(message);

            return Enumerable.Empty<ILoad>();
        }
    }
}