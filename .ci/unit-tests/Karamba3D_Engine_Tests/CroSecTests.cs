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

using BH.Engine.Adapters.Karamba3D;
using Karamba.CrossSections;
using NUnit.Framework;
using System.Linq;

namespace Karamba3D_Engine_Tests
{
    [TestFixture]
    public class CroSecTests : BaseTest
    {
        private CroSec  CreateCrossSectionToTest()
        {
            var material = MaterialTests.CreateMaterialToTest();
            return new CroSec_Box("RandomFamily", "RandomName", "RandomCountry", null, material);
        }

        [Test]
        public void CrossSections_WithSameGuids_WillBeInstancedOnce_Test()
        {
            // Arrange
            var crossSection = CreateCrossSectionToTest();
            crossSection.AddElemId(string.Empty); // the empty string means it will apply to all beams.
            var model = TestUtilities.Create3NotEqualLengthHingesBeam(crossSection);

            // Act
            var bhomModel = model.ToBHoM();
            var bhomMaterial = bhomModel.CrossSections.Single();

            // Assert
            Assert.AreEqual(bhomMaterial.BHoM_Guid, crossSection.guid);
        }
    }
}
