namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this TemperatureLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var barGroup = k3dLoad.GetElements(k3dModel)
                                  .OfType<ModelElementStraightLine>()
                                  .Select(e => bhomModel.Elements1D[e.ind]);
            
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);

            if (k3dLoad.incdT != Vector3.Unset)
            {
                // Create linear temperature load
                Base.Compute.RecordError("Linear temperature changes are not supported yet");
            }

            // Create uniform temperature load
            yield return new BarUniformTemperatureLoad
            {
                TemperatureChange = k3dLoad.incT,
                Loadcase = null,
                Objects = new BHoMGroup<Bar> { Elements = barGroup.ToList()},
                Axis = loadAxis,
                Projected = isProjected
            };
        }
    }
}