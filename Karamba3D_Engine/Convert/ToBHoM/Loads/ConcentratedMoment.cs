namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System.Linq;

    public static partial class Convert
    {
        public static BarPointLoad ToBhOM(this Karamba.Loads.Beam.ConcentratedMoment k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var bhomLoad = ((ConcentratedLoad)k3dLoad).ToBhOM(k3dModel, bhomModel);
            bhomLoad.Moment = k3dLoad.Values.ToBhOM();
            
            return bhomLoad;
        }
    }
}