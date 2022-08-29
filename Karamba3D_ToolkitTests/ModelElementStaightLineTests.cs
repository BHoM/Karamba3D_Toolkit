namespace Karamba3D_ToolkitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Geometry;
    using BH.oM.Structure.Elements;
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
        public void CreateBeamToTest()
        {
            
        }

        [Test]
        public void ToBhOMTest()
        {
            // Arrange
            //var points = new[] { new Point3(0, 0, 0), new Point3(1, 0, 0) };
            //var builderBeam = new BuilderBeam()
            //{
            //    Pos = new BuilderElementPositionByPoints(points),
            //    ecce_glo = 0.5 * Vector3.UnitY,
            //    ecce_loc = 0.5 * Vector3.UnitY,
            //};
            //var modelBuilder = new ModelBuilder(0.1);
            //var model = modelBuilder.build(
            //    points,
            //    Enumerable.Empty<FemMaterial>().ToList(),
            //    Enumerable.Empty<CroSec>().ToList(),
            //    Enumerable.Empty<Support>().ToList(),
            //    Enumerable.Empty<Load>().ToList(),
            //    new[] { builderBeam },
            //    Enumerable.Empty<ElemSet>().ToList(),
            //    Enumerable.Empty<Joint>().ToList(),
            //    new MessageLogger());

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
        public void CoordinateSystemTest()
        {
            //Point3[] points =
            //{
            //    Point3.Zero,
            //    new Point3(1, 1, 1),
            //    new Point3(-1, 1, 1),
            //    new Point3(-1, -1, 1),
            //    new Point3(1, -1, 1),
            //    new Point3(1, 1, -1),
            //    new Point3(-1, 1, -1),
            //    new Point3(-1, -1, -1),
            //    new Point3(1, -1, -1),
            //};

            //int i = 0;
            //var nodes = points.Select(p => new Node(i++, p));

            //int i = 0;
            //var beams = points.Select(p => new ModelBeam(i++, new BuilderBeam(),) Node(i++, p));

        }



    }
}