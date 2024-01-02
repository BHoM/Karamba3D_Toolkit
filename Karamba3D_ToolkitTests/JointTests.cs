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
using BH.oM.Structure.Constraints;
using Karamba.Joints;
using NUnit.Framework;

namespace Karamba3D_ToolkitTests
{
    [TestFixture]
    public class JointTests : BaseTest
    {
        [Test]
        public void ToBHoMConversionTest()
        {
            var joint = new Joint();
            joint.c[0] = 0;
            joint.c[1] = 1;
            Joint nullJoint = null;

            var barRelease = joint.ToBHoM();
            var nullBarRelease = nullJoint.ToBHoM();

            Assert.IsNull(nullBarRelease);
            Assert.AreEqual(barRelease.StartRelease.TranslationX, DOFType.Spring);
            Assert.AreEqual(barRelease.StartRelease.TranslationalStiffnessX, 0);
            Assert.AreEqual(barRelease.StartRelease.TranslationY, DOFType.Spring);
            Assert.AreEqual(barRelease.StartRelease.TranslationalStiffnessY, 1);
        }
    }
}
