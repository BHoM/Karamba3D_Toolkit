using BH.oM.Structure.MaterialFragments;
using Karamba.Materials;
using Karamba.Models;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        private static IMaterialFragment ToBHoM(this FemMaterial_Isotrop k3dMaterial, Karamba.Models.Model k3dModel, BHoMModel bhomModel)
        {
            if (bhomModel.Materials.TryGetValue(k3dMaterial.guid, out var bhomMaterial))
            {
                return bhomMaterial;
            }

            IIsotropic bhomIsotropicMaterial;
            switch (GetMaterialType(k3dMaterial))
            {
                case MaterialType.Aluminium:
                {
                    bhomIsotropicMaterial = new Aluminium();
                    break;
                }

                case MaterialType.Steel:
                {
                    bhomIsotropicMaterial = new Steel()
                    {
                        UltimateStress = k3dMaterial.ft(),
                        YieldStress = k3dMaterial.ft(),
                    };
                    break;
                }

                case MaterialType.Concrete:
                {
                    bhomIsotropicMaterial = new Concrete()
                    {
                        CylinderStrength = -k3dMaterial.fc(),
                        CubeStrength = default,
                    };
                    break;
                }

                default:
                {
                    bhomIsotropicMaterial = new GenericIsotropicMaterial();
                    break;
                }
            }

            bhomIsotropicMaterial.Density = k3dMaterial.gamma();
            bhomIsotropicMaterial.DampingRatio = default;
            bhomIsotropicMaterial.Name = k3dMaterial.name;
            bhomIsotropicMaterial.PoissonsRatio = k3dMaterial.nue12();
            bhomIsotropicMaterial.ThermalExpansionCoeff = k3dMaterial.alphaT();
            bhomIsotropicMaterial.YoungsModulus = k3dMaterial.E();

            bhomIsotropicMaterial.BHoM_Guid = k3dMaterial.guid;
            bhomModel.Materials.Add(bhomIsotropicMaterial.BHoM_Guid, bhomIsotropicMaterial);

            return bhomIsotropicMaterial;
        }
    }
}


