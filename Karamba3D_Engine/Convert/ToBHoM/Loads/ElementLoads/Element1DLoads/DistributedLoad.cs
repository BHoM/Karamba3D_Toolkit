namespace BH.Engine.Adapters.Karamba3D
{
    using System;
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
        public static IEnumerable<ILoad> ToBhOM(this DistributedLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            if (k3dLoad.Positions.Count == 2 &&
                k3dLoad.Positions[0] == 0 &&
                Math.Abs(k3dLoad.Positions[1] - 1) < double.Epsilon &&
                k3dLoad.Values.Distinct().Count() == 1)
            {
                return CreateUniformLoad(k3dLoad, k3dModel ,bhomModel);
            }

            return CreateNonUniformLoad(k3dLoad, k3dModel, bhomModel);

        }

        private static IEnumerable<ILoad> CreateNonUniformLoad(DistributedLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);

            for (int i = 0; i < k3dLoad.Positions.Count - 1; i++)
            {
                yield return new BarVaryingDistributedLoad()
                {
                    StartPosition = k3dLoad.Positions[i],
                    EndPosition = k3dLoad.Positions[i + 1],
                    ForceAtStart = k3dLoad is DistributedForce ? (k3dLoad.Values[i] * k3dLoad.Direction).ToBhOM() : new Vector(),
                    ForceAtEnd = k3dLoad is DistributedForce ? (k3dLoad.Values[i+1] * k3dLoad.Direction).ToBhOM() : new Vector(),
                    MomentAtStart = k3dLoad is DistributedMoment ? (k3dLoad.Values[i] * k3dLoad.Direction).ToBhOM() : new Vector(),
                    MomentAtEnd = k3dLoad is DistributedMoment ? (k3dLoad.Values[i+1] * k3dLoad.Direction).ToBhOM() : new Vector(),
                    Loadcase = null,
                    Axis = loadAxis,
                    Projected = isProjected,
                    RelativePositions = true
                };
            }

            
        }

        private static IEnumerable<ILoad> CreateUniformLoad(DistributedLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            k3dLoad.GetOrientation(out var loadAxis, out var isProjected);
            var bars = k3dLoad.GetElementIndices(k3dModel).Select(i => bhomModel.Elements1D[i]);

            yield return new BarUniformlyDistributedLoad()
            {
                Force = k3dLoad is DistributedForce ? (k3dLoad.Values.First() * k3dLoad.Direction).ToBhOM() : new Vector(),
                Moment = k3dLoad is DistributedForce ? (k3dLoad.Values.First() * k3dLoad.Direction).ToBhOM() : new Vector(),
                Loadcase = null,
                Objects = new BHoMGroup<Bar> { Elements = bars.ToList() },
                Axis = loadAxis,
                Projected = isProjected,
            };
        }
    }
}