/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.Joints;
using Karamba.Loads;
using Karamba.Materials;
using Karamba.Models;
using Karamba.Supports;
using Karamba.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Node = Karamba.Nodes.Node;

namespace Karamba3D_Engine_Tests
{
    public static class TestUtilities
    {
        /// <summary>
        /// Create a 3 hinges beam with different lengths. The beam's lengths are in order
        /// 8, 16 and 8 units.
        /// </summary>
        /// <param name="additionalEntities">Additional entities of a <see cref="Model"/> that will be added to the it.</param>
        /// <returns>The <see cref="Model"/>.</returns>
        public static Model Create3NotEqualLengthHingesBeam(params object[] additionalEntities)
        {
            /*      Beam0        Beam1        Beam2
             *   o--------o----------------o--------o
             *   Δ        Δ                Δ        Δ
             *   P0       P1               P2       P3
             */

            var points = new List<Point3>
            {
                Point3.Zero, 
                new Point3(8, 0, 0),
                new Point3(8 + 16, 0, 0), 
                new Point3(8 + 16 + 8, 0, 0)
            };
            var supports = new List<Support>
            {
                new Support(0, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(2, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
                new Support(3, new[] { true, true, true, false, false, true }, Plane3.WorldXY),
            };
            var beamBuilders = new List<BuilderElement>
            {
                new BuilderBeam(points[0], points[1]),
                new BuilderBeam(points[1], points[2]),
                new BuilderBeam(points[2], points[3])
            };
            var crossSection = new CroSec_Circle();
            beamBuilders.ForEach(b => b.crosec = crossSection);
            int counter = 0;
            beamBuilders.ForEach(b => b.id = $"beam{counter++}");

            var model = BuildModel(points, supports, beamBuilders, additionalEntities);
            return model;
        }

        /// <summary>
        /// Create a simple fixed fixed beam with a length of 10 units.
        /// </summary>
        /// <param name="additionalEntities">Additional entities of a <see cref="Model"/> that will be added to the it.</param>
        /// <returns>The <see cref="Model"/>.</returns>
        public static Model CreateFixedFixedBeam(params object[] additionalEntities)
        {
            /*          Beam0      
             *   |------------------|
             *   P0                 P1
             */

            var points = new List<Point3>
            {
                Point3.Zero, 
                new Point3(10, 0, 0)
            };
            var supports = new List<Support>
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
            };
            var beamBuilders = new List<BuilderElement>
            {
                new BuilderBeam(points[0], points[1]),
            };
            var crossSection = new CroSec_Circle();
            beamBuilders.ForEach(b => b.crosec = crossSection);

            var model = BuildModel(points, supports, beamBuilders, additionalEntities);
            return model;
        }

        /// <summary>
        /// Create a simple fixed fixed beam with a length of 10 units.
        /// </summary>
        /// <param name="additionalEntities">Additional entities of a <see cref="Model"/> that will be added to the it.</param>
        /// <returns>The <see cref="Model"/>.</returns>
        public static Model CreateFixedFreeBeam(params object[] additionalEntities)
        {
            /*          Beam0      
             *   |------------------
             *   P0                 
             */

            var points = new List<Point3>
            {
                Point3.Zero, 
                new Point3(10, 0, 0)
            };
            var supports = new List<Support>
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
            };
            var beamBuilders = new List<BuilderElement>
            {
                new BuilderBeam(points[0], points[1]),
            };
            var crossSection = new CroSec_Circle();
            beamBuilders.ForEach(b => b.crosec = crossSection);

            var model = BuildModel(points, supports, beamBuilders, additionalEntities);
            return model;
        }



        /// <summary>
        /// Create a king post model with trusses.
        /// </summary>
        /// <param name="additionalEntities">Additional entities of a <see cref="Model"/> that will be added to the it.</param>
        /// <returns>The <see cref="Model"/>.</returns>
        public static Model CreateKingPostTruss(params object[] additionalEntities)
        {
            //               O  Node 0
            //              /|\
            //             / | \
            //        T0  /  |  \ T3
            //           / T4|   \
            //          /    |    \
            //         /  T1 |  T2 \
            // Node 1 O------O------O Node 2
            //        Δ    Node 3   □
            var points = new[]
            {
                new Point3(0, 0.5, 0),
                new Point3(0, 0, 0),
                new Point3(1, 0, 0),
                new Point3(0.5, 0, 0)
            };

            var supports = new[]
            {
                new Support(0, new[] { true, true, true, false, false, false }, Plane3.Default),
                new Support(1, new[] { false, false, false, false, false, false }, Plane3.Default)
            };

            var trussBuilders = new[]
            {
                new BuilderBeam(points[0], points[1]),
                new BuilderBeam(points[1], points[3]),
                new BuilderBeam(points[3], points[2]),
                new BuilderBeam(points[2], points[0]),
                new BuilderBeam(points[0], points[3]),
            };

            var crossSection = new CroSec_Circle();
            foreach (var truss in trussBuilders)
            {
                truss.bending_stiff = false;
                truss.crosec = crossSection;
            }

            var model = BuildModel(points, supports, trussBuilders, additionalEntities);
            return model;
        }

