﻿using BH.oM.Structure.Elements;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        internal static Node ToBhOM(this Karamba.Nodes.Node k3dNode)
        {
            return new Node()
            {
                Name = k3dNode.ind.ToString(),
                Position = k3dNode.pos.ToBhOM()
            };
        }
    }
}