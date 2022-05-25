using System.Linq;
using BH.Engine.Adapters.Karamba3D;
using Karamba.CrossSections;
using NUnit.Framework;

namespace Karamba3D_Tests
{
    public static class CrossSectionTests
    {
        [Test]
        public static void HEconversion()
        {
            BH.Engine.Base.Compute.LoadAllAssemblies();

            // Assert
            var croSec = new CroSec_I()
            {
                name = "HEA100",
                family = "HE"
            };

            // Act
            var test = Query.ReadDataSets();
            var pippo = test.ToList();
            var bhomSection = croSec.ToBHoM();
        }
    }
}