        public static Model CreateEightBeamModelToTest(double rotationAngle = 0.0)
        {
            var points = new[]
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
                var builder = new BuilderBeam
                {
                    Ori = new BuilderElementStraightLineOrientation(
                        null,
                        new List<double>
                        {
                            rotationAngle
                        }),
                };
                return new ModelBeam(i, builder, indices, beamNodes.ToList());
            }).ToList();

            var model = new Model();
            model.nodes.AddRange(nodes);
            model.elems.AddRange(beams);
            return model;
        }

        public static Model CreateHingedBeamModel(double orientationAngle = 0.0, params object[] additionalEntities)
        {
            /*          Beam0      
            *   o------------------o
            *   Δ                  Δ
            *   P0                 P1
            */

            var points = new[]
            {
                new Point3(0, 0.0, 0),
                new Point3(1, 0, 0),
            };
            var supports = new[]
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, false }, Plane3.WorldXY),
            };
            var beamBuilders = new[]
            {
                new BuilderBeam(points[0], points[1]),
            };
            if (orientationAngle != 0)
            {
                var writer = beamBuilders[0].Ori.Writer;
                writer.Alpha = orientationAngle;
                beamBuilders[0].Ori = writer.Reader;
            }
            beamBuilders[0].crosec = new CroSec_Circle();

            var model = BuildModel(points, supports, beamBuilders, additionalEntities);
            return model;
        }

        public static Model CreateHingedTrussModel(params object[] additionalEntities)
        {
            /*          Beam0      
            *   o------------------o
            *   Δ                  Δ
            *   P0                 P1
            */

            var points = new[]
            {
                new Point3(0, 0.0, 0),
                new Point3(1, 0, 0),
            };
            var supports = new[]
            {
                new Support(0, new[] { true, true, true, true, true, true }, Plane3.WorldXY),
                new Support(1, new[] { true, true, true, false, false, false }, Plane3.WorldXY),
            };
            var beamBuilders = new List<BuilderElement>
            {
                new BuilderBeam(points[0], points[1]),
            };
            beamBuilders[0].bending_stiff = false;
            var crossSection = new CroSec_Circle();
            beamBuilders.ForEach(b => b.crosec = crossSection);

            var model = BuildModel(points, supports, beamBuilders, additionalEntities);
            return model;
        }

        private static Model BuildModel(
            IEnumerable<Point3> points,
            IEnumerable<Support> supports,
            IEnumerable<BuilderElement> beamBuilders,
            object[] additionalEntities)
        {
            
            AddAdditionalModelObjects(out var materials, out var crossSections, out var loads, out var joints, additionalEntities);

            var modelBuilder = new ModelBuilder(0.005);
            var model = modelBuilder.build(
                points.ToList(),
                materials,
                crossSections,
                supports.ToList(),
                loads,
                beamBuilders.ToList(),
                Enumerable.Empty<ElemSet>().ToList(),
                joints,
                new MessageLogger());
            return model;
        }

        private static void AddAdditionalModelObjects(
            out List<FemMaterial> materials,
            out List<CroSec> crossSections,
            out List<Load> loads,
            out List<Joint> joints,
            params object[] additionalModelObjects)
        {
            materials = new List<FemMaterial>();
            crossSections = new List<CroSec>();
            loads = new List<Load>();
            joints = new List<Joint>();
            foreach (var entity in additionalModelObjects)
            {
                switch (entity)
                {
                    case FemMaterial femMaterial:
                        materials.Add(femMaterial);
                        continue;

                    case CroSec crossSection:
                        crossSections.Add(crossSection);
                        continue;

                    case Load load:
                        loads.Add(load);
                        continue;

                    case Joint joint:
                        joints.Add(joint);
                        continue;

                    default:
                        throw new ArgumentException($"{entity.GetType()} cannot be used to create the model.");
                }
            }
        }
    }
}