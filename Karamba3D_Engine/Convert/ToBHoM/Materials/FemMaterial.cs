using BH.oM.Structure.MaterialFragments;
using Karamba.Materials;
using Karamba.Models;

namespace BH.Engine.Adapters.Karamba3D
{
    public static partial class Convert
    {
        private static MaterialType GetMaterialType(FemMaterial material)
        {
            switch (material.family)
            {
                case "Steel":
                    return MaterialType.Steel;

                case "Wood":
                case "Hardwood":
                case "ConiferousTimber":
                case "GlulamTimber":
                    return MaterialType.Timber;

                case "Aluminum":
                    return MaterialType.Aluminium;

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


