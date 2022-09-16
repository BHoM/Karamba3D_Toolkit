namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this StrainLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var message = string.Format(Resource.WarningNotSupportedType, typeof(StrainLoad));
            Base.Compute.RecordWarning(message);

            return Enumerable.Empty<ILoad>();
        }
    }
}