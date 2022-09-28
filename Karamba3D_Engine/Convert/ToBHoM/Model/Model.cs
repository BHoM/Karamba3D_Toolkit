namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
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

        private static IEnumerable<IBHoMObject> ReturnBhOMObjects(this BhOMModel bhomModel)
        {
            return bhomModel.Elements1D.Values.Cast<IBHoMObject>()
                             .Concat(bhomModel.Nodes.Values)
                             .Concat(bhomModel.Loads);
        }

        internal static BhOMModel ToBhOMModel(this Karamba.Models.Model k3dModel)
        {
            var bhomModel = new BhOMModel()
            {
                Nodes = new Dictionary<int, Node>(k3dModel.nodes.Count),
                Elements1D = new Dictionary<int, Bar>(k3dModel.elems.Count)
            };

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
            bhomModel.Loads = k3dModel.GetLoads().SelectMany(g => g.ToBhOM(k3dModel, bhomModel));
            
            // Convert supports and register to corresponding nodes
            k3dModel.supports.ForEach(s => s.ToBhOM(k3dModel, bhomModel));

            return bhomModel;
        }

        public static IEnumerable<IBHoMObject> ToBhOM(this Karamba.Models.Model k3dModel)
        {
            return k3dModel.ToBhOMModel().ReturnBhOMObjects();
        }
    }
}