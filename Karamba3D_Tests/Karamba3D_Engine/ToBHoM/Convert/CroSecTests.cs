using System.Linq;
using BH.Engine.Adapters.Karamba3D;
using Karamba.CrossSections;

namespace Karamba3D_Tests
{
    using NUnit.Framework;

    public class CroSecTests
    {
        [Test]
        public void HEconversion()
        {
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