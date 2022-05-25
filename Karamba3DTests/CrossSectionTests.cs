using BH.Engine.Adapters.Karamba3D;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Structure.SectionProperties;
using Karamba.CrossSections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Karamba3DTests
{
    [TestClass]
    public class CrossSectionTests
    {
        [TestMethod]
        public void GetDatasetData()
        {
            List<SteelSection> data = Compute.GetDatasetData<SteelSection>(@"EU_SteelSections\HE", true);
        }

        [TestMethod]
        public void HEconversion()
        {
            var croSec = new CroSec_I()
            {
                name = "HEA100",
                family = "HE"
            };

            var bhomSection = croSec.ToBHoM();
        }

    }
}
