namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this TranslationalGap k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            Base.Compute.RecordError(
                string.Format(
                    Resource.WarningNotSupportedType,
                    typeof(TranslationalGap)));

            return Enumerable.Empty<ILoad>();
        }
    }
}