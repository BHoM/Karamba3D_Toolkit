namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Karamba.Elements;
    using Karamba.Supports;
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
            var loads = k3dModel.gravities.Values.Select(g => g.ToBhOM());

            // Convert all the supports and assign them to the corresponding node.
            k3dModel.supports.ForEach(s => bhomNodes[s.node_ind].RegisterSupport(s));

            return bhomElements.Values.Cast<IBHoMObject>()
                               .Concat(bhomNodes.Values)
                               .Concat(loads);
        }

        
        
    }
}