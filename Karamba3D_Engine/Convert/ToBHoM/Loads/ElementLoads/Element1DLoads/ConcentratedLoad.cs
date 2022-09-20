namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Elements;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        internal static IEnumerable<ILoad> ToBhOM(this ConcentratedLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);

            var groupedBars = from element in k3dLoad.GetElements(k3dModel)
                              let nodes = element.node_inds.Select(i => k3dModel.nodes[i]).ToList()
                              let length = element.characteristic_length(nodes)
                              let bhomBar = bhomModel.Elements1D[element.ind]
                              group bhomBar by length;


            foreach (var group in groupedBars)
            {
                yield return new BarPointLoad
                {
                    Force = k3dLoad is ConcentratedForce ? k3dLoad.Values.ToBhOM() : new Vector(),
                    Moment = k3dLoad is ConcentratedMoment ? k3dLoad.Values.ToBhOM() : new Vector(),
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