using System;
using System.Collections.Generic;
using Karamba.CrossSections;
using Karamba.Utilities;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using BH.Engine.Adapters.Karamba3D;
using BH.oM.Structure.SectionProperties;

namespace Karamba3D_ToolkitTests
{
    public class CroSecTests : BaseTest
    {
        private CroSec  CreateCrossSectionToTest()
        {
            var material = MaterialTests.CreateMaterialToTest();
            return new CroSec_Box("RandomFamily", "RandomName", "RandomCountry", null, material);
        }

        [Test]
        public void CrossSections_WithSameGuids_WillBeInstancedOnce_Test()
        {
            // Arrange
            var crossSection = CreateCrossSectionToTest();
            crossSection.AddElemId(string.Empty); // the empty string means it will apply to all beams.
            var model = TestUtilities.Create3NotEqualLengthHingesBeam(crossSection);

            // Act
            var bhomModel = model.ToBhOM();
            var bhomMaterial = bhomModel.CrossSections.Single();

            // Assert
            Assert.AreEqual(bhomMaterial.BHoM_Guid, crossSection.guid);
        }
    }
}