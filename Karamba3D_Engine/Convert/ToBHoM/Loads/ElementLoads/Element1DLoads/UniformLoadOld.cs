namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBHoM(this UniformlyDistLoad_OLD k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);
            yield return new BarUniformlyDistributedLoad
            {
                Force = (k3dLoad.Load).ToBHoM(),
                Loadcase = null,
                Objects = new BHoMGroup<Bar> { Elements = GetLoadedBhomBars(k3dLoad, k3dModel, bhomModel) },
                Axis = loadAxis,
                Projected = isProjected,
            };
        }

    }
}