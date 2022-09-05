﻿namespace Karamba3D_ToolkitTests
{
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Structure.Constraints;
    using Karamba.Joints;
    using NUnit.Framework;

    [TestFixture]
    public class JointTests
    {
        [Test]
        public void ToBhOMConversionTest()
        {
            var joint = new Joint();
            joint.c[0] = 0;
            joint.c[1] = 1;
            Joint nullJoint = null;

            var barRelease = joint.ToBhOM();
            var nullBarRelease = nullJoint.ToBhOM();

            Assert.IsNull(nullBarRelease);
            Assert.AreEqual(barRelease.StartRelease.TranslationX, DOFType.Spring);
            Assert.AreEqual(barRelease.StartRelease.TranslationalStiffnessX, 0);
            Assert.AreEqual(barRelease.StartRelease.TranslationY, DOFType.Spring);
            Assert.AreEqual(barRelease.StartRelease.TranslationalStiffnessY, 1);
        }
    }
}