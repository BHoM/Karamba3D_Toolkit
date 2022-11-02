namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using Adapter.Karamba3D;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBhOM(this TemperatureLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            if (k3dLoad.incdT != Vector3.Unset ||
                k3dLoad.incdT != Vector3.Zero)
            {
                K3dLogger.RecordWarning(Resource.WarningLinearTemperatureChangesNotSupported);
            }

            yield return new BarUniformTemperatureLoad
            {
                TemperatureChange = k3dLoad.incT,
                Loadcase = null,
                Objects = new BHoMGroup<Bar> { Elements = GetLoadedBhomBars(k3dLoad, k3dModel, bhomModel)},
            };
        }
    }
}