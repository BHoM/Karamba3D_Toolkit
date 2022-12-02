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
        private static IEnumerable<ILoad> ToBHoM(this ConcentratedLoad k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);

            var groupedBars = from element in k3dLoad.GetLoadedK3dElements(k3dModel)
                              let length = element.characteristic_length(k3dModel.nodes)
                              let bhomBar = bhomModel.Elements1D[element.ind]
                              group bhomBar by length;
            
            foreach (var group in groupedBars)
            {
                yield return new BarPointLoad
                {
                    Force = k3dLoad is ConcentratedForce ? k3dLoad.Values.ToBHoM() : new Vector(),
                    Moment = k3dLoad is ConcentratedMoment ? k3dLoad.Values.ToBHoM() : new Vector(),
                    DistanceFromA = group.Key * k3dLoad.Position,
                    Loadcase = null,
                    Objects =  new BHoMGroup<Bar> { Elements = group.ToList() },
                    Axis = loadAxis,
                    Projected = isProjected
                };
            }
        }
    }
}