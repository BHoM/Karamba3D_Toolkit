namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation.Language;
    using System.Xml;
    using Karamba.Elements;
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba.Supports;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public static partial class Convert
    {
        private static IEnumerable<Load> GetLoads(this Model k3dModel)
        {
            return ((IEnumerable<Load>)k3dModel.ploads)
                   .Concat(k3dModel.pmass)
                   .Concat(k3dModel.mloads)
                   .Concat(k3dModel.eloads)
                   .Concat(k3dModel.gravities.Values);
        }

        public static IEnumerable<IBHoMObject> ToBhOM(this Karamba.Models.Model k3dModel)
        {
            var bhomModel = new BhOMModel(k3dModel.nodes.Count, k3dModel.elems.Count);

            // Convert nodes
            k3dModel.nodes.ForEach( n => bhomModel.Nodes[n.ind] = n.ToBhOM());

            // Convert 1D elements
            var beams = k3dModel.elems.OfType<ModelElementStraightLine>();
            foreach (var beam in beams)
            {
                var bhomElement = beam.ToBhOM(k3dModel, bhomModel);

                if(bhomElement is Bar bhomBar)
                {
                    bhomModel.Elements1D[beam.ind] = bhomBar;
                }
            }

            // Convert loads
            bhomModel.Loads = k3dModel.GetLoads().SelectMany(g => g.ToBhOM(k3dModel, bhomModel)).ToList();
            
            // Convert supports
            k3dModel.supports.ForEach(s => s.ToBhOM(k3dModel, bhomModel));

            // TODO Is it possible to return splitted elements?
            return bhomModel.ReturnBhOMEntities();
        }
    }
}