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

using NUnit.Framework;

namespace Karamba3D_ToolkitTests
{
    [TestFixture] 
    public class LoadCaseTests
    {
        [Test]
        public void LoadCases_WhenMultiple_AreStoredAsExpected_Test()
        {
            //// Arrange
            //var loads = new Load[]
            //{
            //    new PointLoad(1, new Vector3(1, 2, 3), new Vector3(4, 5, 6), "LoadCase1", false),
            //    new ConcentratedForce(
            //        new[] { string.Empty },
            //        null,
            //        "LoadCase2",
            //        0.6,
            //        new Vector3(7, 8, 9),
            //        LoadOrientation.global),
            //    new ConcentratedMoment(
            //        new[] { string.Empty },
            //        null,
            //        "LoadCase1",
            //        0.6,
            //        new Vector3(10, 11, 12),
            //        LoadOrientation.global)
            //};
            //var model = TestUtilities.CreateFixedFreeBeam(loads);

            //// Act
            //var bhomModel = model.ToBHoM();
            //var loadCases = bhomModel.LoadCases;

            //// Assert
            //var expectedNames = new[] { "LoadCase1", "LoadCase2" };
            //var expectedNumbers = new[] { 0, 1 };
            //CollectionAssert.AreEqual(expectedNames, loadCases.Select(l => l.Name).ToArray());
            //CollectionAssert.AreEqual(expectedNumbers, loadCases.Select(l => l.Number).ToArray());
            Assert.Warn("TODO Update NuGet version of K3D");
        }
    }
}