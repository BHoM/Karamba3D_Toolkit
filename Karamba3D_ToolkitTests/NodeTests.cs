namespace Karamba3D_ToolkitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Base;
    using BH.oM.Geometry;
    using BH.oM.Structure.Constraints;
    using BH.oM.Structure.Loads;
    using Karamba.Geometry;
    using Karamba.Models;
    using Karamba.Nodes;
    using Karamba.Supports;
    using NUnit.Framework;

    public class NodeTests : BaseTest
    {
        [Test]
        public void Node_ConversionTest()
        {
            // Arrange
            var node = new Node(123, new Point3(1, 2, 3));
            var model = new Model();
            model.nodes.Add(node);

            // Act
            var bhomModel = model.ToBhomModel();

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
            var bhomModel = model.ToBhomModel();

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
            var bhomModel = model.ToBhomModel();
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
    }
}