namespace BH.Engine.Adapters.Karamba3D
{
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static PointLoad ToBhOM(this Karamba.Loads.PointLoad k3dPointLoad, BhOMModel bhomModel)
        {
            var nodes = new BHoMGroup<Node>();
            nodes.Elements.Add(bhomModel.Nodes[k3dPointLoad.node_ind]);

            return new PointLoad()
            {
                Axis = LoadAxis.Global,
                Loadcase = new Loadcase { Name = k3dPointLoad.LcName },
                Force = k3dPointLoad.force.ToBhOM(),
                Moment = k3dPointLoad.moment.ToBhOM(),
                Projected = false,
                Objects = nodes,
            };

        }
    }
}