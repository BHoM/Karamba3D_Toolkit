namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using Adapter.Karamba3D;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> IToBhOM(this Load k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            return  (IEnumerable<ILoad>)ToBhOM(k3dLoad as dynamic, k3dModel, bhomModel);
        }
    }
}