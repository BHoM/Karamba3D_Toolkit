namespace Karamba3D_ToolkitTests
{
    using BH.Engine.Adapters.Karamba3D;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Loads.Beam;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture] 
    public class LoadCaseTests
    {
        [Test]
        public void LoadCases_WhenMultiple_AreStoredAsExpected_Test()
        {
            //// Arrange
            //var loads = new Load[]
            //{
            //    new PointLoad(1, new Vector3(1, 2, 3), new Vector3(4, 5, 6), "LoadCase1", false),
            //    new ConcentratedForce(
            //        new[] { string.Empty },
            //        null,
            //        "LoadCase2",
            //        0.6,
            //        new Vector3(7, 8, 9),
            //        LoadOrientation.global),
            //    new ConcentratedMoment(
            //        new[] { string.Empty },
            //        null,
            //        "LoadCase1",
            //        0.6,
            //        new Vector3(10, 11, 12),
            //        LoadOrientation.global)
            //};
            //var model = TestUtilities.CreateFixedFreeBeam(loads);

            //// Act
            //var bhomModel = model.ToBHoM();
            //var loadCases = bhomModel.LoadCases;

            //// Assert
            //var expectedNames = new[] { "LoadCase1", "LoadCase2" };
            //var expectedNumbers = new[] { 0, 1 };
            //CollectionAssert.AreEqual(expectedNames, loadCases.Select(l => l.Name).ToArray());
            //CollectionAssert.AreEqual(expectedNumbers, loadCases.Select(l => l.Number).ToArray());
            Assert.Fail();
        }
    }
}