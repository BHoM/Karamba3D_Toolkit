namespace BH.Engine.Adapters.Karamba3D
{
    using BH.oM.Structure.MaterialFragments;
    using Karamba.Materials;
    using System;
    using Adapter.Karamba3D;
    using Karamba.Elements;
    using Karamba3D_Engine;

    public static partial class Convert
    {
        public static IMaterialFragment ToBhOM(this Karamba.Materials.FemMaterial k3dMaterial)
        {
            if (k3dMaterial is FemMaterial_Isotrop k3dIsotropic)
                return k3dIsotropic.ToBhOM();

            K3dLogger.RecordWarning(string.Format(Resource.WarningNotSupportedType, k3dMaterial.GetType().Name));
            return null;
        }
    }
}


