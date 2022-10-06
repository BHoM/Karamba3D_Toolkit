namespace Karamba3D_ToolkitTests
{
    using BH.Engine.Adapters.Karamba3D;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Models;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;
    using Node = Karamba.Nodes.Node;

    [TestFixture]
    public class ModelElementStaightLineTests
    {
        private Model CreateEightBeamModelToTest(double rotationAngle = 0.0)
        {
            Point3[] points =
            {
                Point3.Zero,
                new Point3(1, 1, 1),
                new Point3(-1, 1, 1),
                new Point3(-1, -1, 1),
                new Point3(1, -1, 1),
                new Point3(1, 1, -1),
                new Point3(-1, 1, -1),
                new Point3(-1, -1, -1),
                new Point3(1, -1, -1),
            };

            int i = 0;
            var nodes = points.Select(p =>  new Node(i++, p)).ToList();

            i = 0;
            var baseNode = new Node(0, points[0]);
            var beams = points.Skip(1).Select(p =>
            {
                i++;
                int[] indices = { 0, i };
                Node[] beamNodes = { baseNode, new Node(i, points[i]) };
                var builder = new BuilderBeam();
                builder.Ori = new BuilderElementStraightLineOrientation(null, new List<double> { rotationAngle });
                return new ModelBeam(i, builder, indices, beamNodes.ToList());
            }).ToList();

            var model = new Model();
            model.nodes.AddRange(nodes);
            model.elems.AddRange(beams);
            return model;
        }

        [Test]
        public void CoordinateSystemTest_LocalCoordinateSystems_HaveSameConvention()
        {
            // Arrange
            var model = CreateEightBeamModelToTest();
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            // Act
            var bhomModel = model.ToBhOMModel();
            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBhOM()).ToList();
            var bhomCoordinateSystem = bhomModel.Elements1D.Values.Select(BH.Engine.Structure.Query.CoordinateSystem).ToList();

            // Assert
            for (int i = 0; i < k3dCoordinateSystems.Count; i++)
            {
                CustomAssert.BhOMObjectsAreEqual(
                    k3dCoordinateSystems[i],
                    bhomCoordinateSystem[i],
                    new BhOMEqualityTestOptions() { FailureMessage = $"Iteration {i} failed." });
            }
        }

        [Test]
        public void CoordinateSystemTest_WithRotationAngle()
        {
            // Arrange
            var rotationAngle = 45;
            var model = CreateEightBeamModelToTest(rotationAngle);
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            // Act
            var bhomModel = model.ToBhOMModel();
            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBhOM()).ToList();
            var bhomCoordinateSystem = bhomModel.Elements1D.Values.Select(BH.Engine.Structure.Query.CoordinateSystem).ToList();

            // Assert
            double tolerance = 1E-5;
            for (int i = 0; i < k3dCoordinateSystems.Count; i++)
            {
                CustomAssert.BhOMObjectsAreEqual(
                    k3dCoordinateSystems[i],
                    bhomCoordinateSystem[i],
                    new BhOMEqualityTestOptions { FailureMessage = $"Iteration {i} failed." , DoubleTolerance = tolerance});
            }
        }



    }
}