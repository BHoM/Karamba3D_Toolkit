namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this Karamba.Loads.GravityLoad k3dLoad)
        {
            yield return new GravityLoad()
            {
                Name = string.Empty,
                Axis = LoadAxis.Global,
                GravityDirection = k3dLoad.force.ToBhOM(),
                Projected = false,
                Loadcase = new Loadcase()
                {
                    Name = k3dLoad.LcName,
                    Number = -1, // TODO assign this
                    Nature = LoadNature.Other,
                }
            };
        }
        
    }
}