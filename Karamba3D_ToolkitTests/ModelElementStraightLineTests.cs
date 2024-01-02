/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.Engine.Adapter.Karamba3D;
using BH.Engine.Adapters.Karamba3D;
using BH.oM.Geometry;
using BH.oM.Structure.Constraints;
using BH.oM.Structure.Elements;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.Joints;
using Karamba.Models;
using Karamba3D_Engine;
using NUnit.Framework;
using System;
using System.Linq;
using Node = Karamba.Nodes.Node;

namespace Karamba3D_ToolkitTests
{
    [TestFixture]
    public class ModelElementStraightLineTests : BaseTest
    {
        [Test]
        public void CoordinateSystemTest_LocalCoordinateSystems_HaveSameConvention()
        {
            // Arrange
            var model = TestUtilities.CreateEightBeamModelToTest();
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            // Act
            var bhomModel = model.ToBHoMModel();
            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBHoM()).ToList();
            var bhomCoordinateSystem = bhomModel.Elements1D.Values.Select(BH.Engine.Structure.Query.CoordinateSystem).ToList();

            // Assert
            for (int i = 0; i < k3dCoordinateSystems.Count; i++)
            {
                CustomAsserts.BhOMObjectsAreEqual(
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
            var model = TestUtilities.CreateEightBeamModelToTest(rotationAngle);
            var beams = model.elems.OfType<ModelBeam>();
            var nodes = model.nodes;

            // Act
            var bhomModel = model.ToBHoMModel();
            var k3dCoordinateSystems = beams.Select(b => b.localCoSys(nodes).ToBHoM()).ToList();
            var bhomCoordinateSystem = bhomModel.Elements1D.Values.Select(BH.Engine.Structure.Query.CoordinateSystem).ToList();

            // Assert
            double tolerance = 1E-5;
            for (int i = 0; i < k3dCoordinateSystems.Count; i++)
            {
                CustomAsserts.BhOMObjectsAreEqual(
                    k3dCoordinateSystems[i],
                    bhomCoordinateSystem[i],
                    new BhOMEqualityTestOptions { FailureMessage = $"Iteration {i} failed." , DoubleTolerance = tolerance});
            }
        }

        [Test]
        public void ModelBeam_ConversionTest()
        {
            // Arrange
            var model = TestUtilities.CreateHingedBeamModel();
            var beam = model.elems.Single() as ModelBeam;

            // Act
            var bhomModel = model.ToBHoMModel();
            var bhomBeam = bhomModel.Elements1D.Values.Single();
            var bhomCrossSection = bhomModel.CrossSections.Single().Value;

            // Assert
            var expectedBeam = new Bar()
            {
                Name = "0",
                StartNode = bhomModel.Nodes[0],
                EndNode = bhomModel.Nodes[1],
                SectionProperty = bhomCrossSection,
                FEAType = BarFEAType.Flexural
            };
            CustomAsserts.BhOMObjectsAreEqual(bhomBeam, expectedBeam);
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
            var bhomModel = model.ToBHoMModel();

            // Assert
            var expectedMessage = string.Format(Resource.WarningNotYetSupportedType, spring.GetType().Name);
            StringAssert.Contains(expectedMessage, K3dLogger.GetWarnings().Single());
        }

        [Test]

        public void ModelTruss_ConversionTest()
        {
            // Arrange
            var model = TestUtilities.CreateHingedTrussModel();
            var truss = model.elems.Cast<ModelTruss>().Single();

            // Act
            var bhomModel = model.ToBHoMModel();
            var bhomTruss = bhomModel.Elements1D.Values.Single();
            var bhomCrossSection = bhomModel.CrossSections.Values.Single();

            // Assert
            var expectedTruss = new Bar()
            {
                Name = "0",
                StartNode = bhomModel.Nodes[0],
                EndNode = bhomModel.Nodes[1],
                SectionProperty = bhomCrossSection,
                FEAType = BarFEAType.Axial,
            };
            CustomAsserts.BhOMObjectsAreEqual(bhomTruss, expectedTruss);
        }

        [Test]
        public void ModelBeam_WithEccentricity_ConversionTest()
        {
            // Arrange
            var rad = Math.PI / 4;
            var model = TestUtilities.CreateHingedBeamModel(orientationAngle: rad);
            var beam = model.elems.Single() as ModelBeam;
            var beamBuilder = beam.BuilderElement();
            var eccentricity = new Vector3(0.1, 0.1, 0);
            beamBuilder.ecce_glo = eccentricity;
            beamBuilder.ecce_loc = eccentricity;
            var crossSection = beam.crosec;
            crossSection.ecce_loc = eccentricity;

            // Act
            var bhomModel = model.ToBHoMModel();
            var bhomBeam = bhomModel.Elements1D.Values.Single();
            

            // Assert
            // The local coordinate system is rotated of Pi/4 around the X global axis.
            // Therefore we expect a total eccentricity e_total = e_global + e_local.
            // Where e_local has been evaluated with respect to the local coordinate system of the beam
            // and consists of eccentricities given for the beam and for the cross section.
            var expectedVector = new Vector()
            {
                X = 0.3,
                Y = 0.1 * (1 + Math.Sqrt(2)),
                Z = 0.1 * Math.Sqrt(2)
            };
            Assert.AreEqual(bhomBeam.Offset.Start, bhomBeam.Offset.End);
            CustomAsserts.BhOMObjectsAreEqual(bhomBeam.Offset.Start, expectedVector, new BhOMEqualityTestOptions { DoubleTolerance = 1E-5});
            Assert.That(bhomBeam.OrientationAngle, Is.EqualTo(rad));
        }

        [Test]
        public void ModelBeam_WithReleases_ConversionTest()
        {
            // Arrange
            var model = TestUtilities.CreateHingedBeamModel();
            var beam = model.elems.Single() as ModelBeam;
            beam.joint = new Joint { c = new double?[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12} };

            // Act
            var bhomModel = model.ToBHoMModel();
            var bhomRelease = bhomModel.Elements1D.Values.Single().Release;

            // Assert
            var expected = new BarRelease
            {
                StartRelease = new Constraint6DOF()
                {
                    TranslationX = DOFType.Spring,
                    TranslationY = DOFType.Spring,
                    TranslationZ = DOFType.Spring,
                    RotationX = DOFType.Spring,
                    RotationY = DOFType.Spring,
                    RotationZ = DOFType.Spring,
                    TranslationalStiffnessX = 1,
                    TranslationalStiffnessY = 2,
                    TranslationalStiffnessZ = 3,
                    RotationalStiffnessX = 4,
                    RotationalStiffnessY = 5,
                    RotationalStiffnessZ = 6
                },
                EndRelease = new Constraint6DOF()
                {
                    TranslationX = DOFType.Spring,
                    TranslationY = DOFType.Spring,
                    TranslationZ = DOFType.Spring,
                    RotationX = DOFType.Spring,
                    RotationY = DOFType.Spring,
                    RotationZ = DOFType.Spring,
                    TranslationalStiffnessX = 7,
                    TranslationalStiffnessY = 8,
                    TranslationalStiffnessZ = 9,
                    RotationalStiffnessX = 10,
                    RotationalStiffnessY = 11,
                    RotationalStiffnessZ = 12
                },
            };
            CustomAsserts.BhOMObjectsAreEqual(bhomRelease, expected);
        }
    }
}
