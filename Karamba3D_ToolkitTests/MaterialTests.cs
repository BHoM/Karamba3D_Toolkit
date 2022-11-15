namespace Karamba3D_ToolkitTests
{
    using BH.Engine.Adapters.Karamba3D;
    using BH.oM.Structure.MaterialFragments;
    using Karamba.Materials;
    using NUnit.Framework;
    using System;
    using System.Linq;

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
            var bhomModel = model.ToBhOM();
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
            var bhomModel = model.ToBhOM();
            var bhomMaterial = bhomModel.Materials.Single();

            // Assert
            Assert.That(bhomMaterial, Is.TypeOf(instanceType));
        }
    }
}