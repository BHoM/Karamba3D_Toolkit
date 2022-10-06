namespace Karamba3D_ToolkitTests
{
    using System.Linq;
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Structure.Elements;
    using BH.oM.Structure.Loads;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using Karamba.Models;
    using NUnit.Framework;

    // Test all type of load
        // Concentrated force
        // Concentrated moment
        // Distributed force uniform
        // Distributed moment uniform
        // Distributed force non uniform
        // Distributed moment non uniform
        // Uniform load old
        // Strain load
        // Temperature load
        // Gravity load
        // PointLoad
        // Point mass

    // Test load case assign for the bhom Model

    [TestFixture]
    public class LoadTests
    {
        [Test]
        public void GetOrientation_Test()
        {
            // Arrange
            string loadCase = "TestBhOMLoadCase";
            double position = 0.5;
            ElementLoad[] loads =
            {
                // Global load
                new ConcentratedForce(null, null, loadCase, position, Vector3.UnitZ, LoadOrientation.global),

                // Local load
                new ConcentratedForce(null, null, loadCase, position, Vector3.UnitZ, LoadOrientation.local),

                // Projected load
                new ConcentratedForce(null, null, loadCase, position, Vector3.UnitZ, LoadOrientation.proj)
            };
            var model = new Model();
            model.eloads.AddRange(loads);

            // Act
            var bhomLoads = model.ToBhOMModel().Loads.OfType<IElementLoad<Bar>>().ToList();

            // Arrange
            Assert.AreEqual(bhomLoads.Count(), 3);
        }

        [Test]
        public void ConcentratedForce_ConversionTest()
        {
        }

        [Test]
        public void ConcentratedMoment_ConversionTest()
        {
        }

        [Test]
        public void DistributedForce_ConversionTest()
        {
        }

        [Test]
        public void DistributedMoment_ConversionTest()
        {
        }

        [Test]
        public void UniformLoadOld_ConversionTest()
        {
        }

        [Test]
        public void StrainLoad_ConversionTest()
        {
        }

        [Test]
        public void TemperatureLoad_ConversionTest()
        {
        }

        [Test]
        public void GravityLoad_ConversionTest()
        {
        }

        [Test]
        public void PointLoad_ConversionTest()
        {
        }

        [Test]
        public void PointMass_ConversionTest()
        {
        }
    }
}