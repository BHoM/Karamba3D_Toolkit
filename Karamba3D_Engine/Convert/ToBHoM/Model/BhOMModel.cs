namespace BH.Engine.Adapters.Karamba3D
{
    using System;
    using oM.Structure.Elements;
    using oM.Structure.Loads;
    using System.Collections.Generic;
    using System.Linq;
    using oM.Structure.MaterialFragments;
    using oM.Structure.SectionProperties;

    internal class BhOMModel
    {
        private readonly IDictionary<string, Loadcase> _loadCases = new Dictionary<string, Loadcase>();

        public IDictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();

        public IDictionary<int, Bar> Elements1D { get; set; } = new Dictionary<int, Bar>();

        public List<ILoad> Loads { get; set; } = new List<ILoad>();

        public ICollection<Loadcase> LoadCases => _loadCases.Values;

        public IDictionary<Guid, IMaterialFragment> Materials { get; set; } = new Dictionary<Guid, IMaterialFragment>();

        public IDictionary<Guid, ISectionProperty> CrossSections { get; set; } = new Dictionary<Guid, ISectionProperty>();

        public BhOMModel()
        {
        }

        public bool TryGetLoadCase(string loadCaseName, out Loadcase loadcase)
        {
            return _loadCases.TryGetValue(loadCaseName, out loadcase);
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
                Number = _loadCases.Count + 1
            };
            _loadCases.Add(loadCaseName, loadCase);

            return loadCase;
        }
    }
}