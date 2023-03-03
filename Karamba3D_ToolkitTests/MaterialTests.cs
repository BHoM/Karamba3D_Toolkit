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

using BH.Engine.Adapters.Karamba3D;
using BH.oM.Structure.MaterialFragments;
using Karamba.Materials;
using NUnit.Framework;
using System;
using System.Linq;

namespace Karamba3D_ToolkitTests
{
    public class MaterialTests
    {
        internal static FemMaterial_Isotrop CreateMaterialToTest(string name = null, string family = null)
        {
            return new FemMaterial_Isotrop(
                 family ?? "FamilyTestMaterial",
                name ?? "TestMaterial",
                1,
                2,
                3,
                4,
                5,
                -5,
                FemMaterial.FlowHypothesis.mises,
                7,
                null);
        }

        [Test]
        public void Material_WithSameGuids_WillBeInstancedOnce_Test()
        {
            // Arrange
            var material = CreateMaterialToTest();
            material.AddBeamId(string.Empty); // the empty string means it will apply to all beams.
            var model = TestUtilities.Create3NotEqualLengthHingesBeam(material);

            // Act
            var bhomModel = model.ToBHoM();
            var bhomMaterial = bhomModel.Materials.Single();

            // Assert
            Assert.AreEqual(bhomMaterial.BHoM_Guid, material.guid);
        }

        [TestCase("Steel", typeof(Steel))]
        [TestCase("Wood", typeof(GenericIsotropicMaterial))]
        [TestCase("Hardwood", typeof(GenericIsotropicMaterial))]
        [TestCase("ConiferousTimber", typeof(GenericIsotropicMaterial))]
        [TestCase("GlulamTimber", typeof(GenericIsotropicMaterial))]
        [TestCase("Aluminum", typeof(Aluminium))]
        [TestCase("Concrete", typeof(Concrete))]
        [TestCase("LightweightConcrete", typeof(Concrete))]
        [TestCase("ReinfSteel", typeof(GenericIsotropicMaterial))]
        [TestCase("RandomName", typeof(GenericIsotropicMaterial))]
        public void MaterialsFromDefaultTable_WillBeInstantiatedWithCorrectType_Test(string familyName, Type instanceType)
        {
            // Arrange
            var material = CreateMaterialToTest(family: familyName);
            material.AddBeamId(string.Empty); // the empty string means it will apply to all beams.
            var model = TestUtilities.CreateHingedBeamModel(additionalEntities: material);

            // Act
            var bhomModel = model.ToBHoM();
            var bhomMaterial = bhomModel.Materials.Single();

            // Assert
            Assert.That(bhomMaterial, Is.TypeOf(instanceType));
        }
    }
}