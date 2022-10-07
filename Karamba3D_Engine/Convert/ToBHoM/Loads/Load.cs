namespace BH.Engine.Adapters.Karamba3D
{
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