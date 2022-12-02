namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Structure.Elements;

    public static partial class Convert
    {
        internal static Node ToBHoM(this Karamba.Nodes.Node k3dNode)
        {
            if (k3dNode is null)
                return null;

            return new Node()
            {
                Name = k3dNode.ind.ToString(),
                Position = k3dNode.pos.ToBHoM()
            };
        }
    }
}