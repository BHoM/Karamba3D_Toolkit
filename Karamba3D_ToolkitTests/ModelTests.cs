namespace Karamba3D_ToolkitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using BH.Engine.Adapters.Karamba3D;
    using Karamba.Elements;
    using Karamba.Geometry;
    using Karamba.Loads;
    using Karamba.Models;
    using Karamba.Nodes;
    using Karamba.Supports;
    using KarambaCommon;
    using NUnit.Framework;

    [TestFixture]
    public class ModelTests
    {
        public static Model CreateHingedBeamModel(Point3 start = default, Point3 end = default)
        {
            start = start == default ? Point3.Zero : start;
            end = end == default ? new Point3(1, 0, 0) : end;

            Node[] nodes = { new Node(0, start), new Node(1, start) };
            Support[] supports =
            {
                new Support(0, new [] {true, true, true, true, true, true }, Plane3.WorldXY ),
                new Support(1, new [] {true, true, true, false, false, false }, Plane3.WorldXY)
            };
            BuilderElement[] beams = { new BuilderBeam(start, end), };

            var model = new Toolkit().Model.AssembleModel(
                beams.ToList(),
                supports.ToList(),
                new List<Load>(),
                out _,
                out _,
                out _,
                out _,
                out _);

            return model;
        }

        [Test]
        public void Test()
        {
            var model = CreateHingedBeamModel();
            var test = model.ToBhOM().ToArray();

            // Test spring
            // Test Joint
            // Test all type of load
            // Test load case assign for the bhom Model
            // Test support
            // Test support with local coosys
            // Test support with prescribed displacement
            // Test beam
            // Test truss
            // Test eccentricity on beam
            // Test releases on beam, truss
            // Test orientation of local coosys for beam
            // Ask for an implementation of sprin as non rigid link
        }
    }
}