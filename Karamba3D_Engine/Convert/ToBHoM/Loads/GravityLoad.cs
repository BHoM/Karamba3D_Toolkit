namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Models;
    using oM.Structure.Loads;
    using System.Collections.Generic;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBhOM(this Karamba.Loads.GravityLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
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