namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Karamba.Elements;
    using oM.Base;
    using oM.Structure.Elements;

    public static partial class Convert
    {
        public static IEnumerable<IBHoMObject> ToBhOM(this Karamba.Models.Model k3dModel)
        {
            var bhomNodes = new Dictionary<int, Node>(k3dModel.nodes.Count);
            var bhomElements = new Dictionary<int, Bar>();

            // Convert all the nodes
            foreach (var node in k3dModel.nodes)
            {
                bhomNodes[node.ind] = node.ToBHoM();
            }

            // Convert all the elements 1D
            var beams = k3dModel.elems.OfType<ModelElementStraightLine>();
            foreach (var beam in beams)
            {
                var bhomElement = beam.ToBhOM(k3dModel, bhomNodes);

                if(bhomElement != null)
                {
                    bhomElements[beam.ind] = bhomElement;
                }
            }

            // Convert all the loads
            // Convert all the supports

            return  bhomElements.Values;
        }
        
    }
}