namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation.Language;
    using System.Xml;
    using Karamba.Elements;
    using Karamba.Supports;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        public static IEnumerable<IBHoMObject> ToBhOM(this Karamba.Models.Model k3dModel)
        {
            // TODO decide if to create a bhom model to be used for con
            var bhomModel = new BhOMModel(k3dModel.nodes.Count, k3dModel.elems.Count);

            // Convert all the nodes
            foreach (var node in k3dModel.nodes)
            {
                bhomModel.Nodes[node.ind] = node.ToBhOM();
            }

            // Convert all the elements 1D
            var beams = k3dModel.elems.OfType<ModelElementStraightLine>();
            foreach (var beam in beams)
            {
                var bhomElement = beam.ToBhOM(k3dModel, bhomModel);

                if(bhomElement != null)
                {
                    bhomModel.Elements1D[beam.ind] = bhomElement;
                }
            }

            // Convert all the loads
            bhomModel.Loads.Concat(k3dModel.gravities.Values.Select(g => g.ToBhOM()));
            // TODO unique LoadCase.Number needs to be created for each LoadCase.Name
            
            // Convert all the supports and assign them to the corresponding node.
            k3dModel.supports.ForEach(s =>
            {
                var node = bhomModel.Nodes[s.node_ind];
                node.RegisterSupport(s);

                if(s.TryToConvertIntoPointDisplacement(node, out var displacementLoad))
                    bhomModel.Loads.Append(displacementLoad);
            });

            return bhomModel.ReturnBhOMEntities();
        }

        
        
    }
}