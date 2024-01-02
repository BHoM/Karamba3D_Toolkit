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

namespace Karamba3D_ToolkitTests
{
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
            var bhomModel = model.ToBHoMModel();

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
            CustomAsserts.BhOMObjectsAreEqual(bhomModel.Nodes[123], expectedNode);
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
            var bhomModel = model.ToBHoMModel();

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
            CustomAsserts.BhOMObjectsAreEqual(bhomModel.Nodes[345], expectedNode);
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
            var bhomModel = model.ToBHoMModel();
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
                Loadcase = new Loadcase() { Name = loadCaseName, Number = 1 },
                Objects =
                    new BHoMGroup<BH.oM.Structure.Elements.Node>()
                    {
                        Elements = new List<BH.oM.Structure.Elements.Node> { expectedNode }
                    },
                Translation = prescribedTranslations.ToBHoM(),
                Rotation = prescribedRotations.ToBHoM(),
                Axis = LoadAxis.Global,
                Projected = false
            };

            CustomAsserts.BhOMObjectsAreEqual(bhomModel.Nodes[123], expectedNode);
            CustomAsserts.BhOMObjectsAreEqual(bhomModel.Loads.Single(), expectedLoad);
            Assert.AreEqual(bhomModel.Nodes[123].BHoM_Guid, bhomLoad.Objects.Elements.Single().BHoM_Guid);
            Assert.AreEqual(bhomModel.LoadCases.Single().BHoM_Guid, bhomLoad.Loadcase.BHoM_Guid);
        }
    }
}
