namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBHoM(this DistributedLoad k3dLoad, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            var positions = k3dLoad.Positions.ToArray();
            var values = k3dLoad.Values.ToArray();
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);
            var objects = new BHoMGroup<Bar> { Elements = GetLoadedBhomBars(k3dLoad, k3dModel, bhomModel).ToList() };

            if (positions.Length == 2 &&
                Math.Abs(positions[0] - 0) < double.Epsilon &&
                Math.Abs(positions[1] - 1) < double.Epsilon &&
                k3dLoad.Values.Distinct().Count() == 1)
            {
                // Create uniform load
                yield return new BarUniformlyDistributedLoad()
                {
                    Force = k3dLoad is DistributedForce ? (k3dLoad.Values.First() * k3dLoad.Direction).ToBHoM() : new Vector(),
                    Moment = k3dLoad is DistributedMoment ? (k3dLoad.Values.First() * k3dLoad.Direction).ToBHoM() : new Vector(),
                    Loadcase = null,
                    Axis = loadAxis,
                    Projected = isProjected,
                    Objects = objects,
                };
            }
            else
            {
                // Create non uniform load
                for (int i = 0; i < positions.Length - 1; i++)
                {
                    yield return new BarVaryingDistributedLoad()
                    {
                        StartPosition = positions[i],
                        EndPosition = positions[i + 1],
                        ForceAtStart = k3dLoad is DistributedForce ? (values[i] * k3dLoad.Direction).ToBHoM() : new Vector(),
                        ForceAtEnd = k3dLoad is DistributedForce ? (values[i + 1] * k3dLoad.Direction).ToBHoM() : new Vector(),
                        MomentAtStart = k3dLoad is DistributedMoment ? (values[i] * k3dLoad.Direction).ToBHoM() : new Vector(),
                        MomentAtEnd = k3dLoad is DistributedMoment ? (values[i + 1] * k3dLoad.Direction).ToBHoM() : new Vector(),
                        Loadcase = null,
                        Axis = loadAxis,
                        Projected = isProjected,
                        RelativePositions = true,
                        Objects = objects,
                    };
                }
            }
        }
    }
}