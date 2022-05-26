using BH.Engine.Adapters.Karamba3D;
using BH.oM.Base;
using BH.oM.Data.Library;
using BH.oM.Structure.SectionProperties;
using Karamba.CrossSections;
using Karamba.Materials;
using KarambaCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Tests.Karamba3D
{
    [TestClass]
    public class CrossSectionConversion
    {
        [TestMethod]
        public void GetDatasetData()
        {
            List<SteelSection> data = Compute.GetDatasetData<SteelSection>(@"EU_SteelSections\HE", true);
        }

        [TestMethod]
        [DataRow("HEA100", "HE")]
        [DataRow("HEA200", "HE")]
        [DataRow("HEA300", "HE")]
        [DataRow("HEA400", "HE")]
        public void CroSec_I_Steel(string sectionName, string sectionFamily)
        {
            //string sectionName = "HEA100";
            //string sectionFamily = "HE";
            string sectionSubtype = sectionName.Replace(Regex.Match(sectionName, @"\d+").Value, "").Replace(sectionFamily, "");

            List<SteelSection> allBHoMSectionOfFamily = Compute.GetDatasetData<SteelSection>($@"EU_SteelSections\{sectionFamily}", true);
            List<SteelSection> relevantBhomSections = allBHoMSectionOfFamily.Where(d => 
                    d.Name.Contains(sectionFamily) && 
                    d.Name.CrossSectionNumber() == sectionName.CrossSectionNumber() &&
                    d.Name.Replace(d.Name.CrossSectionNumber().ToString(), "").Replace(sectionFamily, "").Trim() == sectionSubtype)
                .ToList();

            Assert.IsTrue(relevantBhomSections.Count() == 1, $"More than one corresponding BHoM section found for the section name `{sectionName}`.");

            SteelSection bhomSection = relevantBhomSections.First();

            var croSec = new CroSec_I()
            {
                name = sectionName,
                family = sectionFamily
            };

            var convertedSection = croSec.ToBHoM();

            Assert.AreEqual(bhomSection.Name, convertedSection.Name);
        }


        [TestMethod]
        public void Concreteconversion()
        {
            CroSec_Trapezoid trapezoidSection = new CroSec_Trapezoid("", "", "", null, new FemMaterial_Isotrop("concrete", "", 1, 1, 1, 1, 1, 1, FemMaterial.FlowHypothesis.mises, 1, null), 600, 300, 300);
            //var concreteCrossSection = new CroSec_I("", "", "", null, new FemMaterial_Isotrop("concrete", "", 1, 1, 1, 1, 1, 1, FemMaterial.FlowHypothesis.mises, 1, null));

            //var asd = new Toolkit();
            //var gna = asd.Material.IsotropicMaterial();

            var bhomSection = trapezoidSection.ToBHoM();
        }
    }
}
