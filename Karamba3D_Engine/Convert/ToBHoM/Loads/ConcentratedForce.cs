namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Geometry;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static BarPointLoad ToBhOM(this Karamba.Loads.Beam.ConcentratedForce k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var bhomLoad = ((ConcentratedLoad)k3dLoad).ToBhOM(k3dModel, bhomModel);
            bhomLoad.Force = k3dLoad.Values.ToBhOM();

            return bhomLoad;
        }
    }
}