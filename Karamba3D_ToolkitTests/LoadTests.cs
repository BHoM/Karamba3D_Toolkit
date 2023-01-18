//namespace Karamba3D_ToolkitTests
//{
//    using System;
//    using BH.Engine.Adapter.Karamba3D;
//    using BH.Engine.Adapters.Karamba3D;
//    using BH.oM.Base;
//    using BH.oM.Geometry;
//    using BH.oM.Structure.Elements;
//    using BH.oM.Structure.Loads;
//    using Karamba.Geometry;
//    using Karamba.Loads;
//    using Karamba.Loads.Beam;
//    using Karamba3D_Engine;
//    using NUnit.Framework;
//    using System.Collections.Generic;
//    using System.Linq;
//    using BH.Engine.Geometry;

//    [TestFixture]
//    public class LoadTests : BaseTest
//    {
//        [Test]
//        public void GetOrientation_Test()
//        {
//            // Arrange
//            var model = TestUtilities.CreateHingedBeamModel();

//            var beamIds = model.elems.Select(b => b.id).ToList();
//            string loadCase = "TestBhOMLoadCase";
//            double position = 0.5;
//            ElementLoad[] loads =
//            {
//                // Global load
//                new ConcentratedForce(beamIds, null, loadCase, position, Vector3.UnitZ, LoadOrientation.global),

//                // Local load
//                new ConcentratedForce(beamIds, null, loadCase, position, Vector3.UnitZ, LoadOrientation.local),

//                // Projected load
//                new ConcentratedForce(beamIds, null, loadCase, position, Vector3.UnitZ, LoadOrientation.proj)
//            };
//            model.eloads.AddRange(loads);

//            // Act
//            var bhomModel = model.ToBhomModel();
//            var bhomLoads = bhomModel.Loads.OfType<IElementLoad<Bar>>().ToList();

//            // Arrange
//            Assert.AreEqual(bhomLoads.Count(), 3);
//        }

//        [TestCase(LoadOrientation.global, true)]
//        [TestCase(LoadOrientation.local, true)]
//        [TestCase(LoadOrientation.proj, true)]
//        [TestCase(LoadOrientation.global, false)]
//        [TestCase(LoadOrientation.local, false)]
//        [TestCase(LoadOrientation.proj, false)]
//        public void ConcentratedLoad_ConversionTest(LoadOrientation loadOrientation, bool isForce)
//        {
//            // In K3D the load is assigned with a parametric position, while bhom counterpart are in real distances.
//            // It is expected that the single karamba loads will be converted into 2 bhom loads.

//            // Arrange
//            var vector = new Vector3(1, 2, 3);
//            double position = 0.5;
//            string loadCase = "TestLoadCase";
//            var beamIds = new[] { string.Empty };
//            var load = isForce ?
//                (Load)new ConcentratedForce(beamIds, null, loadCase, position, vector, loadOrientation) :
//                new ConcentratedMoment(beamIds, null, loadCase, position, vector, loadOrientation);

//            var model = TestUtilities.Create3NotEqualLengthHingesBeam(load);

//            // Act
//            var bhomModel = model.ToBhOM();
//            var bhomBars = bhomModel.Bars.ToList();
//            var bhomLoads = bhomModel.Loads.Cast<BarPointLoad>().ToList();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            LoadAxis loadAxis = loadOrientation == LoadOrientation.local ? LoadAxis.Local : LoadAxis.Global;
//            bool projected = loadOrientation == LoadOrientation.proj;
//            var expectedForce = isForce ? vector.ToBhOM() : new Vector();
//            var expectedMoment = isForce ? new Vector() : vector.ToBhOM();
//            var expectedFirstLoad = new BarPointLoad
//            {
//                Axis = loadAxis,
//                DistanceFromA = 4, // This is exactly at the mid of the first and third bar.
//                Force = expectedForce,
//                Moment = expectedMoment,
//                Loadcase = bhomLoadCase,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBars[0], bhomBars[2] } },
//                Projected = projected,
//            };
//            var expectedSecondLoad = new BarPointLoad
//            {
//                Axis = loadAxis,
//                DistanceFromA = 8, // This is exactly at the mid of the second bar.
//                Force = expectedForce,
//                Moment = expectedMoment,
//                Loadcase = bhomLoadCase,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBars[1] } },
//                Projected = projected,
//            };

//            // The values are as expected
//            CustomAssert.BhOMObjectsAreEqual(bhomLoads[0], expectedFirstLoad);
//            CustomAssert.BhOMObjectsAreEqual(bhomLoads[1], expectedSecondLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoads[0].Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);
//            Assert.AreEqual(bhomLoads[1].Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoads[0].Objects.Elements[0].BHoM_Guid, bhomBars[0].BHoM_Guid);
//            Assert.AreEqual(bhomLoads[0].Objects.Elements[1].BHoM_Guid, bhomBars[2].BHoM_Guid);
//            Assert.AreEqual(bhomLoads[1].Objects.Elements[0].BHoM_Guid, bhomBars[1].BHoM_Guid);
//        }

