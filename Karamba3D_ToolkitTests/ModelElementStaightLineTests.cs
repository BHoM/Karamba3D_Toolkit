namespace Karamba3D_ToolkitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BH.Engine.Adapters.Karamba3D;
    using BH.Engine.Geometry;
    using BH.oM.Geometry;
    using BH.oM.Geometry.CoordinateSystem;
    using BH.oM.Structure.Elements;
    using FluentAssertions;
    using Karamba.CrossSections;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Joints;
    using Karamba.Loads;
    using Karamba.Materials;
    using Karamba.Models;
    using Karamba.Supports;
    using Karamba.Utilities;
    using NUnit.Framework;
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
        public void ToBhOMTest()
        {
            // Arrange
            var model = new Model();
            var builderBeam = new BuilderBeam();
            var modelNodes = new[] { new Node(0, new Point3(0, 0, 0)), new Node(1, new Point3(1, 0, 0)) };
            var beam = new ModelBeam(1, builderBeam, new[] { 0, 1 }, new List<Node> { new Node(1, Point3.Unset) });
            beam.BuilderElement().ecce_loc = 0.5 * Vector3.UnitY;
            beam.BuilderElement().ecce_glo = 0.5 * Vector3.UnitY;

            model.nodes.AddRange(modelNodes);
            model.elems.Add(beam);

            // Act
            var bhomBar = model.ToBhOM().OfType<Bar>().First();

            // Assert
            Assert.AreEqual("1", bhomBar.Name);
            Assert.AreEqual("0", bhomBar.StartNode.Name);
            Assert.AreEqual("1", bhomBar.EndNode.Name);
            Assert.AreEqual(Point.Origin, bhomBar.StartNode.Position);
            Assert.AreEqual(BH.Engine.Geometry.Create.Point(1), bhomBar.EndNode.Position);
            Assert.AreEqual(BarFEAType.Flexural, bhomBar.FEAType);
            Assert.AreEqual(BH.Engine.Geometry.Create.Vector(0, 1, 0), bhomBar.Offset.Start);
            Assert.AreEqual(BH.Engine.Geometry.Create.Vector(0, 1, 0), bhomBar.Offset.End);
            // Assert.AreEqual(); TODO make test for cross section and use it to compare here
        }

        [Test]
        public void CoordinateSystemTest_LocalCoordinateSystems_HaveSameConvention()
        {
            // Arrange
            var model = CreateEightBeamModelToTest();
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            // Act
            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBhOM()).ToArray();
            var bhomBar = model.ToBhOM().OfType<Bar>();
            var bhomCoordinateSystem = bhomBar.Select(BH.Engine.Structure.Query.CoordinateSystem).ToArray();

            // Assert
            Point test1 = Create.Point(1, 2, 3);
            var test2 = Create.Point(1.1, 2.1, 3.1);

            //test1.Should().BeEquivalentTo(test2, options => options.Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, 0.1)).When(info => info));

            //for (int j = 0; j < k3dCoordinateSystems.Count(); j++)
            //{
            //    var k3dCoSys = k3dCoordinateSystems[j];
            //    k3dCoSys.X.X = k3dCoSys.X.X + 0.01;
            //    var bhomCoSys = bhomCoordinateSystem[j];
            //    k3dCoSys.Should()
            //            .BeEquivalentTo(
            //                bhomCoSys,
            //                options =>
            //                {
            //                    return options.Using<double>(d => d.Subject.Should().BeApproximately(d.Expectation, 0.1))
            //                           .WhenTypeIs<double>();
            //                });

            //}
        }

        [Test]
        public void CoordinateSystemTest_WithRotationAngle()
        {
            // Arrange
            var rotationAngle = 45;
            var model = CreateEightBeamModelToTest(rotationAngle);
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBhOM()).ToArray();
            var bhomBar = model.ToBhOM().OfType<Bar>();
            var bhomCoordinateSystem = bhomBar.Select(BH.Engine.Structure.Query.CoordinateSystem).ToArray();

            // Assert
            for (int j = 0; j < k3dCoordinateSystems.Count(); j++)
            {
                var k3dCoSys = k3dCoordinateSystems[j];
                var bhomCoSys = bhomCoordinateSystem[j];
                k3dCoSys.Should().BeEquivalentTo(bhomCoSys);
            }
        }



    }
}