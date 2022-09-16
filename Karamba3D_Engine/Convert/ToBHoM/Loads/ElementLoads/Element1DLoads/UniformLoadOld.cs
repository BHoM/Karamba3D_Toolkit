namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this UniformlyDistLoad_OLD k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);
            var bars = k3dLoad.GetElementIndices(k3dModel).Select(i => bhomModel.Elements1D[i]);

            yield return new BarUniformlyDistributedLoad()
            {
                Force = (k3dLoad.Load).ToBhOM(),
                Moment = new Vector(),
                Loadcase = bhomModel.RegisterLoadCase(k3dLoad.LcName),
                Objects = new BHoMGroup<Bar> { Elements = bars.ToList() },
                Axis = loadAxis,
                Projected = isProjected,
            };
        }

    }
}