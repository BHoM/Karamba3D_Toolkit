namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Structure.MaterialFragments;
    using Karamba.Materials;

    public static partial class Convert
    {
        private static IIsotropic ToBhOM(this Karamba.Materials.FemMaterial_Isotrop k3dMaterial)
        {
            // TODO: Check Karamba settings for Units of Measure (INIReader reader)
            // OR use the automated Karamba conversion (which only converts numbers if they are not in the right Unit - works only at runtime via Karamba)

            switch (GetMaterialType(k3dMaterial))
            {
                case MaterialType.Aluminium:
                {
                    return new Aluminium()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(), //(obj.E() / (2 * obj.G12())) - 1,
                        Name = k3dMaterial.name // this may store the type of aluminum
                    };
                }

                case MaterialType.Steel:
                {
                    return new Steel()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),
                        Name = k3dMaterial.name,
                        ThermalExpansionCoeff = k3dMaterial.alphaT(),
                        UltimateStress = k3dMaterial.ft(),
                        YieldStress = k3dMaterial.ft() // TODO: where's Fy in Karamba?
                    };
                }

                case MaterialType.Concrete:
                {
                    return new Concrete()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),
                        Name = k3dMaterial.name,
                        ThermalExpansionCoeff = k3dMaterial.alphaT(),
                        CylinderStrength = k3dMaterial.fc(),
                    };
                }

                default:
                {
                    return new GenericIsotropicMaterial()
                    {
                        Density = k3dMaterial.gamma(), // TODO: map units
                        YoungsModulus = k3dMaterial.E(),
                        PoissonsRatio = k3dMaterial.nue12(),//(obj.E() / (2 * obj.G12())) - 1,
                        Name = k3dMaterial.name
                    };
                }
            }
        }

        private static MaterialType GetMaterialType(FemMaterial material)
        {
            // TODO Should search inside K3D database. This require to have static method in Karamba to find our database.

            switch (material.family)
            {
                case "Steel":
                    return MaterialType.Steel;

                case "Wood":
                case "Hardwood":
                case "ConiferousTimber":
                case "GlulamTimber":
                case "Aluminum":
                    return MaterialType.Timber;

                case "Concrete":
                case "LightweightConcrete":
                    return MaterialType.Concrete;

                case "ReinfSteel":
                    return MaterialType.Rebar;

                default:
                    return MaterialType.Undefined;
            }
        }
    }
}


