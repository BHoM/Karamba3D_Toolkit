namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this StrainLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var message = string.Format(Resource.WarningNotSupportedType, nameof(StrainLoad));
            Base.Compute.RecordWarning(message);

            return Enumerable.Empty<ILoad>();
        }
    }
}