//        [TestCase(LoadOrientation.global, true)]
//        [TestCase(LoadOrientation.local, true)]
//        [TestCase(LoadOrientation.proj, true)]
//        [TestCase(LoadOrientation.local, false)]
//        public void DistributedLoad_WhenEqualValuesAtNodesAreGiven_ConvertToUniformLoadTest_Test(
//            LoadOrientation loadOrientation,
//            bool isForce)
//        {
//            // Arrange
//            var beamIds = new[] { "beam0", "beam2" };
//            string loadCase = "TestLoadCase";
//            var direction = isForce ? new Vector3(1, 2, 3) : Vector3.UnitX;
//            double value = 2.0;
//            var load = isForce ?
//                (DistributedLoad)new DistributedForce(
//                    beamIds,
//                    null,
//                    loadCase,
//                    loadOrientation,
//                    direction,
//                    new[] { 0.0, 1.0 },
//                    Enumerable.Repeat(value, 2)) :
//                new DistributedMoment(
//                    beamIds,
//                    null,
//                    loadCase,
//                    loadOrientation,
//                    direction,
//                    new[] { 0.0, 1.0 },
//                    Enumerable.Repeat(value, 2));
//            var model = TestUtilities.Create3NotEqualLengthHingesBeam(load);

//            // Act
//            var bhomModel = model.ToBhOM();
//            var bhomBars = bhomModel.Bars;
//            var bhomLoad = bhomModel.Loads.Cast<BarUniformlyDistributedLoad>().Single();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            var loadAxis = loadOrientation == LoadOrientation.local ? LoadAxis.Local : LoadAxis.Global;
//            bool projected = loadOrientation == LoadOrientation.proj;
//            var expectedVector = (direction.Unitized * value).ToBhOM();
//            var expectedLoad = new BarUniformlyDistributedLoad
//            {
//                Force = isForce ? expectedVector : new Vector(),
//                Moment = !isForce ? expectedVector : new Vector(),
//                Loadcase = bhomLoadCase,
//                Axis = loadAxis,
//                Projected = projected,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBars[0], bhomBars[2] } }
//            };

//            // The values are as expected
//            CustomAssert.BhOMObjectsAreEqual(bhomLoad, expectedLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoad.Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoad.Objects.Elements[0].BHoM_Guid, bhomBars[0].BHoM_Guid);
//            Assert.AreEqual(bhomLoad.Objects.Elements[1].BHoM_Guid, bhomBars[2].BHoM_Guid);
//        }

//        [TestCase(LoadOrientation.global, true)]
//        public void DistributedLoad_WhenMoreThen2ValuesAreGiven_ConvertToASetOfVaryingLoads_Test(
//            LoadOrientation loadOrientation,
//            bool isForce)
//        {
//            // One K3D distributed load can handle more then two values.
//            // Therefore it will be converted into several bhom distributed loads.

//            // Arrange
//            var beamIds = new[] { "beam0", "beam2" };
//            string loadCase = "TestLoadCase";
//            var direction = isForce ? new Vector3(1, 2, 3) : Vector3.UnitX;
//            var positions = new[] { 0.2, 0.5, 0.8 };
//            var values = new[] { 0.3, 1, 0.7 };
//            var load = isForce ?
//                (DistributedLoad)new DistributedForce(
//                    beamIds,
//                    null,
//                    loadCase,
//                    loadOrientation,
//                    direction,
//                    positions,
//                    values) :
//                new DistributedMoment(
//                    beamIds,
//                    null,
//                    loadCase,
//                    loadOrientation,
//                    direction,
//                    positions,
//                    values);
//            var model = TestUtilities.Create3NotEqualLengthHingesBeam(load);

//            // Act// Act
//            var bhomModel = model.ToBhOM();
//            var bhomBars = bhomModel.Bars;
//            var bhomLoads = bhomModel.Loads.Cast<BarVaryingDistributedLoad>().ToList();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            var loadAxis = loadOrientation == LoadOrientation.local ? LoadAxis.Local : LoadAxis.Global;
//            bool projected = loadOrientation == LoadOrientation.proj;
//            var expectedVectors = values.Select(v => (v * direction.Unitized).ToBhOM()).ToArray();
//            var expectedLoad1 = new BarVaryingDistributedLoad()
//            {
//                StartPosition = positions[0],
//                EndPosition = positions[1],
//                ForceAtStart = isForce ? expectedVectors[0] : new Vector(),
//                ForceAtEnd = isForce ? expectedVectors[1] : new Vector(),
//                MomentAtStart = !isForce ? expectedVectors[0] : new Vector(),
//                MomentAtEnd = !isForce ? expectedVectors[1] : new Vector(),
//                Loadcase = bhomLoadCase,
//                Axis = loadAxis,
//                Projected = projected,
//                RelativePositions = true,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBars[0], bhomBars[2] } }
//            };

