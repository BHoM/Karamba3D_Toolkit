namespace BH.Engine.Adapters.Karamba3D
{
    using System.Collections.Generic;
    using System.Linq;
    using Karamba.Loads;
    using oM.Base;
    using oM.Structure.Elements;
    using oM.Structure.Loads;

    public class BhOMModel
    {
        private readonly IDictionary<string, Loadcase> _loadCases = new Dictionary<string, Loadcase>();

        public IDictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();

        // TODO Review this part when will pass to 2d elements.
        public IDictionary<int, Bar> Elements1D { get; set; } = new Dictionary<int, Bar>();

        public IEnumerable<ILoad> Loads { get; set; } = Enumerable.Empty<ILoad>();

        public BhOMModel()
        {
        }

        public BhOMModel(int nodesCount, int elementsCount)
        {
            Nodes = new Dictionary<int, Node>(nodesCount);
            Elements1D = new Dictionary<int, Bar>(elementsCount);
        }

        public Loadcase RegisterLoadCase(string loadCaseName)
        {
            if (_loadCases.TryGetValue(loadCaseName, out var loadCase))
            {
                return loadCase;
            }

            loadCase = new Loadcase
            {
                Name = loadCaseName,
                Nature = LoadNature.Other,
                Number = _loadCases.Count
            };
            _loadCases.Add(loadCaseName, loadCase);

            return loadCase;
        }

        public IEnumerable<IBHoMObject> ReturnBhOMEntities()
        {
            return Elements1D.Values.Cast<IBHoMObject>()
                        .Concat(Nodes.Values)
                        .Concat(Loads);
        }
    }
}