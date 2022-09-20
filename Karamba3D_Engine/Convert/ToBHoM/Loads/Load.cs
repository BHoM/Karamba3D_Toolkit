namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba.Utilities;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<ILoad> ToBhOM(this Load k3dLoad, Model k3dModel, BhOMModel bhomModel)
        {
            var bhomEntity =  ToBhOM(k3dLoad as dynamic, k3dModel, bhomModel);

            if (bhomEntity is IEnumerable<ILoad> loads)
            {
                foreach (var load in loads)
                {
                    if (load is null)
                        continue;

                    load.Loadcase = bhomModel.RegisterLoadCase(k3dLoad.LcName) ;
                }
            }

            return bhomEntity;
        }
    }
}