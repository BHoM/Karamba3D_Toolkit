namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Models;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using oM.Base;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBhOM(this Karamba.Loads.GravityLoad k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            yield return new GravityLoad()
            {
                Axis = LoadAxis.Global,
                GravityDirection = k3dLoad.force.ToBhOM(),
                Projected = false,
                Loadcase = null,
                Objects = new BHoMGroup<BHoMObject> { Elements = bhomModel.Elements1D.Values.Cast<BHoMObject>().ToList()}
            };
        }
        
    }
}