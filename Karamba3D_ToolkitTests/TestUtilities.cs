namespace Karamba3D_ToolkitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using Node = Karamba.Nodes.Node;

    public static class TestUtilities
    {
        private const double _defaultLimitDistance = 0.005;

        public static Model Create3HingesBeam()
        {
            /*  P0      Beam0        P1      Beam1        P2      Beam2        P3
             *   o--------------------o--------------------o--------------------o
             *   Δ                    Δ                    Δ                    Δ
             */

            Point3[] points = { Point3.Zero, new Point3(0.25, 0, 0), new Point3(0.75, 0, 0), new Point3(1, 0, 0) };
            Support[] supports =
            {
                new Support(0, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(2, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(3, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
            };
            BuilderElement[] beamBuilders =
            {
                new BuilderBeam(points[0], points[1]),
                new BuilderBeam(points[1], points[2]),
                new BuilderBeam(points[2], points[3])
            };

            var modelBuilder = new ModelBuilder(0.005);
            var model = modelBuilder.build(
                points.ToList(),
                Enumerable.Empty<FemMaterial>().ToList(),
                Enumerable.Empty<CroSec>().ToList(),
                supports.ToList(),
                Enumerable.Empty<Load>().ToList(),
                beamBuilders.ToList(),
                Enumerable.Empty<ElemSet>().ToList(),
                Enumerable.Empty<Joint>().ToList(),
                new MessageLogger());

            return model;
        }

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
            var test = model.Elements();
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

        public static Model CreateKingPostTruss()
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
    }
}