//            var expectedLoad2 = new BarVaryingDistributedLoad()
//            {
//                StartPosition = positions[1],
//                EndPosition = positions[2],
//                ForceAtStart = isForce ? expectedVectors[1] : new Vector(),
//                ForceAtEnd = isForce ? expectedVectors[2] : new Vector(),
//                MomentAtStart = !isForce ? expectedVectors[1] : new Vector(),
//                MomentAtEnd = !isForce ? expectedVectors[2] : new Vector(),
//                Loadcase = bhomLoadCase,
//                Axis = loadAxis,
//                Projected = projected,
//                RelativePositions = true,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBars[0], bhomBars[2] } }
//            };

//            // The values are as expected
//            Assert.AreEqual(bhomLoads.Count, 2);
//            CustomAssert.BhOMObjectsAreEqual(bhomLoads[0], expectedLoad1);
//            CustomAssert.BhOMObjectsAreEqual(bhomLoads[1], expectedLoad2);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoads[0].Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);
//            Assert.AreEqual(bhomLoads[1].Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoads[0].Objects.Elements[0].BHoM_Guid, bhomBars[0].BHoM_Guid);
//            Assert.AreEqual(bhomLoads[0].Objects.Elements[1].BHoM_Guid, bhomBars[2].BHoM_Guid);
//            Assert.AreEqual(bhomLoads[1].Objects.Elements[0].BHoM_Guid, bhomBars[0].BHoM_Guid);
//            Assert.AreEqual(bhomLoads[1].Objects.Elements[1].BHoM_Guid, bhomBars[2].BHoM_Guid);
//        }

//        [TestCase(LoadOrientation.global)]
//        [TestCase(LoadOrientation.local)]
//        [TestCase(LoadOrientation.proj)]
//        public void UniformLoadOld_ConversionTest(LoadOrientation loadOrientation)
//        {
//            // Arrange
//            var vector = new Vector3(1, 2, 3);
//            var load = new UniformlyDistLoad_OLD(new[] { string.Empty }, null, vector, loadOrientation, "TestLoadCase");
//            var model = TestUtilities.CreateFixedFixedBeam(load);

//            // Act
//            var bhomModel = model.ToBhOM();
//            var bhomBar = bhomModel.Bars.Single();
//            var bhomLoad = bhomModel.Loads.Cast<BarUniformlyDistributedLoad>().Single();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            // The values are as expected
//            LoadAxis loadAxis = loadOrientation == LoadOrientation.local ? LoadAxis.Local : LoadAxis.Global;
//            bool projected = loadOrientation == LoadOrientation.proj;
//            var expectedLoad = new BarUniformlyDistributedLoad
//            {
//                Force = vector.ToBhOM(),
//                Loadcase = bhomLoadCase,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBar } },
//                Axis = loadAxis,
//                Projected = projected,
//            };
//            CustomAssert.BhOMObjectsAreEqual(bhomLoad, expectedLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoad.Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoad.Objects.Elements.Single().BHoM_Guid, bhomBar.BHoM_Guid);
//        }

//        [Test]
//        public void StrainLoad_ConversionTest()
//        {
//            // Arrange
//            var load = new StrainLoad(
//                new[] { string.Empty },
//                null,
//                new Vector3(1, 2, 3),
//                new Vector3(4, 5, 6),
//                "TestLoadCase");
//            var k3dModel = TestUtilities.CreateFixedFixedBeam(load);

//            // Act
//            k3dModel.ToBhOM();

//            // Assert
//            string expectedMessage = string.Format(Resource.WarningNotYetSupportedType, load.GetType().Name);
//            StringAssert.Contains(expectedMessage, K3dLogger.GetWarnings().Single());
//        }

//        [Test]
//        public void TemperatureLoad_ConversionTest()
//        {
//            // Arrange
//            var load = new TemperatureLoad(new[] { string.Empty }, null, 0.123, Vector3.Unset, "TestLoadCase");
//            var k3dModel = TestUtilities.CreateFixedFixedBeam(load);

//            // Act
//            var bhomModel = k3dModel.ToBhOM();
//            var bhomBar = bhomModel.Bars.Single();
//            var bhomLoad = bhomModel.Loads.Cast<BarUniformTemperatureLoad>().Single();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            var expectedLoad = new BarUniformTemperatureLoad
//            {
//                TemperatureChange = 0.123,
//                Loadcase = bhomLoadCase,
//                Objects = new BHoMGroup<Bar> { Elements = new List<Bar> { bhomBar } },
//            };
//            CustomAssert.BhOMObjectsAreEqual(bhomLoad, expectedLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoad.Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoad.Objects.Elements.Single().BHoM_Guid, bhomBar.BHoM_Guid);
//        }

