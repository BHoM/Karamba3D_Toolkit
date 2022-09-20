namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using Karamba.Models;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this Karamba.Loads.GravityLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            yield return new GravityLoad()
            {
                Name = string.Empty,
                Axis = LoadAxis.Global,
                GravityDirection = k3dLoad.force.ToBhOM(),
                Projected = false,
                Loadcase = null,
            };
        }
        
    }
}