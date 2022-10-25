namespace BH.Engine.Adapters.Karamba3D
{
    using Karamba.Elements;
    using Karamba.Loads;
    using Karamba.Models;
    using oM.Base;
    using oM.Structure.Elements;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

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

        private static BH.oM.Karamba3D.FemModel ToFemModel(this BhOMModel bhomModel)
        {
            return new oM.Karamba3D.FemModel()
            {
                Nodes = bhomModel.Nodes.Values.ToList(),
                Bars = bhomModel.Elements1D.Values.ToList(),
                Loads = bhomModel.Loads.ToList(),
                LoadCases = bhomModel.LoadCases.ToList(),
            };
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
                var bhomElement = beam.IToBhOM(k3dModel, bhomModel);

                if(bhomElement is Bar bhomBar)
                {
                    bhomModel.Elements1D[beam.ind] = bhomBar;
                }
            }

            // Convert loads
            bhomModel.Loads.AddRange(k3dModel.GetLoads().SelectMany(g => g.IToBhOM(k3dModel, bhomModel)));
            
            // Convert supports and register to corresponding nodes
            k3dModel.supports.ForEach(s => s.ToBhOM(k3dModel, bhomModel));

            return bhomModel;
        }

        public static oM.Karamba3D.FemModel ToBhOM(this Karamba.Models.Model k3dModel)
        {
            return k3dModel.ToBhOMModel().ToFemModel();
        }
    }
}