//        [Test]
//        public void TemperatureLoad_WhenHasLinearChanges_ThrownAWarning_Test()
//        {
//            // Arrange
//            var load = new TemperatureLoad(new[] { string.Empty }, null, 0.123, new Vector3(4, 5, 6), "TestLoadCase");
//            var k3dModel = TestUtilities.CreateFixedFixedBeam(load);

//            // Act
//            k3dModel.ToBhOM();

//            // Assert
//            string expectedMessage = Resource.WarningLinearTemperatureChangesNotSupported;
//            StringAssert.Contains(expectedMessage, K3dLogger.GetWarnings().Single());
//        }

//        [Test]
//        public void GravityLoad_ConversionTest()
//        {
//            // Arrange
//            var vector = new Vector3(1, 2, 3);
//            var load = new Karamba.Loads.GravityLoad(vector, "TestLoadCase");
//            var k3dModel = TestUtilities.Create3NotEqualLengthHingesBeam(load);

//            // Act
//            var bhomModel = k3dModel.ToBhOM();
//            var bhomBars = bhomModel.Bars;
//            var bhomLoad = bhomModel.Loads.Cast<BH.oM.Structure.Loads.GravityLoad>().Single();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            // The values are as expected
//            var expectedLoad = new BH.oM.Structure.Loads.GravityLoad
//            {
//                Axis = LoadAxis.Global,
//                GravityDirection = vector.ToBhOM(),
//                Projected = false,
//                Loadcase = bhomLoadCase,
//                Objects = new BHoMGroup<BHoMObject> { Elements = bhomBars.Cast<BHoMObject>().ToList() }
//            };
//            CustomAssert.BhOMObjectsAreEqual(bhomLoad, expectedLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoad.Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            for (int i = 0; i < bhomBars.Count; i++)
//            {
//                Assert.AreEqual(bhomLoad.Objects.Elements[i].BHoM_Guid, bhomBars[i].BHoM_Guid);
//            }
//        }

//        [Test]
//        public void PointLoad_ConversionTest()
//        {
//            // Arrange
//            var force = new Vector3(1, 2, 3);
//            var moment = new Vector3(4, 5, 6);
//            var load = new Karamba.Loads.PointLoad(1, force, moment, false);
//            var k3dModel = TestUtilities.CreateFixedFreeBeam(load);

//            // Act
//            var bhomModel = k3dModel.ToBhOM();
//            var bhomNode = bhomModel.Nodes[1];
//            var bhomLoad = bhomModel.Loads.Cast<BH.oM.Structure.Loads.PointLoad>().Single();
//            var bhomLoadCase = bhomModel.LoadCases.Single();

//            // Assert
//            var expectedLoad = new BH.oM.Structure.Loads.PointLoad()
//            {
//                Force = force.ToBhOM(),
//                Moment = moment.ToBhOM(),
//                Axis = LoadAxis.Global,
//                Projected = false,
//                Objects = new BHoMGroup<Node> { Elements = new List<Node> { bhomNode } },
//                Loadcase = bhomLoadCase,
//            };
//            CustomAssert.BhOMObjectsAreEqual(bhomLoad, expectedLoad);

//            // The load case are the same instances
//            Assert.AreEqual(bhomLoad.Loadcase.BHoM_Guid, bhomLoadCase.BHoM_Guid);

//            // The bars are the same instances
//            Assert.AreEqual(bhomLoad.Objects.Elements.Single().BHoM_Guid, bhomNode.BHoM_Guid);
//        }

//        [Test]
//        public void PointLoad_WhenWithLocalRotation_ThrownAWarning_Test()
//        {
//            // Arrange
//            var force = new Vector3(1, 2, 3);
//            var moment = new Vector3(4, 5, 6);
//            var load = new Karamba.Loads.PointLoad(1, force, moment, true);
//            var k3dModel = TestUtilities.CreateFixedFreeBeam(load);

//            // Act
//            k3dModel.ToBhOM();

//            // Assert
//            string expectedMessage = Resource.WarningPointLoadLocalLoadNotSupported;
//            StringAssert.Contains(expectedMessage, K3dLogger.GetWarnings().Single());
//        }

//        [Test]
//        public void PointMass_ConversionTest()
//        {
//            // Arrange
//            var load = new PointMass(1, 123, 456);
//            var k3dModel = TestUtilities.CreateFixedFreeBeam(load);

//            // Act
//            k3dModel.ToBhOM();

//            // Assert
//            string expectedMessage = string.Format(Resource.WarningNotYetSupportedType, load.GetType().Name);
//            StringAssert.Contains(expectedMessage, K3dLogger.GetWarnings().Single());
//        }
//    }
//}