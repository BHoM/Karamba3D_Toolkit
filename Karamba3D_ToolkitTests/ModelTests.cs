namespace Karamba3D_ToolkitTests
{
    using System;
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Base;
    using BH.oM.Base.Debugging;
    using BH.oM.Geometry;
    using BH.oM.Structure.Constraints;
    using BH.oM.Structure.Elements;
    using BH.oM.Structure.Loads;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Models;
    using Karamba.Supports;
    using Karamba3D_Engine;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using BH.oM.Structure.Offsets;
    using Node = Karamba.Nodes.Node;

    // Test all type of load
    // Test load case assign for the bhom Model
    // Test support
    // Test support with local coosys
    // Test support with prescribed displacement
    // Test beam
        // Test orientation angle for all possible angles
        // Test beam joints as releases

    // Test truss
    // Test eccentricity on beam
    // Test releases on beam, truss
    // Test orientation of local coosys for beam
    // Ask for an implementation of sprin as non rigid link

    [TestFixture]
    public class ModelTests
    {
        public static Model CreateHingedBeamModel(Point3 start = default, Point3 end = default, double orientationAngle = 0.0)
        {
            start = start == default ? Point3.Zero : start;
            end = end == default ? new Point3(1, 0, 0) : end;

            Node[] nodes = { new Node(0, start), new Node(1, start) };
            Support[] supports =
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, false }, Plane3.WorldXY)
            };

            var beamBuilder = new BuilderBeam(start, end);
            if (orientationAngle != 0)
            {
                var writer = beamBuilder.Ori.Writer;
                writer.Alpha = orientationAngle;
                beamBuilder.Ori = writer.Reader;
            }
            var beam = new ModelBeam(0, beamBuilder, nodes.Select(n => n.ind).ToList(), nodes.ToList());

            var model = new Model();
            model.nodes.AddRange(nodes);
            model.supports.AddRange(supports);
            model.elems.Add(beam);
            model.crosecs.Add(beam.crosec);

            return model;
        }

        public static Model CreateHingedTrussModel()
        {
            Node[] nodes = { new Node(0, Point3.Zero), new Node(1, new Point3(1, 0, 0)) };
            Support[] supports =
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, false }, Plane3.WorldXY)
            };

            var truss = new ModelTruss(0, new BuilderBeam(), nodes.Select(n => n.ind).ToList(), nodes.ToList());

            var model = new Model();
            model.nodes.AddRange(nodes);
            model.supports.AddRange(supports);
            model.elems.Add(truss);

            return model;
        }

        public Model CreateKingPostTruss()
        {
            

            var nodes = new List<Node>
            {
                new Node(0, new Point3(0, 0.5, 0)),
                new Node(1, new Point3(0, 0, 0)),
                new Node(2, new Point3(1, 0, 0)),
                new Node(3, new Point3(0.5, 0, 0))
            };

            Support[] supports =
            {
                new Support(0, new[] { true, true, true, false, false, false }, Plane3.Default),
                new Support(1, new[] { false, false, false, false, false, false }, Plane3.Default)
            };

            var nodeIndices = nodes.Select(n => n.ind).ToList();
            List<ModelElement> trusses = new List<ModelElement>
            {
                new ModelTruss(0, new BuilderBeam(), new[] { nodeIndices[0], nodeIndices[1] }, new List<Node> { nodes[0], nodes[1] }),
                new ModelTruss(1, new BuilderBeam(), new[] { nodeIndices[1], nodeIndices[3] }, new List<Node> { nodes[1], nodes[3] }),
                new ModelTruss(2, new BuilderBeam(), new[] { nodeIndices[3], nodeIndices[2] }, new List<Node> { nodes[3], nodes[2] }),
                new ModelTruss(3, new BuilderBeam(), new[] { nodeIndices[2], nodeIndices[0] }, new List<Node> { nodes[2], nodes[0] }),
                new ModelTruss(4, new BuilderBeam(), new[] { nodeIndices[0], nodeIndices[3] }, new List<Node> { nodes[0], nodes[3] })
            };

            var model = new Model();
            model.nodes.AddRange(nodes);
            model.supports.AddRange(supports);
            model.elems.AddRange(trusses);

            return model;
        }

        [Test]
        public void Node_ConversionTest()
        {
            // Arrange
            var node = new Node(123, new Point3(1, 2, 3));
            var model = new Model();
            model.nodes.Add(node);

            // Act
            var bhomModel = model.ToBhOMModel();

            // Assert
            var expectedNode = new BH.oM.Structure.Elements.Node()
            {
                Name = "123",
                Position = new Point
                {
                    X = 1,
                    Y = 2,
                    Z = 3
                }
            };
            CustomAssert.BhOMObjectsAreEqual(bhomModel.Nodes[123], expectedNode);
        }

        [Test]
        public void Node_WithSupport_ConversionTest()
        {
            // Arrange
            var node = new Node(345, new Point3(4, 5, 6));
            var localCoordinateSystem = new Plane3(Point3.Zero, Vector3.UnitX);
            var support = new Support(345, new[] { true, false, true, false, true, false }, localCoordinateSystem);

            var model = new Model();
            model.supports.Add(support);
            model.nodes.Add(node);

            // Act
            var bhomModel = model.ToBhOMModel();

            // Assert
            var expectedNode = new BH.oM.Structure.Elements.Node()
            {
                Name = "345",
                Position = new Point
                {
                    X = 4,
                    Y = 5,
                    Z = 6
                },
                Support = new Constraint6DOF
                {
                    TranslationX = DOFType.Fixed,
                    TranslationY = DOFType.Free,
                    TranslationZ = DOFType.Fixed,
                    RotationX = DOFType.Free,
                    RotationY = DOFType.Fixed,
                    RotationZ = DOFType.Free
                },
                Orientation = Basis.YZ,
            };
            CustomAssert.BhOMObjectsAreEqual(bhomModel.Nodes[345], expectedNode);
        }

        [Test]
        public void Node_WithSupportAndPrescribedDisplacement_ConversionTest()
        {
            // Arrange
            var node = new Node(123, new Point3(4, 5, 6));
            var prescribedTranslations = new Vector3(11, 22, 33);
            var prescribedRotations = new Vector3(44, 55, 66);
            var loadCaseName = "PrescribedDisplacementLoadCase";
            var support = new Support(
                node.ind,
                new List<bool> { true, false, true, false, true, false },
                Plane3.Default,
                prescribedTranslations,
                prescribedRotations,
                loadCaseName);

            var model = new Model();
            model.supports.Add(support);
            model.nodes.Add(node);

            // Act
            var bhomModel = model.ToBhOMModel();
            var bhomLoad = bhomModel.Loads.Single() as PointDisplacement;

            // Assert
            var expectedNode = new BH.oM.Structure.Elements.Node()
            {
                Name = "123",
                Position = new Point
                {
                    X = 4,
                    Y = 5,
                    Z = 6
                },
                Support = new Constraint6DOF
                {
                    TranslationX = DOFType.Fixed,
                    TranslationY = DOFType.Free,
                    TranslationZ = DOFType.Fixed,
                    RotationX = DOFType.Free,
                    RotationY = DOFType.Fixed,
                    RotationZ = DOFType.Free
                },
            };

            var expectedLoad = new PointDisplacement()
            {
                Loadcase = new Loadcase() { Name = loadCaseName, Number = 0 },
                Objects =
                    new BHoMGroup<BH.oM.Structure.Elements.Node>()
                    {
                        Elements = new List<BH.oM.Structure.Elements.Node> { expectedNode }
                    },
                Translation = prescribedTranslations.ToBhOM(),
                Rotation = prescribedRotations.ToBhOM(),
                Axis = LoadAxis.Global,
                Projected = false
            };

            CustomAssert.BhOMObjectsAreEqual(bhomModel.Nodes[123], expectedNode);
            CustomAssert.BhOMObjectsAreEqual(bhomModel.Loads.Single(), expectedLoad);
            Assert.AreEqual(bhomModel.Nodes[123].BHoM_Guid, bhomLoad.Objects.Elements.Single().BHoM_Guid);
            Assert.AreEqual(bhomModel.LoadCases.Single().BHoM_Guid, bhomLoad.Loadcase.BHoM_Guid);
        }

        [Test]
        public void ModelBeam_ConversionTest()
        {
            // Arrange
            var model = CreateHingedBeamModel();
            var beam = model.elems.Single() as ModelBeam;

            // Act
            var bhomModel = model.ToBhOMModel();
            var bhomBeam = bhomModel.Elements1D.Values.Single();

            // Assert
            var expectedBeam = new Bar()
            {
                Name = "0",
                StartNode = bhomModel.Nodes[0],
                EndNode = bhomModel.Nodes[1],
                SectionProperty = beam.crosec.ToBhOM(),
                FEAType = BarFEAType.Flexural
            };
            CustomAssert.BhOMObjectsAreEqual(bhomBeam, expectedBeam);
        }

        [Test]
        public void ModelSpring_ConversionTest()
        {
            // Arrange
            Node[] nodes = { new Node(0, Point3.Zero), new Node(1, new Point3(1, 0, 0)) };
            var spring = new ModelSpring(0, new BuilderBeam(), nodes.Select(n => n.ind).ToList(), nodes.ToList());
            var model = new Model();
            model.elems.Add(spring);

            // Act
            var bhomModel = model.ToBhOMModel();

            // Assert
            var expectedMessage = BH.Engine.Base.Query.AllEvents().Single();
            Assert.AreEqual(expectedMessage.Type, EventType.Warning);
            StringAssert.Contains(
                expectedMessage.Message,
                string.Format(Resource.WarningNotSupportedType, spring.GetType().Name));
        }

        [Test]
        public void ModelTruss_ConversionTest()
        {
            // Arrange
            var model = CreateHingedTrussModel();
            var truss = model.elems.Single() as ModelTruss;

            // Act
            var bhomModel = model.ToBhOMModel();
            var bhomTruss = bhomModel.Elements1D.Values.Single();

            // Assert
            var expectedTruss = new Bar()
            {
                Name = "0",
                StartNode = bhomModel.Nodes[0],
                EndNode = bhomModel.Nodes[1],
                SectionProperty = truss.crosec.ToBhOM(),
                FEAType = BarFEAType.Axial,
            };
            CustomAssert.BhOMObjectsAreEqual(bhomTruss, expectedTruss);
        }

        [Test]
        public void ModelBeam_WithEccentricity_ConversionTest()
        {
            // Arrange
            var rad = Math.PI / 4;
            var model = CreateHingedBeamModel(orientationAngle: rad);
            var beam = model.elems.Single() as ModelBeam;
            var beamBuilder = beam.BuilderElement();
            var eccentricity = new Vector3(0.1, 0.1, 0);
            beamBuilder.ecce_glo = eccentricity;
            beamBuilder.ecce_loc = eccentricity;
            var crossSection = beam.crosec;
            crossSection.ecce_loc = eccentricity;

            // Act
            var bhomModel = model.ToBhOMModel();
            var bhomBeam = bhomModel.Elements1D.Values.Single();
            

            // Assert
            // The local coordinate system is rotated of Pi/4 around the X global axis.
            // Therefore we expect a total eccentricity e_total = e_global + e_local.
            // Where e_local has been evaluated with respect to the local coordinate system of the beam
            // and consists of eccentricities given for the beam and for the cross section.
            var tolerance = 1E-5;
            Assert.AreEqual(bhomBeam.Offset.Start, bhomBeam.Offset.End);
            Assert.That(bhomBeam.Offset.Start.X, Is.EqualTo(0.3).Within(tolerance));
            Assert.That(bhomBeam.Offset.Start.Y, Is.EqualTo(0.1 * (1 + Math.Sqrt(2))).Within(tolerance));
            Assert.That(bhomBeam.Offset.Start.Z, Is.EqualTo(0.1 * Math.Sqrt(2)).Within(tolerance));
            Assert.That(bhomBeam.OrientationAngle, Is.EqualTo(rad));
        }
    }
}