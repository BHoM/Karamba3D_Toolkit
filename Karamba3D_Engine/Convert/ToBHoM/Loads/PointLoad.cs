namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using Adapter.Karamba3D;
    using Karamba.Models;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        private static IEnumerable<ILoad> ToBhOM(this Karamba.Loads.PointLoad k3dPointLoad, Model k3dModel, BhOMModel bhomModel)
        {
            if (k3dPointLoad.local)
            {
                K3dLogger.RecordWarning(Resource.WarningPointLoadLocalLoadNotSupported);
            }

            yield return new PointLoad
            {
                Axis = LoadAxis.Global,
                Loadcase = null,
                Force = k3dPointLoad.force.ToBhOM(),
                Moment = k3dPointLoad.moment.ToBhOM(),
                Projected = false,
                Objects = new BHoMGroup<Node> { Elements = new List<Node> { bhomModel.Nodes[k3dPointLoad.node_ind] } },
            };

        }
    }
}